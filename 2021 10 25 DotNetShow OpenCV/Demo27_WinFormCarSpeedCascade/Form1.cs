using System;
using System.Threading;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Size = OpenCvSharp.Size;

namespace Demo13_WinFormVideoFromFile
{
    public partial class Form1 : Form
    {
        private bool _run = false;
        private VideoCapture _capture;
        private Mat _image;
        private Thread _cameraThread;
        private string _videoFile = "4K camera example for Traffic Monitoring (Road).mp4";

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
            _capture = new VideoCapture(_videoFile);
            _run = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _run = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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
         
                var bmpWebCam = BitmapConverter.ToBitmap(imageRes);
                pictureBoxWebCam.Image = bmpWebCam;
            }
        }
    }
}