using System;
using System.Collections.Generic;
using System.Text;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using SimpleUdp;

namespace DotNetDrone14_HandPose
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

            const string model = "pose_iter_102000.caffemodel";
            const string modelTxt = "pose_deploy.prototxt";
            const int nPoints = 22;
            const double thresh = 0.01;

            int[][] posePairs =
            {
                new[] {0, 1}, new[] {1, 2}, new[] {2, 3}, new[] {3, 4}, //thumb
                new[] {0, 5}, new[] {5, 6}, new[] {6, 7}, new[] {7, 8}, //index
                new[] {0, 9}, new[] {9, 10}, new[] {10, 11}, new[] {11, 12}, //middle
                new[] {0, 13}, new[] {13, 14}, new[] {14, 15}, new[] {15, 16}, //ring
                new[] {0, 17}, new[] {17, 18}, new[] {18, 19}, new[] {19, 20}, //small
            };
            var image = new Mat();
            var showHandPose = true;
            var run = true;

            using var net = CvDnn.ReadNetFromCaffe(modelTxt, model);
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

                if (showHandPose)
                {
                    using var gray = new Mat();
                    Cv2.CvtColor(frame, gray, ColorConversionCodes.BGR2GRAY);

                    var faces = faceCascade.DetectMultiScale(gray, 1.3, 5);
                    foreach (var face in faces)
                    {
                        Cv2.Rectangle(frame, face, Scalar.Red);
                        Cv2.PutText(frame, "face", new Point(face.Left + 2, face.Top + face.Width + 20),
                            HersheyFonts.HersheyComplex, 1, Scalar.Red, 2);
                    }

                    using var frameCopy = frame.Clone();
                    int frameWidth = frame.Cols;
                    int frameHeight = frame.Rows;

                    float aspectRatio = frameWidth / (float)frameHeight;
                    int inHeight = 368;
                    int inWidth = ((int)(aspectRatio * inHeight) * 8) / 8;

                    using var inpBlob = CvDnn.BlobFromImage(frame, 1.0 / 255, new Size(inWidth, inHeight),
                        new Scalar(0, 0, 0), false, false);

                    net.SetInput(inpBlob);

                    using var output = net.Forward();
                    int H = output.Size(2);
                    int W = output.Size(3);

                    var points = new List<Point>();

                    for (int n = 0; n < nPoints; n++)
                    {
                        // Probability map of corresponding body's part.
                        using var probMap = new Mat(H, W, MatType.CV_32F, output.Ptr(0, n));
                        Cv2.Resize(probMap, probMap, new Size(frameWidth, frameHeight));
                        Cv2.MinMaxLoc(probMap, out _, out var maxVal, out _, out var maxLoc);

                        if (maxVal > thresh)
                        {
                            Cv2.Circle(frameCopy, maxLoc.X, maxLoc.Y, 8, new Scalar(0, 255, 255), -1,
                                LineTypes.Link8);
                            Cv2.PutText(frameCopy, Cv2.Format(n), new OpenCvSharp.Point(maxLoc.X, maxLoc.Y),
                                HersheyFonts.HersheyComplex, 1, new Scalar(0, 0, 255), 2, LineTypes.AntiAlias);
                        }

                        points.Add(maxLoc);
                    }

                    int nPairs = 20; //(POSE_PAIRS).Length / POSE_PAIRS[0].Length;

                    for (int n = 0; n < nPairs; n++)
                    {
                        // lookup 2 connected body/hand parts
                        Point partA = points[posePairs[n][0]];
                        Point partB = points[posePairs[n][1]];

                        if (partA.X <= 0 || partA.Y <= 0 || partB.X <= 0 || partB.Y <= 0)
                            continue;

                        Cv2.Line(frame, partA, partB, new Scalar(0, 255, 255), 8);
                        Cv2.Circle(frame, partA.X, partA.Y, 8, new Scalar(0, 0, 255), -1);
                        Cv2.Circle(frame, partB.X, partB.Y, 8, new Scalar(0, 0, 255), -1);
                    }
                }

                var diff = DateTime.Now - startTime;
                var fpsInfo = $"FPS: Nan";
                if (diff.Milliseconds > 0)
                {
                    var fpsVal = 1.0 / diff.Milliseconds * 1000;
                    fpsInfo = $"FPS: {fpsVal:00}";
                }
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
                    case 'h':
                        showHandPose = !showHandPose;
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
