using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Windows.Media;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;

namespace CustomVisionMarvel01
{
    public sealed partial class MainPage
    {

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

        private ObjectDetection _objectDetection;

        private async void InitModel()
        {
            _objectDetection = new ObjectDetection();
            var modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/model.onnx"));
            await _objectDetection.Init(modelFile);
        }

        private async Task LoadAndEvaluateModelAsync(VideoFrame videoFrame)
        {
            var result = await _objectDetection.PredictImageAsync(videoFrame);
            var message = $"{DateTime.Now.ToLongTimeString()}{Environment.NewLine}============================={Environment.NewLine}";
            message = result.Aggregate(message, (current, predictionModel) => current + $" {predictionModel.TagName} {(predictionModel.Probability * 100.0f):#0.00}% {Environment.NewLine}");
            Debug.WriteLine(message);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => TextBlockResults.Text = $"{message}");
        }
    }
}
