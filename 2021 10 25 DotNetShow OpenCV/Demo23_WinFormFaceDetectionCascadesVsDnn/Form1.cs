using System;
using System.Threading;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace Demo09_WinFormFaceDetectionCascadesVsDnn
{
    public partial class Form1 : Form
    {
        private bool _run = false;
        private bool _doFaceDetection = false;
        private VideoCapture _capture;
        private Mat _image;
        private Thread _cameraThread;
        private bool _fps = false;
        private Net _faceNet;
        private CascadeClassifier _faceCascade;
        private const int LineThickness = 2;

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

        private void btnFDDNN_Click(object sender, EventArgs e)
        {
            _doFaceDetection = !_doFaceDetection;
        }

        private void buttonFPS_Click(object sender, EventArgs e)
        {
            _fps = !_fps;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // download model and prototxt from https://github.com/spmallick/learnopencv/tree/master/FaceDetectionComparison/models
            const string configFile = "deploy.prototxt";
            const string faceModel = "res10_300x300_ssd_iter_140000_fp16.caffemodel";
            _faceNet = CvDnn.ReadNetFromCaffe(configFile, faceModel);

            _faceCascade = new CascadeClassifier();
            _faceCascade.Load("haarcascade_frontalface_default.xml");

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
                var newImage = imageRes.Clone();
                if (_doFaceDetection)
                {
                    // HAAR CAscade
                    using var gray = new Mat();
                    Cv2.CvtColor(newImage, gray, ColorConversionCodes.BGR2GRAY);

                    var faces = _faceCascade.DetectMultiScale(gray, 1.3, 5);
                    foreach (var face in faces)
                    {
                        Cv2.Rectangle(newImage, face, Scalar.Red, LineThickness);
                        Cv2.PutText(newImage, "Cascade", new Point(face.Left + 2, face.Top + face.Width + 20),
                            HersheyFonts.HersheyComplexSmall, 1, Scalar.Red, 2);
                    }


                    // DNN
                    int frameHeight = newImage.Rows;
                    int frameWidth = newImage.Cols;

                    using var blob = CvDnn.BlobFromImage(newImage, 1.0, new Size(300, 300),
                        new Scalar(104, 117, 123), false, false);
                    _faceNet.SetInput(blob, "data");

                    using var detection = _faceNet.Forward("detection_out");
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

                            Cv2.Rectangle(imageRes, new Point(x1, y1), new Point(x2, y2), Scalar.Green, LineThickness);
                            Cv2.PutText(imageRes, "DNN", new Point(x1 + 2, y2 + 20),
                                HersheyFonts.HersheyComplexSmall, 1, Scalar.Green, 2);
                        }
                    }

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
                var bmpEffect = BitmapConverter.ToBitmap(newImage);

                pictureBoxWebCam.Image = bmpWebCam;
                pictureBoxEffect.Image = bmpEffect;
            }
        }
    }
}