using System;
using System.Threading;
using USBHIDDRIVER;

namespace UsbMissileConsoleTest02
{
    class Program
    {
        static void Main(string[] args)
        {
            var usb = new USBInterface(@"vid_0a81", @"pid_ff01");
            usb.Connect();
            // sample to file a missile
            WriteData(usb, 8);
            Thread.Sleep(2000);
            WriteData(usb, 0);
            Thread.Sleep(2500);
            WriteData(usb, 8);
            Thread.Sleep(2500);
            WriteData(usb, 0);
            Thread.Sleep(2500);
            Console.ReadLine();
        }
        private static void WriteData(USBInterface usb, byte secondByteValue)
        {
            var command = new byte[] { 0, 2 };
            command[1] = secondByteValue;
            usb.UsbDevice.writeDataSimple(command);
            Thread.Sleep(50);
        }
    }
}
