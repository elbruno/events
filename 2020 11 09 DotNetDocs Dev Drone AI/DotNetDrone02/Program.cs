using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DotNetDrone02
{
    class Program
    {
        private const string UdpIp = "192.168.10.1";
        private const int CommandUdpPort = 8889;
        private const int StateUdpPort = 8890;
        static UdpClient commandUdpClient;
        static IPEndPoint commandEndPoint;
        static UdpClient stateUdpClient;
        static IPEndPoint stateEndPoint;

        static void Main(string[] args)
        {
            Console.WriteLine("Start Hello Drone");
            try
            {
                var response = SendMessage("command");
                Console.WriteLine($"command sdk mode: {response}");
                response = SendMessage("command?");
                Console.WriteLine($"command? sdk mode: {response}");
                Task.Delay(1000).Wait();

                Console.WriteLine($"start read states");
                for (var i = 0; i < 200; i++)
                {
                    SendMessage("battery?");
                    var states = ReadStates();
                    Console.WriteLine($"{i} - {states}");
                    Task.Delay(500).Wait();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                CloseConnections();
            }
            Console.WriteLine("End Hello Drone");
        }

        private static void CloseConnections()
        {
            commandUdpClient?.Dispose();
            stateUdpClient?.Dispose();
        }

        public static string ReadStates()
        {
            stateUdpClient ??= new UdpClient(StateUdpPort);
            stateEndPoint ??= new IPEndPoint(IPAddress.Parse(UdpIp), StateUdpPort);
            if (stateUdpClient.Client.Connected == false)
                stateUdpClient.Connect(UdpIp, StateUdpPort);

            stateUdpClient.Client.ReceiveTimeout = 2500;
            stateUdpClient.Client.ReceiveBufferSize = 256;
            byte[] b1 = new byte[256];
            var bytes = stateUdpClient.Client.Receive(b1, SocketFlags.Partial);
            var responseState = Encoding.ASCII.GetString(b1, 0, b1.Length);
            return responseState;

            //stateUdpClient.Client.ReceiveTimeout = 2500;
            //stateUdpClient.Client.ReceiveBufferSize = 256;

            //var bytes = stateUdpClient.Receive(ref stateEndPoint);
            //var responseState = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            //return responseState;
        }

        private static string SendMessage(string command)
        {
            commandUdpClient ??= new UdpClient();
            commandEndPoint ??= new IPEndPoint(IPAddress.Parse(UdpIp), CommandUdpPort);
            if (commandUdpClient.Client.Connected == false)
                commandUdpClient.Connect(commandEndPoint);
            var datagram = Encoding.ASCII.GetBytes(command);
            commandUdpClient.Send(datagram, datagram.Length);

            commandUdpClient.Client.ReceiveTimeout = 2500;
            commandUdpClient.Client.ReceiveBufferSize = 1024;
            byte[] b1 = new byte[1024];
            var bytes = commandUdpClient.Client.Receive(b1, SocketFlags.Partial);
            var responseState = Encoding.ASCII.GetString(b1, 0, b1.Length);
            return responseState;

            //commandUdpClient.Client.ReceiveTimeout = 2500;
            //commandUdpClient.Client.ReceiveBufferSize = 1024;
            //var bytes = commandUdpClient.Receive(ref commandEndPoint);
            //var responseState = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            //return responseState;
        }
    }
}
