using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DotNetDrone01
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1st working demo
            Console.WriteLine("Hello Drone");

            string droneIp = "192.168.10.1";
            int commandUdpPort = 8889;
            int stateUdpPort = 8890;

            var commandUdpClient = new UdpClient();
            var commandEndPoint = new IPEndPoint(IPAddress.Parse(droneIp), commandUdpPort);
            commandUdpClient.Connect(commandEndPoint);

            var datagram = Encoding.ASCII.GetBytes("command");
            commandUdpClient.Send(datagram, datagram.Length);

            var stateUdpClient = new UdpClient(stateUdpPort);
            var stateEndPoint = new IPEndPoint(IPAddress.Parse(droneIp), stateUdpPort);
            stateUdpClient.Connect(droneIp, stateUdpPort);

            byte[] bytes = stateUdpClient.Receive(ref stateEndPoint);
            var responseState = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            Console.WriteLine(responseState);

            var list = responseState.Replace(';', ':').Split(':');
            var battery = list[21];
            Console.WriteLine($"Battery: {battery}");
        }
    }
}
