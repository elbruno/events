using OpenCvSharp;

namespace Demo02_WebCamEffects
{
    class EffectNone : IEffect
    {
        public Mat applyEffect(Mat image)
        {
            return image;
        }
    }
}
