using System;
using SimpleUdp;

namespace DotNetShowDroneAI02
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
            Console.WriteLine("Hello World!");


        }
    }
}
