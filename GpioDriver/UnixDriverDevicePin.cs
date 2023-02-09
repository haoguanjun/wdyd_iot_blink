// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Device.Gpio;
using System.Device.Gpio.Drivers;

public sealed class UnixDriverDevicePin : IDisposable
{
    public UnixDriverDevicePin(PinValue currentValue)
    {
        LastValue = currentValue;
        FileDescriptor = -1;
        ActiveEdges = PinEventTypes.None;
    }

    public event PinChangeEventHandler? ValueRising;
    public event PinChangeEventHandler? ValueFalling;

    public int FileDescriptor;

    public PinEventTypes ActiveEdges
    {
        get;
        set;
    }

    public PinValue LastValue
    {
        get;
        set;
    }

    public void OnPinValueChanged(PinValueChangedEventArgs args)
    {
        if (ValueRising != null && args.ChangeType == PinEventTypes.Rising)
        {
            ValueRising?.Invoke(this, args);
        }

        if (ValueFalling != null && args.ChangeType == PinEventTypes.Falling)
        {
            ValueFalling?.Invoke(this, args);
        }
    }

    public bool IsCallbackListEmpty()
    {
        return ValueRising == null && ValueFalling == null;
    }

    public void Dispose()
    {
    }
}
