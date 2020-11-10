using System;
using System.Collections.Generic;
using System.Text;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using SimpleUdp;

namespace DotNetDrone15_FaceDnn
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

            var image = new Mat();
            var switchMode = true;
            var run = true;

            // download model and prototxt from https://github.com/spmallick/learnopencv/tree/master/FaceDetectionComparison/models
            const string configFile = "deploy.prototxt";
            const string faceModel = "res10_300x300_ssd_iter_140000_fp16.caffemodel";
            using var faceNet = CvDnn.ReadNetFromCaffe(configFile, faceModel);

            var faceCascade = new CascadeClassifier();
            faceCascade.Load("haarcascade_frontalface_default.xml");

            using var window = new Window("DotNet Docs - Drone Face Detector");
            var frameIndex = 0;

            while (run)
            {
                var startTime = DateTime.Now;
                capture.Read(image);
                if (image.Empty())
                    break;

                var frame = image.Clone();

                if (switchMode)
                {
                    using var gray = new Mat();
                    Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);

                    var faces = faceCascade.DetectMultiScale(gray, 1.3, 5);
                    foreach (var face in faces)
                    {
                        Cv2.Rectangle(frame, face, Scalar.Red);
                        Cv2.PutText(frame, "Face Cascade", new Point(face.Left + 2, face.Top + face.Width + 20),
                            HersheyFonts.HersheyComplexSmall, 1, Scalar.Red, 2);
                    }
                }
                else
                {
                    int frameHeight = frame.Rows;
                    int frameWidth = frame.Cols;

                    using var blob = CvDnn.BlobFromImage(frame, 1.0, new Size(300, 300),
                        new Scalar(104, 117, 123), false, false);
                    faceNet.SetInput(blob, "data");

                    using var detection = faceNet.Forward("detection_out");
                    using var detectionMat = new Mat(detection.Size(2), detection.Size(3), MatType.CV_32F,
                        detection.Ptr(0));
                    for (int i = 0; i < detectionMat.Rows; i++)
                    {
                        float confidence = detectionMat.At<float>(i, 2);

                        if (confidence > 0.7)
                        {
                            int x1 = (int)(detectionMat.At<float>(i, 3) * frameWidth);
                            int y1 = (int)(detectionMat.At<float>(i, 4) * frameHeight);
                            int x2 = (int)(detectionMat.At<float>(i, 5) * frameWidth);
                            int y2 = (int)(detectionMat.At<float>(i, 6) * frameHeight);

                            Cv2.Rectangle(frame, new Point(x1, y1), new Point(x2, y2), Scalar.Green);
                            Cv2.PutText(frame, "Face Dnn", new Point(x1 + 2, y2 + 20),
                                HersheyFonts.HersheyComplexSmall, 1, Scalar.Green, 2);
                        }
                    }

                }

                var diff = DateTime.Now - startTime;
                var fpsInfo = $"FPS: Nan";
                if (diff.Milliseconds > 0)
                {
                    var fpsVal = 1.0 / diff.Milliseconds * 1000;
                    fpsInfo = $"FPS: {fpsVal:00}";
                }
                Cv2.PutText(frame, fpsInfo, new Point(10, 20), HersheyFonts.HersheyComplexSmall, 1, Scalar.White);

                window.ShowImage(frame);

                frameIndex++;

                // get battery
                udpStates.Receive();
                udpCommands.Send("battery?");
                Console.WriteLine($"{frameIndex} - FPS: {capture.Fps} - FPS Calc: {fpsInfo} -  Battery: {battery}");

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
                    case 'f':
                        switchMode = !switchMode;
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
