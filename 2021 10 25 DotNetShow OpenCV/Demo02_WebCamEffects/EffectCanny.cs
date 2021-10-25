using OpenCvSharp;

namespace Demo02_WebCamEffects
{
    class EffectCanny : IEffect
    {
        public Mat applyEffect(Mat image)
        {
            var newImage = new Mat();
            Cv2.Canny(image, newImage, 50, 200);
            return newImage;
        }
    }
}
