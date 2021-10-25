using OpenCvSharp;
using OpenCvSharp.Dnn;

namespace Labs_Yolo3
{
    /// <summary>
    /// OpenCvSharp V4 with YOLO v3
    /// Thank @shimat and Joseph Redmon
    ///
    /// OpenCvSharp
    /// https://github.com/shimat/opencvsharp/
    ///
    /// YOLO
    /// https://pjreddie.com/darknet/yolo/
    /// </summary>
    class Program
    {
        //YOLOv3
        //https://github.com/pjreddie/darknet/blob/master/cfg/yolov3.cfg
        private const string cfg = @"yolov3\yolov3.cfg";

        //https://pjreddie.com/media/files/yolov3.weights
        private const string weight = @"yolov3\yolov3.weights";

        //https://github.com/pjreddie/darknet/blob/master/data/coco.names
        private const string names = @"yolov3\coco.names";

        private static readonly Scalar[] Colors = Enumerable.Repeat(false, 80).Select(x => Scalar.RandomColor()).ToArray();
        private static readonly string[] Labels = File.ReadAllLines(names).ToArray();

        static void Main()
        {
            const float threshold = 0.5f;       //for confidence 
            const float nmsThreshold = 0.3f;    //threshold for nms

            bool yoloV3Enabled = false;

            var net = CvDnn.ReadNetFromDarknet(cfg, weight);
            net.SetPreferableBackend(Backend.OPENCV);
            net.SetPreferableTarget(0);

            using var capture = new VideoCapture(1);
            using var window = new Window("El Bruno - YoloV3 WebCam");
            var image = new Mat();
            var newSize = new Size(640, 480);

            var run = true;
            while (run)
            {
                capture.Read(image);
                if (image.Empty())
                    break;
                Cv2.Resize(image, image, newSize);

                if (yoloV3Enabled)
                {
                    var blob = CvDnn.BlobFromImage(image, 1.0 / 255, new Size(416, 416), new Scalar(), true, false);
                    net.SetInput(blob);
                    var outNames = net.GetUnconnectedOutLayersNames();
                    var outs = outNames.Select(_ => new Mat()).ToArray();
                    net.Forward(outs, outNames);
                    GetResult(outs, image, threshold, nmsThreshold);
                }

                window.ShowImage(image);

                switch ((char)Cv2.WaitKey(100))
                {
                    case (char)27: // ESC
                        run = false;
                        break;
                    case 'y':
                        yoloV3Enabled = !yoloV3Enabled;
                        break;
                }
            }
        }

        /// <summary>
        /// Get result form all output
        /// </summary>
        /// <param name="output"></param>
        /// <param name="image"></param>
        /// <param name="threshold"></param>
        /// <param name="nmsThreshold">threshold for nms</param>
        /// <param name="nms">Enable Non-maximum suppression or not</param>
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

        /// <summary>
        /// Draw result to image
        /// </summary>
        /// <param name="image"></param>
        /// <param name="classes"></param>
        /// <param name="confidence"></param>
        /// <param name="probability"></param>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private static void Draw(Mat image, int classes, float confidence, float probability, double centerX, double centerY, double width, double height)
        {
            //label formating
            var label = $"{Labels[classes]} {probability * 100:0.00}%";
            Console.WriteLine($"confidence {confidence * 100:0.00}% {label}");
            var x1 = (centerX - width / 2) < 0 ? 0 : centerX - width / 2; //avoid left side over edge
            //draw result
            image.Rectangle(new Point(x1, centerY - height / 2), new Point(centerX + width / 2, centerY + height / 2), Colors[classes], 2);
            var textSize = Cv2.GetTextSize(label, HersheyFonts.HersheyTriplex, 0.5, 1, out var baseline);
            Cv2.Rectangle(image, new Rect(new Point(x1, centerY - height / 2 - textSize.Height - baseline),
                new Size(textSize.Width, textSize.Height + baseline)), Colors[classes], Cv2.FILLED);
            var textColor = Cv2.Mean(Colors[classes]).Val0 < 70 ? Scalar.White : Scalar.Black;
            Cv2.PutText(image, label, new Point(x1, centerY - height / 2 - baseline), HersheyFonts.HersheyTriplex, 0.5, textColor);
        }
    }
}
