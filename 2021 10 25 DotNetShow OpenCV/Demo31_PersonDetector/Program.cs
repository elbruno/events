using System;
using OpenCvSharp;

namespace ConsoleApp4
{
    class Program
    {
        static void Main(string[] args)
        {
            using var capture = new VideoCapture(1); // videoUDP);
            {
                var window = new Window("capture");
                using var hog = new HOGDescriptor();
                hog.SetSVMDetector(HOGDescriptor.GetDefaultPeopleDetector());

                var image = new Mat();
                var showPeople = true;
                var run = true;

                while (run)
                {
                    capture.Read(image);
                    if (image.Empty())
                        break;

                    var newSize = new Size(640, 480);
                    using var smallFrame = new Mat();
                    Cv2.Resize(image, smallFrame, newSize);

                    if (showPeople)
                    {
                        Rect[] people = hog.DetectMultiScale(smallFrame, 0, new Size(8, 8), new Size(24, 16), 1.05, 2);
                        // Rect[] people = hog.DetectMultiScale(smallFrame);

                        foreach (var person in people)
                            Cv2.Rectangle(smallFrame, person, Scalar.Aqua);
                    }

                    window.Image = smallFrame;

                    switch ((char)Cv2.WaitKey(100))
                    {
                        case (char)27: // ESC
                            run = false;
                            break;
                        case 'p':
                            showPeople = !showPeople;
                            break;
                    }
                }
            }
        }
    }
}
