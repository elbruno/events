using OpenCvSharp;

namespace Demo02_WebCamEffects
{
    internal interface IEffect
    {
        Mat applyEffect(Mat image);
    }
}