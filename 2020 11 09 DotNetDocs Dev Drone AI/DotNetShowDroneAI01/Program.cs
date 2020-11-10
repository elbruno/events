using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DotNetShowDroneAI01
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            string droneIp = "192.168.10.1";
            int commandUdpPort = 8889;
            int stateUdpPort = 8890;

            var commandUdpClient = new UdpClient();
            var commandEndPoint = new IPEndPoint(IPAddress.Parse(droneIp), commandUdpPort);
            commandUdpClient.Connect(commandEndPoint);

            var datagram = Encoding.ASCII.GetBytes("command");
            commandUdpClient.Client.SendTimeout = 1000;
            commandUdpClient.Send(datagram, datagram.Length);

            var stateUdpClient = new UdpClient(stateUdpPort);
            var stateEndPoint = new IPEndPoint(IPAddress.Parse(droneIp), stateUdpPort);
            stateUdpClient.Connect(droneIp, stateUdpPort);

            for (int i = 0; i < 200; i++)
            {
                stateUdpClient.Client.ReceiveTimeout = 1000;
                byte[] bytes = stateUdpClient.Receive(ref stateEndPoint);
                var responseState = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
                Console.WriteLine(responseState);

                var list = responseState.Replace(';', ':').Split(':');
                var battery = list[21];
                Console.WriteLine($"Battery: {battery}");
            }

        }
    }
}
