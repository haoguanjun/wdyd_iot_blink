using System;
using System.Device.Gpio;
using System.Threading;
using System.Device.Gpio.Drivers;

namespace Blink
{
    class Program
    {
        static void Main(string[] args)
        {
            // 定义引脚
            int pinNumber = 448;
            // 定义延迟时间
            int delayTime = 1000;

            // 获取 GPIO 控制器
            using(var gpioDriver = new WDYDFsDriver())
            using(var controller = new GpioController(PinNumberingScheme.Logical, gpioDriver))
            {
                // 打开引脚
                controller.OpenPin(pinNumber, PinMode.Output);

                // 循环
                while (true)
                {
                    Console.WriteLine($"Light for {delayTime}ms");
                    // 打开 LED
                    controller.Write(pinNumber, PinValue.High);
                    // 等待 1s
                    Thread.Sleep(delayTime);

                    Console.WriteLine($"Dim for {delayTime}ms");
                    // 关闭 LED
                    controller.Write(pinNumber, PinValue.Low);
                    // 等待 1s
                    Thread.Sleep(delayTime);
                }
            }
        }
    }
}
