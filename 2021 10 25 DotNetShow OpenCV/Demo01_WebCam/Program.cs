using OpenCvSharp;
namespace Demo02_WebCam
{
    class Program
    {
        static void Main(string[] args)
        {
            using var capture = new VideoCapture(1);
            using var window = new Window("El Bruno - OpenCVSharp demo");
            var image = new Mat();
            var run = true;
            while (run)
            {
                capture.Read(image);
                if (image.Empty())
                    break;
                window.ShowImage(image);
                if (Cv2.WaitKey(1) == 113) // Q
                    run = false;
            }
        }
    }
}