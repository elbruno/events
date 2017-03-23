using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.Effects;

namespace Urho08.HoloCognitiveServicesFlow
{
    public class MrcVideoEffectDefinition : IVideoEffectDefinition
    {
        public string ActivatableClassId => "Windows.Media.MixedRealityCapture.MixedRealityCaptureVideoEffect";

        public IPropertySet Properties { get; }

        public MrcVideoEffectDefinition()
        {
            Properties = new PropertySet
            {
                {"HologramCompositionEnabled", false},
                {"RecordingIndicatorEnabled", false},
                {"VideoStabilizationEnabled", false},
                {"VideoStabilizationBufferLength", 0},
                {"GlobalOpacityCoefficient", 0.9f},
                {"StreamType", (int)MediaStreamType.Photo}
            };
        }
    }
}