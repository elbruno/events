using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace Demo11_WinFormGoogleNet
{
    public partial class FormGoogleNetDetector : Form
    {
        private bool _run = true;
        private bool _useGoogleNet = false;
        private VideoCapture _capture;
        private Mat _image;
        private Thread _cameraThread;
        private bool _fps = false;
        private Net _netGoogleNet;
        private string[] _classNames;

        const string ProtoTxt = @"models\bvlc_googlenet.prototxt";
        const string CaffeModel = @"models\bvlc_googlenet.caffemodel";
        const string SynsetWords = @"models\synset_words.txt";

        private delegate void SafeCallDelegate(string text);

        public FormGoogleNetDetector()
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

        private void btnGoogleNet_Click(object sender, EventArgs e)
        {
            _useGoogleNet = !_useGoogleNet;
        }

        private void buttonFPS_Click(object sender, EventArgs e)
        {
            _fps = !_fps;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _classNames = File.ReadAllLines(SynsetWords)
                .Select(line => line.Split(' ').Last())
                .ToArray();
            _netGoogleNet = CvDnn.ReadNetFromCaffe(ProtoTxt, CaffeModel);

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
                var imageDnn = new Mat();
                Cv2.Resize(_image, imageDnn, new Size(320, 240));
                if (_useGoogleNet)
                {
                    int frameHeight = imageDnn.Rows;
                    int frameWidth = imageDnn.Cols;

                    // Convert Mat to batch of images
                    using var inputBlob = CvDnn.BlobFromImage(imageDnn, 1, new Size(224, 224), new Scalar(104, 117, 123));
                    _netGoogleNet.SetInput(inputBlob, "data");
                    using var prob = _netGoogleNet.Forward("prob");

                    // find the best class
                    GetMaxClass(prob, out int classId, out double classProb);
                    var msg = @$"Best class: #{classId} '{_classNames[classId]}' - Probability: {classProb:P2}";

                    // display output
                    WriteTextSafe(msg);
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
                    Cv2.PutText(imageDnn, fpsInfo, new Point(10, 20), HersheyFonts.HersheyComplexSmall, 1, Scalar.White);
                }

                var bmpWebCam = BitmapConverter.ToBitmap(imageDnn);
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

        private static void GetMaxClass(Mat probBlob, out int classId, out double classProb)
        {
            // reshape the blob to 1x1000 matrix
            using (var probMat = probBlob.Reshape(1, 1))
            {
                Cv2.MinMaxLoc(probMat, out _, out classProb, out _, out var classNumber);
                classId = classNumber.X;
            }
        }
    }
}