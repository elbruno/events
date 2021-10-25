using OpenCvSharp;

namespace Demo05_BananaFace
{
    class Program
    {
        static void Main(string[] args)
        {

            var faceCascade = new CascadeClassifier();
            faceCascade.Load("haarcascade_frontalface_default.xml");

            var bananaCascade = new CascadeClassifier();
            bananaCascade.Load("banana_classifier.xml");

            using var capture = new VideoCapture(0);
            {
                var image = new Mat();
                var showFace = true;
                var run = true;

                while (run)
                {
                    capture.Read(image);
                    if (image.Empty())
                        break;

                    var newSize = new Size(640, 480);
                    using var smallFrame = new Mat();
                    Cv2.Resize(image, smallFrame, newSize);

                    if (showFace)
                    {
                        using var gray = new Mat();
                        Cv2.CvtColor(smallFrame, gray, ColorConversionCodes.BGR2GRAY);

                        var faces = faceCascade.DetectMultiScale(gray, 1.3, 5);
                        foreach (var face in faces)
                        {
                            Cv2.Rectangle(smallFrame, face, Scalar.Red);
                            Cv2.PutText(smallFrame, "face", new Point(face.Left + 2, face.Top + face.Width + 20),
                                HersheyFonts.HersheyComplex, 1, Scalar.Red, 2);
                        }

                        //var bananas = bananaCascade.DetectMultiScale(gray, 1.3, 5);
                        var bananas = bananaCascade.DetectMultiScale(gray, 1.3, 5, minSize: new Size(250, 75));
                        foreach (var banana in bananas)
                        {
                            Cv2.Rectangle(smallFrame, banana, Scalar.Yellow);
                            Cv2.PutText(smallFrame, "banana", new Point(banana.Left + 2, banana.Top + banana.Width + 20),
                                HersheyFonts.HersheyComplex, 1, Scalar.Yellow, 2);
                        }
                    }

                    using var window = new Window("El Bruno - Face and Banana Detector", smallFrame);
                    switch ((char)Cv2.WaitKey(100))
                    {
                        case (char)27: // ESC
                            run = false;
                            break;
                        case 'f':
                            showFace = !showFace;
                            break;
                    }
                }
            }
        }
    }
}
