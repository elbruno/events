using System.Linq;
using Microsoft.VisualBasic.FileIO;
using OpenCvSharp;
using OpenCvSharp.XFeatures2D;
using OpenCvSharp.XPhoto;

namespace Demo02_WebCamEffects
{
    class EffectFreak : IEffect
    {
        public Mat applyEffect(Mat image)
        {
            var newImage = image.Clone();
            var gray = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

            // ORB
            var orb = ORB.Create(1000);
            KeyPoint[] keypoints = orb.Detect(gray);

            // FREAK
            FREAK freak = FREAK.Create();
            Mat freakDescriptors = new Mat();
            freak.Compute(gray, ref keypoints, freakDescriptors);

            if (keypoints != null)
            {
                var color = new Scalar(0, 255, 0);
                foreach (KeyPoint kpt in keypoints)
                {
                    float r = kpt.Size / 2;
                    Cv2.Circle(newImage, (Point)kpt.Pt, (int)r, color);
                    Cv2.Line(newImage,
                        (Point)new Point2f(kpt.Pt.X + r, kpt.Pt.Y + r),
                        (Point)new Point2f(kpt.Pt.X - r, kpt.Pt.Y - r),
                        color);
                    Cv2.Line(newImage,
                        (Point)new Point2f(kpt.Pt.X - r, kpt.Pt.Y + r),
                        (Point)new Point2f(kpt.Pt.X + r, kpt.Pt.Y - r),
                        color);
                }
            }
            return newImage;
        }
    }
}
