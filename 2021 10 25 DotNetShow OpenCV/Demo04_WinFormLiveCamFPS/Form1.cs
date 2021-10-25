using System;
using System.Threading;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace Demo04_WinForm
{
    public partial class Form1 : Form
    {
        private bool _run = false;
        private bool _canny = false;
        private VideoCapture _capture;
        private Mat _image;
        private Thread _cameraThread;
        private bool _fps = false;

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

        private void btnCanny_Click(object sender, EventArgs e)
        {
            _canny = !_canny;
        }

        private void buttonFPS_Click(object sender, EventArgs e)
        {
            _fps = !_fps;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _capture = new VideoCapture(0);
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
                var newImage = imageRes.Clone();
                if (_canny)
                    Cv2.Canny(imageRes, newImage, 50, 200);

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
                var bmpEffect = BitmapConverter.ToBitmap(newImage);

                pictureBoxWebCam.Image = bmpWebCam;
                pictureBoxEffect.Image = bmpEffect;
            }
        }
    }
}    