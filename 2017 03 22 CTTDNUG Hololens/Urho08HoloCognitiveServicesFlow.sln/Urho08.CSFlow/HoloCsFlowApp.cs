using System;
using System.Collections.Generic;
using Windows.Media.Capture;
using Urho;
using Urho.HoloLens;
using Urho08.HoloCognitiveServicesFlow.Model;
using Urho08.HoloCognitiveServicesFlow.Processors;
using Urho08.MsFlow;

namespace Urho08.HoloCognitiveServicesFlow
{
    public class HoloCsFlowApp : HoloApplication
    {
        Node _busyIndicatorNode;
        MediaCapture _mediaCapture;
        bool _inited;
        bool _busy;
        bool _withPreview;
        private readonly FlowProcessor _flowProcessor;
        private readonly ComputerVisionProcessor _computerVisionProcessor;

        public HoloCsFlowApp(ApplicationOptions opts) : base(opts)
        {
            _flowProcessor = new FlowProcessor();
            _computerVisionProcessor = new ComputerVisionProcessor();
        }

        protected override async void Start()
        {
            ResourceCache.AutoReloadResources = true;
            base.Start();

            EnableGestureTapped = true;

            _busyIndicatorNode = Scene.CreateChild();
            _busyIndicatorNode.SetScale(0.06f);
            _busyIndicatorNode.CreateComponent<BusyIndicator>();

            _mediaCapture = new MediaCapture();
            await _mediaCapture.InitializeAsync();
            await _mediaCapture.AddVideoEffectAsync(new MrcVideoEffectDefinition(), MediaStreamType.Photo);
            await RegisterCortanaCommands(new Dictionary<string, Action> {
                {"Describe", () => CaptureAndShowResult(false)},
                {"Text describe", () => CaptureAndShowResult(true)},
                {"Enable preview", () => EnablePreview(true) },
                {"Disable preview", () => EnablePreview(false) },
                {"Help", Help }
            });

            ShowBusyIndicator(true);
            await TextToSpeech("Welcome to the Hololens and Flow demo!");
            ShowBusyIndicator(false);

            _withPreview = true;
            _inited = true;
        }

        async void Help()
        {
            await TextToSpeech("Available commands are:");
            foreach (var cortanaCommand in CortanaCommands.Keys)
                await TextToSpeech(cortanaCommand);
        }

        async void EnablePreview(bool enable)
        {
            _withPreview = enable;
            await TextToSpeech("Preview mode is " + (enable ? "enabled" : "disabled"));
        }

        public override void OnGestureDoubleTapped()
        {
            CaptureAndShowResult(false);
        }
        void ShowBusyIndicator(bool show)
        {
            _busy = show;
            _busyIndicatorNode.Position = LeftCamera.Node.WorldPosition + LeftCamera.Node.WorldDirection * 1f;
            _busyIndicatorNode.GetComponent<BusyIndicator>().IsBusy = show;
        }

        private async void CaptureAndShowResult(bool readText)
        {
            if (!_inited || _busy) return;
            ShowBusyIndicator(true);
            var resultCv = await _computerVisionProcessor.CaptureAndAnalyze(this, _mediaCapture, _withPreview, readText);
            await TextToSpeech(resultCv.Item1);
            await _flowProcessor.FlowMessage(resultCv.Item2, resultCv.Item1, readText);
            InvokeOnMain(() => ShowBusyIndicator(false));
        }
    }
}