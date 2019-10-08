using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

namespace CustomVisionParkingLot01
{
    public sealed partial class MainPage
    {
        private Stopwatch _stopwatch;
        private ObjectDetection _objectDetection;
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            InitModel();
            await CameraPreview.StartAsync();
            CameraPreview.CameraHelper.FrameArrived += CameraHelper_FrameArrived;
        }

        private async void CameraHelper_FrameArrived(object sender, Microsoft.Toolkit.Uwp.Helpers.FrameEventArgs e)
        {
            if (e.VideoFrame.SoftwareBitmap == null) return;
            try
            {
                if (_objectDetection?.Model == null) return;
                await LoadAndEvaluateModelAsync(e.VideoFrame);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        private async void InitModel()
        {
            _objectDetection = new ObjectDetection();
            var modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/Iteration03.onnx"));
            await _objectDetection.Init(modelFile);
        }

        private async Task LoadAndEvaluateModelAsync(VideoFrame videoFrame)
        {
            _stopwatch = Stopwatch.StartNew();
            var result = await _objectDetection.PredictImageAsync(videoFrame);
            _stopwatch.Stop();
            var message = $"{Environment.NewLine}{DateTime.Now.ToLongTimeString()} - {1000f / _stopwatch.ElapsedMilliseconds,4:f1} fps";
            message = result.Aggregate(message, (current, model) => current + $"{Environment.NewLine} >>{model.TagName} {(model.Probability * 100.0f):#0.00}% ");
            Debug.WriteLine(message);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => TextBlockResults.Text = $"{message}");
        }

        private void SliderThreshold_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (_objectDetection != null)
                _objectDetection.ProbabilityThreshold = (float)e.NewValue / 100;
        }
    }
}
