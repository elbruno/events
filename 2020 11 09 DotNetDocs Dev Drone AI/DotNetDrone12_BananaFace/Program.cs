﻿using System;
using System.Text;
using OpenCvSharp;
using SimpleUdp;

namespace DotNetDrone12_BananaFace
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

            using var capture = new VideoCapture();
            capture.BufferSize = 1;
            capture.Open(videoUDP);

            var faceCascade = new CascadeClassifier();
            faceCascade.Load("haarcascade_frontalface_default.xml");
            var bananaCascade = new CascadeClassifier();
            bananaCascade.Load("banana_classifier.xml");
            var frameSize = new Size(640, 480);

            using var window = new Window("DotNet Docs - Drone Face and Banana Detector");
            var image = new Mat();
            var run = true;
            var frameIndex = 0;

            while (run)
            {
                capture.Read(image);
                if (image.Empty())
                    break;

                using var newFrame = new Mat();
                Cv2.Resize(image, newFrame, frameSize);

                using var gray = new Mat();
                Cv2.CvtColor(newFrame, gray, ColorConversionCodes.BGR2GRAY);
                var faces = faceCascade.DetectMultiScale(gray, 1.3, 5);
                foreach (var face in faces)
                {
                    Cv2.Rectangle(newFrame, face, Scalar.Red, 3);
                    Cv2.PutText(newFrame, "face", new Point(face.Left + 2, face.Top + face.Width + 20),
                        HersheyFonts.HersheyComplex, 1, Scalar.Red, 2);
                }

                var bananas = bananaCascade.DetectMultiScale(gray, 1.3, 5, minSize: new Size(250, 75));
                foreach (var banana in bananas)
                {
                    Cv2.Rectangle(newFrame, banana, Scalar.Yellow);
                    Cv2.PutText(newFrame, "banana", new Point(banana.Left + 2, banana.Top + banana.Width + 20),
                        HersheyFonts.HersheyComplex, 1, Scalar.Yellow, 2);
                }

                window.ShowImage(newFrame);

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
