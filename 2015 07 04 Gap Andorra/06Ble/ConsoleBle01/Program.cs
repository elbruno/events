using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;

namespace ConsoleBle01
{
    class Program
    {
        private static string deviceName = "RS_R056712";
        private static DeviceInformation _parrot;

        static void Main(string[] args)
        {
            GetBtDevicesAndList();
            Console.ReadLine();
        }

        private static void GetBtDevicesAndList()
        {
            IReadOnlyList<string> list= new List<string>();
            Console.WriteLine("Start Get Devices");
            var bts = DeviceInformation.FindAllAsync(); // .FindAllAsync().GetResults();
            do
            {
                Thread.Sleep(100);
            } while (bts.Status != AsyncStatus.Completed);
            var i = 0;
            foreach (var di in bts.GetResults())
            {
                i++;
                if (di.Name == deviceName)
                    _parrot = di;
            }
            Console.WriteLine("{0} Devices Found", i);
        }

    }
}
