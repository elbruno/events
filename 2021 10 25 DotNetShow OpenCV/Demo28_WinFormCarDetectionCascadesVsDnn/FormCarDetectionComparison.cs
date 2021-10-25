using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using OpenCvSharp;
using OpenCvSharp.Dnn;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace Demo14_WinFormCarDetectionCascadesVsDnn
{
    public partial class FormCarDetectionComparison : Form
    {
        private bool _run = false;

        private VideoCapture _capture;
        private Mat _image;
        private Thread _cameraThread;
        private const int LineThickness = 2;

        private CascadeClassifier _carCascade;
        private Net _netMobileNetSsd;
        const string _netMobileNetSsdProtoTxt = @"models\MobileNetSSD_deploy.prototxt.txt";
        const string _netMobileNetSsdCaffeModel = @"models\MobileNetSSD_deploy.caffemodel";
        private readonly List<string> _classesMobileNetSSD = new List<string> { "background", "aeroplane", "bicycle", "bird", "boat", "bottle", "bus", "car", "cat", "chair", "cow", "diningtable", "dog", "horse", "motorbike", "person", "pottedplant", "sheep", "sofa", "train", "tvmonitor" };


        // YOLO V3
        //https://github.com/pjreddie/darknet/blob/master/cfg/yolov3.cfg
        //https://pjreddie.com/media/files/yolov3.weights
        //https://github.com/pjreddie/darknet/blob/master/data/coco.names
        private const string _yoloV3Cfg = @"yolov3\yolov3.cfg";
        private const string _yoloV3Weight = @"yolov3\yolov3.weights";
        private const string _yoloV3Names = @"yolov3\coco.names";
        private static readonly Scalar[] _yoloV3Colors = Enumerable.Repeat(false, 80).Select(x => Scalar.RandomColor()).ToArray();
        private static readonly string[] _yoloV3Labels = File.ReadAllLines(_yoloV3Names).ToArray();
        private Net _netYoloV3;

        //private string _videoFile = "Traffic Cam Time lapse Building a stack train.mp4";
        private string _videoFile = "4K camera example for Traffic Monitoring (Road).mp4";

        public FormCarDetectionComparison()
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
            // Cascade 
            _carCascade = new CascadeClassifier();
            _carCascade.Load("cars.xml");

            // mobile net ssd
            _netMobileNetSsd = CvDnn.ReadNetFromCaffe(_netMobileNetSsdProtoTxt, _netMobileNetSsdCaffeModel);

            // Yolo V3
            _netYoloV3 = CvDnn.ReadNetFromDarknet(_yoloV3Cfg, _yoloV3Weight);
            _netYoloV3.SetPreferableBackend(Backend.OPENCV);
            _netYoloV3.SetPreferableTarget(Target.CPU);

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
                var imageOriginal = new Mat();
                Cv2.Resize(_image, imageOriginal, new Size(320, 240));
                var imageCascade = imageOriginal.Clone();
                var imageMobileNetSsd = imageOriginal.Clone();
                var imageYolo = imageOriginal.Clone();
                if (checkBoxCascade.Checked)
                {
                    // HAAR CAscade
                    using var gray = new Mat();
                    Cv2.CvtColor(imageCascade, gray, ColorConversionCodes.BGR2GRAY);

                    var cars = _carCascade.DetectMultiScale(gray, 1.3, 5);
                    foreach (var car in cars)
                    {
                        Cv2.Rectangle(imageCascade, car, Scalar.Red, LineThickness);
                        Cv2.PutText(imageCascade, "Cascade", new Point(car.Left + 2, car.Top + car.Width + 20),
                            HersheyFonts.HersheyComplexSmall, 1, Scalar.Red, 2);
                    }
                }

                if (checkBoxMobileNetSsd.Checked)
                {
                    // MobileNet SSD
                    int frameHeight = imageMobileNetSsd.Rows;
                    int frameWidth = imageMobileNetSsd.Cols;

                    var imageDnnBlob = new Mat();
                    Cv2.Resize(_image, imageDnnBlob, new Size(300, 300));

                    using var blobMobileNetSsd = CvDnn.BlobFromImage(imageDnnBlob, 0.007843, new Size(300, 300),
                        new Scalar(104, 117, 123), false, false);

                    _netMobileNetSsd.SetInput(blobMobileNetSsd, "data");

                    using var detection = _netMobileNetSsd.Forward("detection_out");
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

                            Cv2.Rectangle(imageMobileNetSsd, new Point(x1, y1), new Point(x2, y2), Scalar.Green,
                                LineThickness);
                        }
                    }
                }

                if (checkBoxYoloV3.Checked)
                {
                    var blob = CvDnn.BlobFromImage(imageYolo, 1.0 / 255, new Size(416, 416), new Scalar(), true, false);
                    _netYoloV3.SetInput(blob);
                    var outNames = _netYoloV3.GetUnconnectedOutLayersNames();
                    var outs = outNames.Select(_ => new Mat()).ToArray();
                    _netYoloV3.Forward(outs, outNames);
                    const float threshold = 0.5f;       //for confidence 
                    const float nmsThreshold = 0.3f;    //threshold for nms
                    GetResult(outs, imageYolo, threshold, nmsThreshold);
                }

                if (checkBoxFPS.Checked)
                {
                    var diff = DateTime.Now - startTime;
                    var fpsInfo = $"FPS: Nan";
                    if (diff.Milliseconds > 0)
                    {
                        var fpsVal = 1.0 / diff.Milliseconds * 1000;
                        fpsInfo = $"FPS: {fpsVal:00}";
                    }
                    Cv2.PutText(imageCascade, fpsInfo, new Point(10, 20), HersheyFonts.HersheyComplexSmall, 1, Scalar.White);
                }

                var bmpOriginal = BitmapConverter.ToBitmap(imageOriginal);
                var bmpCascade = BitmapConverter.ToBitmap(imageCascade);
                var bmpMobileNetSsd = BitmapConverter.ToBitmap(imageMobileNetSsd);
                var bmpYolo = BitmapConverter.ToBitmap(imageYolo);

                pictureBoxOriginal.Image = bmpOriginal;
                pictureBoxCascade.Image = bmpCascade;
                pictureBoxMobileNetSsd.Image = bmpMobileNetSsd;
                pictureBoxYoloV3.Image = bmpYolo;
            }
        }

        private static void GetResult(IEnumerable<Mat> output, Mat image, float threshold, float nmsThreshold, bool nms = true)
        {
            //for nms
            var classIds = new List<int>();
            var confidences = new List<float>();
            var probabilities = new List<float>();
            var boxes = new List<Rect2d>();

            var w = image.Width;
            var h = image.Height;
            /*
             YOLO3 COCO trainval output
             0 1 : center                    2 3 : w/h
             4 : confidence                  5 ~ 84 : class probability 
            */
            const int prefix = 5;   //skip 0~4

            foreach (var prob in output)
            {
                for (var i = 0; i < prob.Rows; i++)
                {
                    var confidence = prob.At<float>(i, 4);
                    if (confidence > threshold)
                    {
                        //get classes probability
                        //Cv2.MinMaxLoc(prob.Row[i].ColRange(prefix, prob.Cols), out _, out Point max);
                        Cv2.MinMaxLoc(prob[i, i + 1, 5, prob.Cols], out _, out Point max);
                        var classes = max.X;
                        var probability = prob.At<float>(i, classes + prefix);

                        if (probability > threshold) //more accuracy, you can cancel it
                        {
                            //get center and width/height
                            var centerX = prob.At<float>(i, 0) * w;
                            var centerY = prob.At<float>(i, 1) * h;
                            var width = prob.At<float>(i, 2) * w;
                            var height = prob.At<float>(i, 3) * h;

                            if (!nms)
                            {
                                // draw result (if don't use NMSBoxes)
                                Draw(image, classes, confidence, probability, centerX, centerY, width, height);
                                continue;
                            }

                            //put data to list for NMSBoxes
                            classIds.Add(classes);
                            confidences.Add(confidence);
                            probabilities.Add(probability);
                            boxes.Add(new Rect2d(centerX, centerY, width, height));
                        }
                    }
                }
            }

            if (!nms) return;

            //using non-maximum suppression to reduce overlapping low confidence box
            CvDnn.NMSBoxes(boxes, confidences, threshold, nmsThreshold, out int[] indices);

            Console.WriteLine($"NMSBoxes drop {confidences.Count - indices.Length} overlapping result.");

            foreach (var i in indices)
            {
                var box = boxes[i];
                Draw(image, classIds[i], confidences[i], probabilities[i], box.X, box.Y, box.Width, box.Height);
            }

        }

        private static void Draw(Mat image, int classes, float confidence, float probability, double centerX, double centerY, double width, double height)
        {
            //label formating
            var label = $"{_yoloV3Labels[classes]} {probability * 100:0.00}%";
            Console.WriteLine($"confidence {confidence * 100:0.00}% {label}");
            var x1 = (centerX - width / 2) < 0 ? 0 : centerX - width / 2; //avoid left side over edge
            //draw result
            image.Rectangle(new Point(x1, centerY - height / 2), new Point(centerX + width / 2, centerY + height / 2), _yoloV3Colors[classes], 2);
            var textSize = Cv2.GetTextSize(label, HersheyFonts.HersheyTriplex, 0.5, 1, out var baseline);
            Cv2.Rectangle(image, new Rect(new Point(x1, centerY - height / 2 - textSize.Height - baseline),
                new Size(textSize.Width, textSize.Height + baseline)), _yoloV3Colors[classes], Cv2.FILLED);
            var textColor = Cv2.Mean(_yoloV3Colors[classes]).Val0 < 70 ? Scalar.White : Scalar.Black;
            Cv2.PutText(image, label, new Point(x1, centerY - height / 2 - baseline), HersheyFonts.HersheyTriplex, 0.5, textColor);
        }
    }
}