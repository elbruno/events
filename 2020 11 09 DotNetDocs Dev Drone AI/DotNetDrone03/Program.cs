using System;
using System.Text;
using SimpleUdp;

namespace DotNetDrone03
{
    class Program
    {
        private const string DroneIp = "192.168.10.1";
        private const int CommandUdpPort = 8889;
        private const int StateUdpPort = 8890;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello Drone Start!");

            var udpCommands = new UdpEndpoint(DroneIp, CommandUdpPort);
            udpCommands.EndpointDetected += UdpCommands_EndpointDetected;
            udpCommands.DatagramReceived += UdpCommands_DatagramReceived;
            udpCommands.MaxDatagramSize = 1024;
            udpCommands.Send("command");
            udpCommands.Receive();

            var udpStates = new UdpEndpoint(DroneIp, StateUdpPort);
            udpStates.EndpointDetected += UdpStates_EndpointDetected;
            udpStates.DatagramReceived += UdpStates_DatagramReceived;
            udpStates.MaxDatagramSize = 256;
            udpStates.Receive();

            for (int i = 0; i < 200; i++)
            {
                udpStates.Receive();
            }

            Console.WriteLine("Press any key to stop");
            Console.ReadKey();
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

        private static void UdpStates_EndpointDetected(object sender, EndpointMetadata e)
        {
            Console.WriteLine("Endpoint detected: " + e.Ip + ":" + e.Port);
        }

        private static void UdpCommands_EndpointDetected(object sender, EndpointMetadata e)
        {
            Console.WriteLine("Endpoint detected: " + e.Ip + ":" + e.Port);
        }
    }
}
