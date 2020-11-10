using System;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using SimpleUdp;

namespace DotNetDrone05
{
    class Program
    {
        private static UdpEndpoint udpCommands;
        private static UdpEndpoint udpStates;
        private static string battery = "";
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

            udpCommands.Send("streamon");
            string videoUDP = "udp://192.168.10.1:11111";

            //using var capture = new VideoCapture(videoUDP);
            using var capture = new VideoCapture();
            capture.Open(videoUDP, VideoCaptureAPIs.FFMPEG);
            capture.BufferSize = 1;
            capture.Set(VideoCaptureProperties.BufferSize, (double)1);
            //Task.Delay(1000).Wait();


            using var window = new Window("DotNet Docs - Drone");
            var image = new Mat();
            var run = true;
            var frameIndex = 0;

            while (run)
            {
                //capture.Read(image);
                //Console.WriteLine($"BackEnd Name: {capture.GetBackendName()}");
                //Console.WriteLine($"BufferSize: {capture.BufferSize}");
                //Console.WriteLine($"FourCC: {capture.FourCC}");
                //Console.WriteLine($"Mode: {capture.Mode}");
                //Console.WriteLine($"Settings: {capture.Settings}");
                //BackEnd Name: FFMPEG
                //BufferSize: 0
                //FourCC: h264
                //Mode: 0
                //Settings: 0
                //167 - FPS: 25 - Battery: 86

                if (capture.Grab())
                {
                    if (capture.Retrieve(image, 0))
                    {
                        if (image.Empty())
                            break;
                        window.ShowImage(image);
                    }
                }

                frameIndex++;

                // get battery
                udpStates.Receive();
                udpCommands.Send("battery?");
                Console.WriteLine($"{frameIndex} - FPS: {capture.Fps} -  Battery: {battery}");

                switch ((char)Cv2.WaitKey(100))
                {
                    case (char)27: // ESC
                        run = false;
                        break;
                    case 't':
                        udpCommands.Send("takeoff");
                        break;
                    case 'l':
                        udpCommands.Send("land");
                        break;
                }
            }

            capture.Release();
            udpCommands.Send("streamoff");
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
            //Console.WriteLine("state received: " + responseState);
            var list = responseState.Replace(';', ':').Split(':');
            battery = list[21];
        }
    }
}
