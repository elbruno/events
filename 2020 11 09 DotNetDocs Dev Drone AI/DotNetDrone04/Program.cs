using System;
using System.Text;
using System.Threading.Tasks;
using SimpleUdp;

namespace DotNetDrone04
{
    class Program
    {
        private static UdpEndpoint udpCommands;
        private static UdpEndpoint udpStates;
        private const string DroneIp = "192.168.10.1";
        private const int CommandUdpPort = 8889;
        private const int StateUdpPort = 8890;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello Drone Start!");

            udpCommands = new UdpEndpoint(DroneIp, CommandUdpPort);
            udpCommands.DatagramReceived += UdpCommands_DatagramReceived;
            udpCommands.MaxDatagramSize = 1024;
            udpCommands.Send("command");
            
            udpStates = new UdpEndpoint(DroneIp, StateUdpPort);
            udpStates.DatagramReceived += UdpStates_DatagramReceived;
            udpStates.MaxDatagramSize = 256;
            
            Task.Run(GetStates);

            Console.WriteLine("Q - Exit app");
            var exit = false;
            do
            {
                var key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.T:
                        udpCommands.Send("takeoff");
                        break;
                    case ConsoleKey.L:
                        udpCommands.Send("land");
                        break;
                    case ConsoleKey.Q:
                        exit = true;
                        break;
                }
            } while (!exit);
        }

        private static void GetStates()
        {
            while (true)
                udpStates.Receive();
        }

        private static void UdpCommands_DatagramReceived(object sender, Datagram e)
        {
            var responseState = Encoding.ASCII.GetString(e.Data, 0, e.Data.Length);
            Console.WriteLine("Command received: " + responseState);
        }

        private static void UdpStates_DatagramReceived(object sender, Datagram e)
        {
            var responseState = Encoding.ASCII.GetString(e.Data, 0, e.Data.Length);
            Console.WriteLine("state received: " + responseState);
        }
    }
}
