using OpenCvSharp;

namespace Demo02_WebCamEffects
{
    class EffectBrisk : IEffect
    {
        public Mat applyEffect(Mat image)
        {
            var newImage = image.Clone();
            var gray = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

            var brisk = BRISK.Create();
            var keypoints = brisk.Detect(gray);

            {
                var color = new Scalar(0, 255, 0);
                foreach (var kpt in keypoints)
                {
                    var r = kpt.Size / 2;
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
