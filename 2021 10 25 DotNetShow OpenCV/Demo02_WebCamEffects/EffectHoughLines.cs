using System;
using System.Linq;
using Microsoft.VisualBasic.FileIO;
using OpenCvSharp;
using OpenCvSharp.XFeatures2D;
using OpenCvSharp.XPhoto;

namespace Demo02_WebCamEffects
{
    class EffectHoughLines : IEffect
    {
        public Mat applyEffect(Mat image)
        {
            var newImage = image.Clone();
            var gray = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

            // (1) Load the image
            using (Mat imgProb = image.Clone())
            {
                // Preprocess
                Cv2.Canny(gray, gray, 50, 200, 3, false);

                // (3) Run Standard Hough Transform 
                LineSegmentPolar[] segStd = Cv2.HoughLines(gray, 1, Math.PI / 180, 50, 0, 0);
                int limit = Math.Min(segStd.Length, 10);
                for (int i = 0; i < limit; i++)
                {
                    // Draws result lines
                    float rho = segStd[i].Rho;
                    float theta = segStd[i].Theta;
                    double a = Math.Cos(theta);
                    double b = Math.Sin(theta);
                    double x0 = a * rho;
                    double y0 = b * rho;
                    Point pt1 = new Point
                        {X = (int) Math.Round(x0 + 1000 * (-b)), Y = (int) Math.Round(y0 + 1000 * (a))};
                    Point pt2 = new Point
                        {X = (int) Math.Round(x0 - 1000 * (-b)), Y = (int) Math.Round(y0 - 1000 * (a))};
                    image.Line(pt1, pt2, Scalar.Red, 3, LineTypes.AntiAlias, 0);
                }

                // (4) Run Probabilistic Hough Transform
                LineSegmentPoint[] segProb = Cv2.HoughLinesP(gray, 1, Math.PI / 180, 50, 50, 10);
                foreach (LineSegmentPoint s in segProb)
                {
                    imgProb.Line(s.P1, s.P2, Scalar.Red, 3, LineTypes.AntiAlias, 0);
                }

                newImage = imgProb.Clone();
            }

            return newImage;
        }
    }
}
