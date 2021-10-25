using OpenCvSharp;

namespace Demo02_WebCamEffects
{
    class EffectFast : IEffect
    {
        public Mat applyEffect(Mat image)
        {
            var newImage = new Mat();
            Cv2.CvtColor(image, newImage, ColorConversionCodes.BGR2GRAY, 0);
            KeyPoint[] keypoints = Cv2.FAST(newImage, 50, true);
            foreach (KeyPoint kp in keypoints)
                newImage.Circle((Point)kp.Pt, 3, Scalar.Red, -1, LineTypes.AntiAlias, 0);
            return newImage;
        }
    }
}
