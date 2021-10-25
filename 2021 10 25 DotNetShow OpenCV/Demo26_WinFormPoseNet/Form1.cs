using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace Demo12_WinFormPoseNet
{
    public partial class Form1 : Form
    {
        private bool _run = true;
        private bool _detectPose = false;
        private bool _showJoints = false;
        private bool _fps = true;
        private VideoCapture _capture;
        private Mat _image;
        private Thread _cameraThread;

        const string PoseCaffeModel = @"models\pose\mpi\pose_iter_160000.caffemodel";
        const string PoseProtoTxt = @"models\pose\mpi\pose_deploy_linevec_faster_4_stages.prototxt";
        const int nPoints = 15;
        const double thresh = 0.1;

        int[][] posePairs =
        {
            new[] {0, 1}, new[] {1, 2}, new[] {2, 3},
            new[] {3, 4}, new[] {1, 5}, new[] {5, 6},
            new[] {6, 7}, new[] {1, 14}, new[] {14, 8}, new[] {8, 9},
            new[] {9, 10}, new[] {14, 11}, new[] {11, 12}, new[] {12, 13},
        };
        private Net _netPose;
        private delegate void SafeCallDelegate(string text);

        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
            Closed += Form1_Closed;
        }

        private void Form1_Closed(object sender, EventArgs e)
        {
            _cameraThread.Interrupt();
            _capture.Release();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            _run = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _run = false;
        }

        private void btnDetectPose_Click(object sender, EventArgs e)
        {
            _detectPose = !_detectPose;
        }

        private void buttonFPS_Click(object sender, EventArgs e)
        {
            _fps = !_fps;
        }

        private void btnShowJoints_Click(object sender, EventArgs e)
        {
            _showJoints = !_showJoints;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _netPose = CvDnn.ReadNetFromCaffe(PoseProtoTxt, PoseCaffeModel);
            _netPose.SetPreferableBackend(Backend.OPENCV);

            _capture = new VideoCapture(1);
            _image = new Mat();
            _cameraThread = new Thread(new ThreadStart(CaptureCameraCallback));
            _cameraThread.Start();
        }

private void CaptureCameraCallback()
{
    while (true)
    {
        if (!_run) continue;
        var startTime = DateTime.Now;

        _capture.Read(_image);
        if (_image.Empty()) return;
        var imageRes = new Mat();
        Cv2.Resize(_image, imageRes, new Size(320, 240));
        if (_detectPose)
        {

            var frameWidth = imageRes.Cols;
            var frameHeight = imageRes.Rows;

            const int inWidth = 368;
            const int inHeight = 368;

            // Convert Mat to batch of images
            using var inpBlob = CvDnn.BlobFromImage(imageRes, 1.0 / 255, new Size(inWidth, inHeight), new Scalar(0, 0, 0), false, false);

            _netPose.SetInput(inpBlob);

            using var output = _netPose.Forward();
            var H = output.Size(2);
            var W = output.Size(3);

            var points = new List<Point>();

            for (var n = 0; n < nPoints; n++)
            {
                // Probability map of corresponding body's part.
                using var probMap = new Mat(H, W, MatType.CV_32F, output.Ptr(0, n));
                var p = new Point2f(-1, -1);

                Cv2.MinMaxLoc(probMap, out _, out var maxVal, out _, out var maxLoc);

                var x = (frameWidth * maxLoc.X) / W;
                var y = (frameHeight * maxLoc.Y) / H;

                if (maxVal > thresh)
                {
                    p = maxLoc;
                    p.X *= (float)frameWidth / W;
                    p.Y *= (float)frameHeight / H;

                    Cv2.Circle(imageRes, (int)p.X, (int)p.Y, 8, Scalar.Azure, -1);
                    //Cv2.PutText(imageRes, Cv2.Format(n), new Point((int)p.X, (int)p.Y), HersheyFonts.HersheyComplex, 1, new Scalar(0, 0, 255), 1);
                }

                points.Add((Point)p);
            }

            WriteTextSafe(@$"Joints {nPoints} found");

            var nPairs = 14; //(POSE_PAIRS).Length / POSE_PAIRS[0].Length;

            for (var n = 0; n < nPairs; n++)
            {
                // lookup 2 connected body/hand parts
                var partA = points[posePairs[n][0]];
                var partB = points[posePairs[n][1]];
                if (partA.X <= 0 || partA.Y <= 0 || partB.X <= 0 || partB.Y <= 0)
                    continue;
                Cv2.Line(imageRes, partA, partB, new Scalar(0, 255, 255), 8);
                Cv2.Circle(imageRes, partA.X, partA.Y, 8, new Scalar(0, 0, 255), -1);
                Cv2.Circle(imageRes, partB.X, partB.Y, 8, new Scalar(0, 0, 255), -1);
            }

            // display output
            //WriteTextSafe(msg);
        }

        if (_fps)
        {
            var diff = DateTime.Now - startTime;
            var fpsInfo = $"FPS: Nan";
            if (diff.Milliseconds > 0)
            {
                var fpsVal = 1.0 / diff.Milliseconds * 1000;
                fpsInfo = $"FPS: {fpsVal:00}";
            }
            Cv2.PutText(imageRes, fpsInfo, new Point(10, 20), HersheyFonts.HersheyComplexSmall, 1, Scalar.White);
        }

        var bmpWebCam = BitmapConverter.ToBitmap(imageRes);
        pictureBoxWebCam.Image = bmpWebCam;
    }
        }

        private void WriteTextSafe(string text)
        {
            if (lblOutputAnalysis.InvokeRequired)
            {
                var d = new SafeCallDelegate(WriteTextSafe);
                lblOutputAnalysis.Invoke(d, new object[] { text });
            }
            else
            {
                lblOutputAnalysis.Text = text;
            }
        }
    }
}