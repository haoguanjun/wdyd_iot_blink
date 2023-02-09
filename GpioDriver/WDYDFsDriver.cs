// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace System.Device.Gpio.Drivers;

/// <summary>
/// A GPIO driver for Unix.
/// </summary>
public class WDYDFsDriver : UnixDriver
{
    private const int ERROR_CODE_EINTR = 4; // Interrupted system call

    private const string GpioBasePath = "/sys/class/gpio";
    private const string GpioChip = "gpiochip";
    private const string GpioLabel = "/label";
    private const string GpioContoller = "pinctrl";
    private const string GpioOffsetBase = "/base";
    private const int PollingTimeout = 50;

    private static readonly int s_pinOffset = ReadOffset();

    private readonly List<int> _exportedPins = new List<int>();
    public readonly Dictionary<int, UnixDriverDevicePin> _devicePins = new Dictionary<int, UnixDriverDevicePin>();
    private readonly Dictionary<int, PinValue> _pinValues = new Dictionary<int, PinValue>();
    private TimeSpan _statusUpdateSleepTime = TimeSpan.FromMilliseconds(1);
    private int _pollFileDescriptor = -1;
    private Thread? _eventDetectionThread;
    private int _pinsToDetectEventsCount;
    private CancellationTokenSource? _eventThreadCancellationTokenSource;

    private static int ReadOffset()
    {
        return 0;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SysFsDriver"/> class.
    /// </summary>
    public WDYDFsDriver()
    {
        if (Environment.OSVersion.Platform != PlatformID.Unix)
        {
            throw new PlatformNotSupportedException($"{GetType().Name} is only supported on Linux/Unix.");
        }
    }

    /// <summary>
    /// The sleep time after an event occured and before the new value is read.
    /// </summary>
    TimeSpan StatusUpdateSleepTime
    {
        get
        {
            return _statusUpdateSleepTime;
        }
        set
        {
            _statusUpdateSleepTime = value;
        }
    }

    /// <summary>
    /// The number of pins provided by the driver.
    /// </summary>
    protected override int PinCount => throw new PlatformNotSupportedException("This driver is generic so it can not enumerate how many pins are available.");

    /// <summary>
    /// Converts a board pin number to the driver's logical numbering scheme.
    /// </summary>
    /// <param name="pinNumber">The board pin number to convert.</param>
    /// <returns>The pin number in the driver's logical numbering scheme.</returns>
    protected override int ConvertPinNumberToLogicalNumberingScheme(int pinNumber) => throw new PlatformNotSupportedException("This driver is generic so it can not perform conversions between pin numbering schemes.");

    /// <summary>
    /// Opens a pin in order for it to be ready to use.
    /// This retains the pin direction, but if it is output, the value will always be low after open.
    /// </summary>
    /// <param name="pinNumber">The pin number in the driver's logical numbering scheme.</param>
    protected override void OpenPin(int pinNumber)
    {
        int pinOffset = pinNumber + s_pinOffset;
        string pinPath = $"{GpioBasePath}/gpio{pinOffset}";
        // If the directory exists, this becomes a no-op since the pin might have been opened already by the some controller or somebody else.
        if (!Directory.Exists(pinPath))
        {
            try
            {
                File.WriteAllText(Path.Combine(GpioBasePath, "export"), pinOffset.ToString(CultureInfo.InvariantCulture));
                // SysFsHelpers.EnsureReadWriteAccessToPath(pinPath);

                _exportedPins.Add(pinNumber);
                // Default value is low, otherwise it's the set pin mode with default value that will override this
                _pinValues.Add(pinNumber, PinValue.Low);
            }
            catch (UnauthorizedAccessException e)
            {
                // Wrapping the exception in order to get a better message.
                throw new UnauthorizedAccessException("Opening pins requires root permissions.", e);
            }
        }
    }

    /// <summary>
    /// Closes an open pin.
    /// </summary>
    /// <param name="pinNumber">The pin number in the driver's logical numbering scheme.</param>
    protected override void ClosePin(int pinNumber)
    {
        int pinOffset = pinNumber + s_pinOffset;
        string pinPath = $"{GpioBasePath}/gpio{pinOffset}";
        // If the directory doesn't exist, this becomes a no-op since the pin was closed already.
        if (Directory.Exists(pinPath))
        {
            try
            {
                SetPinEventsToDetect(pinNumber, PinEventTypes.None);
                if (_devicePins.ContainsKey(pinNumber))
                {
                    _devicePins[pinNumber].Dispose();
                    _devicePins.Remove(pinNumber);
                    _pinValues.Remove(pinNumber);
                }

                // If this controller wasn't the one that opened the pin, then Remove will return false, so we don't need to close it.
                if (_exportedPins.Remove(pinNumber))
                {
                    File.WriteAllText(Path.Combine(GpioBasePath, "unexport"), pinOffset.ToString(CultureInfo.InvariantCulture));
                }
            }
            catch (UnauthorizedAccessException e)
            {
                throw new UnauthorizedAccessException("Closing pins requires root permissions.", e);
            }
        }
    }

    /// <summary>
    /// Sets the mode to a pin.
    /// </summary>
    /// <param name="pinNumber">The pin number in the driver's logical numbering scheme.</param>
    /// <param name="mode">The mode to be set.</param>
    protected override void SetPinMode(int pinNumber, PinMode mode)
    {
        if (mode == PinMode.InputPullDown || mode == PinMode.InputPullUp)
        {
            throw new PlatformNotSupportedException("This driver is generic so it does not support Input Pull Down or Input Pull Up modes.");
        }

        string directionPath = GetDirectionPath(pinNumber);
        string sysFsMode = ConvertPinModeToSysFsMode(mode);
        if (File.Exists(directionPath))
        {
            try
            {
                File.WriteAllText(directionPath, sysFsMode);
            }
            catch (UnauthorizedAccessException e)
            {
                throw new UnauthorizedAccessException("Setting a mode to a pin requires root permissions.", e);
            }
        }
        else
        {
            throw new InvalidOperationException("There was an attempt to set a mode to a pin that is not open.");
        }
    }

    private string ConvertPinModeToSysFsMode(PinMode mode)
    {
        if (mode == PinMode.Input)
        {
            return "in";
        }

        if (mode == PinMode.Output)
        {
            return "out";
        }

        throw new PlatformNotSupportedException($"{mode} is not supported by this driver.");
    }

    private PinMode ConvertSysFsModeToPinMode(string sysFsMode)
    {
        sysFsMode = sysFsMode.Trim();
        if (sysFsMode == "in")
        {
            return PinMode.Input;
        }

        if (sysFsMode == "out")
        {
            return PinMode.Output;
        }

        throw new ArgumentException($"Unable to parse {sysFsMode} as a PinMode.");
    }

    /// <summary>
    /// Reads the current value of a pin.
    /// </summary>
    /// <param name="pinNumber">The pin number in the driver's logical numbering scheme.</param>
    /// <returns>The value of the pin.</returns>
    protected override PinValue Read(int pinNumber)
    {
        PinValue result = default;
        string valuePath = GetGpioValuePath(pinNumber);
        if (File.Exists(valuePath))
        {
            try
            {
                string valueContents = File.ReadAllText(valuePath);
                result = ConvertSysFsValueToPinValue(valueContents);
            }
            catch (UnauthorizedAccessException e)
            {
                throw new UnauthorizedAccessException("Reading a pin value requires root permissions.", e);
            }
        }
        else
        {
            throw new InvalidOperationException("There was an attempt to read from a pin that is not open.");
        }

        _pinValues[pinNumber] = result;
        return result;
    }

    /// <inheritdoc/>
    protected void Toggle(int pinNumber) => Write(pinNumber, !_pinValues[pinNumber]);

    private PinValue ConvertSysFsValueToPinValue(string value)
    {
        return value.Trim() switch
        {
            "0" => PinValue.Low,
            "1" => PinValue.High,
            _ => throw new ArgumentException($"Invalid GPIO pin value {value}.")
        };
    }

    /// <summary>
    /// Writes a value to a pin.
    /// </summary>
    /// <param name="pinNumber">The pin number in the driver's logical numbering scheme.</param>
    /// <param name="value">The value to be written to the pin.</param>
    protected override void Write(int pinNumber, PinValue value)
    {
        string valuePath = GetGpioValuePath( pinNumber);
        if (File.Exists(valuePath))
        {
            try
            {
                string sysFsValue = ConvertPinValueToSysFs(value);
                File.WriteAllText(valuePath, sysFsValue);
                _pinValues[pinNumber] = value;
            }
            catch (UnauthorizedAccessException e)
            {
                throw new UnauthorizedAccessException("Reading a pin value requires root permissions.", e);
            }
        }
        else
        {
            throw new InvalidOperationException("There was an attempt to write to a pin that is not open.");
        }
    }

    private string ConvertPinValueToSysFs(PinValue value)
        => value == PinValue.High ? "1" : "0";

    /// <summary>
    /// Checks if a pin supports a specific mode.
    /// </summary>
    /// <param name="pinNumber">The pin number in the driver's logical numbering scheme.</param>
    /// <param name="mode">The mode to check.</param>
    /// <returns>The status if the pin supports the mode.</returns>
    protected override bool IsPinModeSupported(int pinNumber, PinMode mode)
    {
        // Unix driver does not support pull up or pull down resistors.
        if (mode == PinMode.InputPullDown || mode == PinMode.InputPullUp)
        {
            return false;
        }

        return true;
    }

    protected override WaitForEventResult WaitForEvent(int pinNumber, PinEventTypes eventTypes, CancellationToken cancellationToken)
    {
        return new WaitForEventResult();
    }

    private void SetPinEventsToDetect(int pinNumber, PinEventTypes eventTypes)
    {

    }

    private PinEventTypes GetPinEventsToDetect(int pinNumber)
    {
        return PinEventTypes.None;
    }

    private PinEventTypes StringValueToPinEventType(string value)
    {
        return value.Trim() switch
        {
            "none" => PinEventTypes.None,
            "both" => PinEventTypes.Falling | PinEventTypes.Rising,
            "rising" => PinEventTypes.Rising,
            "falling" => PinEventTypes.Falling,
            _ => throw new ArgumentException("Invalid pin event value.", value)
        };
    }

    private string PinEventTypeToStringValue(PinEventTypes kind)
    {
        if (kind == PinEventTypes.None)
        {
            return "none";
        }

        if ((kind & PinEventTypes.Falling) != 0 && (kind & PinEventTypes.Rising) != 0)
        {
            return "both";
        }

        if (kind == PinEventTypes.Rising)
        {
            return "rising";
        }

        if (kind == PinEventTypes.Falling)
        {
            return "falling";
        }

        throw new ArgumentException("Invalid Pin Event Type.", nameof(kind));
    }

    private void AddPinToPoll(int pinNumber, ref int valueFileDescriptor, ref int pollFileDescriptor, out bool closePinValueFileDescriptor)
    {
        closePinValueFileDescriptor = true;
    }

    private unsafe bool WasEventDetected(int pollFileDescriptor, int valueFileDescriptor, out int pinNumber, CancellationToken cancellationToken)
    {
        pinNumber = 0;
        return false;
    }

    private void RemovePinFromPoll(int pinNumber, ref int valueFileDescriptor, ref int pollFileDescriptor, bool closePinValueFileDescriptor, bool closePollFileDescriptor, bool cancelEventDetectionThread)
    {
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }

    /// <summary>
    /// Adds a handler for a pin value changed event.
    /// </summary>
    /// <param name="pinNumber">The pin number in the driver's logical numbering scheme.</param>
    /// <param name="eventTypes">The event types to wait for.</param>
    /// <param name="callback">Delegate that defines the structure for callbacks when a pin value changed event occurs.</param>
    protected override void AddCallbackForPinValueChangedEvent(int pinNumber, PinEventTypes eventTypes, PinChangeEventHandler callback)
    {
        if (!_devicePins.ContainsKey(pinNumber))
        {
            _devicePins.Add(pinNumber, new UnixDriverDevicePin(Read(pinNumber)));
            _pinsToDetectEventsCount++;
            AddPinToPoll(pinNumber, ref _devicePins[pinNumber].FileDescriptor, ref _pollFileDescriptor, out _);
        }

        if ((eventTypes & PinEventTypes.Rising) != 0)
        {
            _devicePins[pinNumber].ValueRising += callback;
        }

        if ((eventTypes & PinEventTypes.Falling) != 0)
        {
            _devicePins[pinNumber].ValueFalling += callback;
        }

        PinEventTypes events = (GetPinEventsToDetect(pinNumber) | eventTypes);
        SetPinEventsToDetect(pinNumber, events);

        // Remember which events are active
        _devicePins[pinNumber].ActiveEdges = events;
        InitializeEventDetectionThread();
    }

    private void InitializeEventDetectionThread()
    {
        if (_eventDetectionThread == null)
        {
            _eventDetectionThread = new Thread(DetectEvents)
            {
                IsBackground = true
            };
            _eventThreadCancellationTokenSource = new CancellationTokenSource();
            _eventDetectionThread.Start();
        }
    }

    private void DetectEvents()
    {
        if (_eventThreadCancellationTokenSource == null)
        {
            throw new InvalidOperationException("Cannot start to detect events when CancellationTokenSource is null.");
        }

        while (_pinsToDetectEventsCount > 0)
        {
            try
            {
                bool eventDetected = WasEventDetected(_pollFileDescriptor, -1, out int pinNumber, _eventThreadCancellationTokenSource.Token);
                if (eventDetected)
                {
                    if (_statusUpdateSleepTime > TimeSpan.Zero)
                    {
                        Thread.Sleep(_statusUpdateSleepTime); // Adding some delay to make sure that the value of the File has been updated so that we will get the right event type.
                    }

                    PinValue newValue = Read(pinNumber);

                    UnixDriverDevicePin currentPin = _devicePins[pinNumber];
                    PinEventTypes activeEdges = currentPin.ActiveEdges;
                    PinEventTypes eventType = activeEdges;
                    PinEventTypes secondEvent = PinEventTypes.None;
                    // Only if the active edges are both, we need to query the current state and guess about the change
                    if (activeEdges == (PinEventTypes.Falling | PinEventTypes.Rising))
                    {
                        PinValue oldValue = currentPin.LastValue;
                        if (oldValue == PinValue.Low && newValue == PinValue.High)
                        {
                            eventType = PinEventTypes.Rising;
                        }
                        else if (oldValue == PinValue.High && newValue == PinValue.Low)
                        {
                            eventType = PinEventTypes.Falling;
                        }
                        else if (oldValue == PinValue.High)
                        {
                            // Both high -> There must have been a low-active peak
                            eventType = PinEventTypes.Falling;
                            secondEvent = PinEventTypes.Rising;
                        }
                        else
                        {
                            // Both low -> There must have been a high-active peak
                            eventType = PinEventTypes.Rising;
                            secondEvent = PinEventTypes.Falling;
                        }

                        currentPin.LastValue = newValue;
                    }
                    else
                    {
                        // Update the value, in case we need it later
                        currentPin.LastValue = newValue;
                    }

                    PinValueChangedEventArgs args = new PinValueChangedEventArgs(eventType, pinNumber);
                    currentPin.OnPinValueChanged(args);
                    if (secondEvent != PinEventTypes.None)
                    {
                        args = new PinValueChangedEventArgs(secondEvent, pinNumber);
                        currentPin.OnPinValueChanged(args);
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                break; // If cancellation token source is disposed then we need to exit this thread.
            }
        }

        _eventDetectionThread = null;
    }

    /// <summary>
    /// Removes a handler for a pin value changed event.
    /// </summary>
    /// <param name="pinNumber">The pin number in the driver's logical numbering scheme.</param>
    /// <param name="callback">Delegate that defines the structure for callbacks when a pin value changed event occurs.</param>
    protected override void RemoveCallbackForPinValueChangedEvent(int pinNumber, PinChangeEventHandler callback)
    {
        if (!_devicePins.ContainsKey(pinNumber))
        {
            throw new InvalidOperationException("Attempted to remove a callback for a pin that is not listening for events.");
        }

        _devicePins[pinNumber].ValueFalling -= callback;
        _devicePins[pinNumber].ValueRising -= callback;
        if (_devicePins[pinNumber].IsCallbackListEmpty())
        {
            _pinsToDetectEventsCount--;

            bool closePollFileDescriptor = (_pinsToDetectEventsCount == 0);
            RemovePinFromPoll(pinNumber, ref _devicePins[pinNumber].FileDescriptor, ref _pollFileDescriptor, true, closePollFileDescriptor, true);
            _devicePins[pinNumber].Dispose();
            _devicePins.Remove(pinNumber);
        }
    }

    /// <summary>
    /// Gets the mode of a pin.
    /// </summary>
    /// <param name="pinNumber">The pin number in the driver's logical numbering scheme.</param>
    /// <returns>The mode of the pin.</returns>
    protected override PinMode GetPinMode(int pinNumber)
    {
        pinNumber += s_pinOffset;
        string directionPath = GetDirectionPath(pinNumber);
        if (File.Exists(directionPath))
        {
            try
            {
                string sysFsMode = File.ReadAllText(directionPath);
                return ConvertSysFsModeToPinMode(sysFsMode);
            }
            catch (UnauthorizedAccessException e)
            {
                throw new UnauthorizedAccessException("Getting a mode to a pin requires root permissions.", e);
            }
        }
        else
        {
            throw new InvalidOperationException("There was an attempt to get a mode to a pin that is not open.");
        }
    }

    private string GetGpioFolderName( int pinNumber)
    {
        string folderName = string.Empty;
        switch( pinNumber )
        {
            case 442:
                folderName = "P40_2";
                break;
            case 448:
                folderName = "P41_0";
                break;
            case 449:
                folderName = "P41_1";
                break;
            case 497:
                folderName = "P47_1";
                break;
            case 507:
                folderName = "P48_3";
                break;
            case 508:
                folderName = "P48_4";
                break;
            default:
                throw new Exception($"Not supported pin number: {pinNumber}");
                break;
        }
        return folderName;
    }

    private string GetDirectionPath(int pinNumber)
    {
        string folderName = GetGpioFolderName(pinNumber);   
        string path = $"{GpioBasePath}/{folderName}/direction";
        return path;
    }

    private string GetGpioValuePath(int pinNumber)
    {
        string folderName = GetGpioFolderName(pinNumber);   
        string path = $"{GpioBasePath}/{folderName}/value";
        return path;
    }

    private string GetGpioEdgePath(int pinNumber)
    {
        string folderName = GetGpioFolderName(pinNumber);   
        string path = $"{GpioBasePath}/{folderName}/edge";
        return path;
    }
    
}