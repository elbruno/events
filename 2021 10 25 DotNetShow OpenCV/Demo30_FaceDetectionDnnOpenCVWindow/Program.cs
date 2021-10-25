using System;
using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace Demo07_FaceDetectionDnn
{
    class Program
    {
        static void Main(string[] args)
        {
            // download model and prototxt from https://github.com/spmallick/learnopencv/tree/master/FaceDetectionComparison/models
            const string configFile = "deploy.prototxt";
            const string faceModel = "res10_300x300_ssd_iter_140000_fp16.caffemodel";
            using var faceNet = CvDnn.ReadNetFromCaffe(configFile, faceModel);

            var faceCascade = new CascadeClassifier();
            faceCascade.Load("haarcascade_frontalface_default.xml");

            var image = new Mat();
            var switchMode = true;
            var run = true;

            using var capture = new VideoCapture(1);
            {
                using var window = new Window("El Bruno - Face Detector");
                while (run)
                {
                    var startTime = DateTime.Now;

                    capture.Read(image);
                    if (image.Empty())
                        break;

                    var newSize = new Size(640, 480);
                    using var frame = new Mat();
                    Cv2.Resize(image, frame, newSize);

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


                    window.Image = frame;
                    switch ((char)Cv2.WaitKey(100))
                    {
                        case (char)27: // ESC
                            run = false;
                            break;
                        case 'f':
                            switchMode = !switchMode;
                            break;
                    }
                }
            }
        }
    }
}