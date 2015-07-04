using System;
using USBHIDDRIVER;

namespace ConsoleApplication1
{
    internal class Program
    {
        private static USBInterface _device;

        private static void Main(string[] args)
        {
            _device = new USBInterface("vid_1294", "pid_1320");
            _device.Connect();
            var line = Console.ReadLine();
            while (line != null && line != "E")
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
