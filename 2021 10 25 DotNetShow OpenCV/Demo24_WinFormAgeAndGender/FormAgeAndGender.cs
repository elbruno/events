using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace Demo10_WinFormAgeAndGender
{
    public partial class FormAgeAndGender : Form
    {
        private bool _run = true;
        private bool _doFaceDetection = true;
        private bool _doAgeGender = false;

        private VideoCapture _capture;
        private Mat _image;
        private Thread _cameraThread;
        private bool _fps = false;
        private Net _faceNet;
        private Net _ageNet;
        private Net _genderNet;
        private const int LineThickness = 2;
        private const int Padding = 10;
        private readonly List<string> _genderList = new List<string> { "Male", "Female" };
        private readonly List<string> _ageList = new List<string> { "(0-2)", "(4-6)", "(8-12)", "(15-20)", "(25-32)", "(38-43)", "(48-53)", "(60-100)" };


        public FormAgeAndGender()
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

        private void btnAgeGender_Click(object sender, EventArgs e)
        {
            _doAgeGender = !_doAgeGender;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // # detect faces, age and gender using models from https://github.com/spmallick/learnopencv/tree/08e61fe80b8c0244cc4029ac11e44cd0fbb008c3/AgeGender
            const string faceProto = "models/deploy.prototxt";
            const string faceModel = "models/res10_300x300_ssd_iter_140000_fp16.caffemodel";
            const string ageProto = @"models/age_deploy.prototxt";
            const string ageModel = @"models/age_net.caffemodel";
            const string genderProto = @"models/gender_deploy.prototxt";
            const string genderModel = @"models/gender_net.caffemodel";
            _ageNet = CvDnn.ReadNetFromCaffe(ageProto, ageModel);
            _genderNet = CvDnn.ReadNetFromCaffe(genderProto, genderModel);
            _faceNet = CvDnn.ReadNetFromCaffe(faceProto, faceModel);

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

                if (_doFaceDetection) DetectFaces(newImage, imageRes);

                if (_fps) CalculateFps(startTime, newImage);

                var bmpWebCam = BitmapConverter.ToBitmap(imageRes);
                var bmpEffect = BitmapConverter.ToBitmap(newImage);

                pictureBoxWebCam.Image = bmpWebCam;
                pictureBoxEffect.Image = bmpEffect;
            }
        }

        private static void CalculateFps(DateTime startTime, Mat imageRes)
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

        private void DetectFaces(Mat newImage, Mat imageRes)
        {
            // DNN
            int frameHeight = newImage.Rows;
            int frameWidth = newImage.Cols;

            using var blob = CvDnn.BlobFromImage(newImage, 1.0, new Size(300, 300), new Scalar(104, 117, 123), false, false);
            _faceNet.SetInput(blob, "data");

            using var detection = _faceNet.Forward("detection_out");
            using var detectionMat = new Mat(detection.Size(2), detection.Size(3), MatType.CV_32F, detection.Ptr(0));
            for (int i = 0; i < detectionMat.Rows; i++)
            {
                float confidence = detectionMat.At<float>(i, 2);

                if (confidence > 0.7)
                {
                    int x1 = (int)(detectionMat.At<float>(i, 3) * frameWidth);
                    int y1 = (int)(detectionMat.At<float>(i, 4) * frameHeight);
                    int x2 = (int)(detectionMat.At<float>(i, 5) * frameWidth);
                    int y2 = (int)(detectionMat.At<float>(i, 6) * frameHeight);

                    Cv2.Rectangle(newImage, new Point(x1, y1), new Point(x2, y2), Scalar.Green, LineThickness);

                    if (_doAgeGender)
                        AnalyzeAgeAndGender(x1, y1, x2, y2, imageRes, newImage);
                }
            }
        }

        private void AnalyzeAgeAndGender(int x1, int y1, int x2, int y2, Mat imageRes, Mat newImage)
        {
            // get face frame
            var x = x1 - Padding;
            var y = y1 - Padding;
            var w = (x2 - x1) + Padding * 3;
            var h = (y2 - y1) + Padding * 3;
            Rect roiNew = new Rect(x, y, w, h);
            var face = imageRes[roi: roiNew];

            var meanValues = new Scalar(78.4263377603, 87.7689143744, 114.895847746);
            var blobGender = CvDnn.BlobFromImage(face, 1.0, new Size(227, 227), mean: meanValues,
                swapRB: false);
            _genderNet.SetInput(blobGender);
            var genderPreds = _genderNet.Forward();

            GetMaxClass(genderPreds, out int classId, out double classProbGender);
            var gender = _genderList[classId];

            _ageNet.SetInput(blobGender);
            var agePreds = _ageNet.Forward();
            GetMaxClass(agePreds, out int classIdAge, out double classProbAge);
            var age = _ageList[classIdAge];

            var label = $"{gender},{age}";
            Cv2.PutText(newImage, label, new Point(x1 - 10, y2 + 20), HersheyFonts.HersheyComplexSmall, 1, Scalar.Yellow, 1);
        }

        private void GetMaxClass(Mat probBlob, out int classId, out double classProb)
        {
            // reshape the blob to 1x1000 matrix
            using var probMat = probBlob.Reshape(1, 1);
            Cv2.MinMaxLoc(probMat, out _, out classProb, out _, out var classNumber);
            classId = classNumber.X;
            Debug.WriteLine($"X: {classNumber.X} - Y: {classNumber.Y} ");
        }
    }
}