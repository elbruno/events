using OpenCvSharp;

namespace Demo02_WebCamEffects
{
    public class Effects
    {
        public enum EffectType { None, Canny, Fast,
            Brisk,
            Freak,
            HoughLines,
            Star
        }

        public EffectType Effect;

        public Mat ApplyEffect(Mat image)
        {
            IEffect effect = Effect switch
            {
                EffectType.Brisk => new EffectBrisk(),
                EffectType.Canny => new EffectCanny(),
                EffectType.Fast => new EffectFast(),
                EffectType.Freak => new EffectFreak(),
                EffectType.HoughLines => new EffectHoughLines(),
                EffectType.Star => new EffectStar(),
                _ => new EffectNone()
            };

            return effect.applyEffect(image);
        }

        public void ApplyEffect(char key)
        {
            Effect = key switch
            {
                'b' => EffectType.Brisk,
                'c' => EffectType.Canny,
                'f' => EffectType.Fast,
                'g' => EffectType.Freak,
                'h' => EffectType.HoughLines,
                'n' => EffectType.None,
                's' => EffectType.Star,
                _ => Effect
            };

        }
    }
}
