using System;
using USBHIDDRIVER;

namespace ConsoleApplication1
{
    internal class Program
    {
        private static USBInterface _device;

        private static void Main(string[] args)
        {
            Console.WriteLine("Press key (1 ... 8) to change color in Light Notifier");
            Console.WriteLine("Press key (E) to exit app");
            Console.WriteLine("Press other key to crash the app");

            _device = new USBInterface("vid_1294", "pid_1320");
            _device.Connect();
            var line = Console.ReadLine();
            while (line != null && line.ToUpper() != "E")
            {
                var b = Convert.ToByte(line);
                WriteDataEx(b);
                line = Console.ReadLine();
            }
            Console.ReadLine();
        }

        private static void WriteDataEx(byte byteValue)
        {
            var cmdData = new byte[] { 0, 0, 0, 0, 0, 0 };
            cmdData[1] = byteValue;
            _device.UsbDevice.writeDataSimple(cmdData);
        }
    }
}
