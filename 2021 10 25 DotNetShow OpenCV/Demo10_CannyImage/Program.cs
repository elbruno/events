using System;
using OpenCvSharp;

namespace Demo01_CannyImage
{
    class Program
    {
        static void Main(string[] args)
        {
            using var src = new Mat("labs1.png", ImreadModes.Grayscale);
            using var dst = new Mat();

            Cv2.Canny(src, dst, 50, 200);
            using (new Window("src image", src))
            using (new Window("dst image", dst))
            {
                Cv2.WaitKey();
            }
        }
    }
}
