using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace CustomVisionParkingLot02
{
    public sealed partial class MainPage
    {
        private ObjectDetection _objectDetection;
        private readonly SolidColorBrush _lineBrushYellow = new SolidColorBrush(Windows.UI.Colors.Yellow);
        private readonly SolidColorBrush _lineBrushGreen = new SolidColorBrush(Windows.UI.Colors.Green);
        private readonly SolidColorBrush _lineBrushGray = new SolidColorBrush(Windows.UI.Colors.DarkGray);
        private readonly SolidColorBrush _fillBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
        private readonly double _lineThickness = 2.0;
        private Stopwatch _stopwatch;
        private uint _overlayCanvasActualWidth;
        private uint _overlayCanvasActualHeight;
        private IList<PredictionModel> _predictions;
        int _processingFlag;

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
            if (Interlocked.CompareExchange(ref this._processingFlag, 1, 0) == 0)
            {
                try
                {
                    if (e.VideoFrame.SoftwareBitmap == null) return;
                    if (_objectDetection?.Model == null) return;
                    await LoadAndEvaluateModelAsync(e.VideoFrame);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                }
                finally
                {
                    Interlocked.Exchange(ref this._processingFlag, 0);
                }
            }
        }

        private async void InitModel()
        {
            _objectDetection = new ObjectDetection();
            var modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/Iteration03.onnx"));
            await _objectDetection.Init(modelFile);
        }

        private void SliderThreshold_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (_objectDetection != null)
                _objectDetection.ProbabilityThreshold = ((float)e.NewValue)/100;
        }

        private async Task LoadAndEvaluateModelAsync(VideoFrame videoFrame)
        {
            _stopwatch = Stopwatch.StartNew();
            _predictions = await _objectDetection.PredictImageAsync(videoFrame);
            _stopwatch.Stop();

            var message = string.Empty;
            foreach (var model in _predictions)
            {
                //if (model.Probability * 100.0f > _threshold)
                message += $" {model.TagName} {(model.Probability * 100.0f):#0.00}% {Environment.NewLine}";
            }
            message += $"{Environment.NewLine}{DateTime.Now.ToLongTimeString()} - {1000f / _stopwatch.ElapsedMilliseconds,4:f1} fps";
            Debug.WriteLine(message);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                TextBlockResults.Text = $"{message}";
                DrawFrames();
            });
        }

        private void DrawFrames()
        {
            OverlayCanvas.Children.Clear();
            if (_predictions.Count <= 0) return;
            foreach (var prediction in _predictions)
                DrawFrame(prediction, OverlayCanvas);
        }

        private void DrawFrame(PredictionModel prediction, Canvas overlayCanvas)
        {
            _overlayCanvasActualWidth = (uint)CameraPreview.ActualWidth;
            _overlayCanvasActualHeight = (uint)CameraPreview.ActualHeight;

            prediction.BoundingBox.Normalize();
            var x = (uint)Math.Max(prediction.BoundingBox.Left, 0);
            var y = (uint)Math.Max(prediction.BoundingBox.Top, 0);
            var w = (uint)Math.Min(_overlayCanvasActualWidth - x, prediction.BoundingBox.Width);
            var h = (uint)Math.Min(_overlayCanvasActualHeight - y, prediction.BoundingBox.Height);

            // fit to current size
            var factorSize = 100u;
            x = _overlayCanvasActualWidth * x / factorSize;
            y = _overlayCanvasActualHeight * y / factorSize;
            w = _overlayCanvasActualWidth * w / factorSize;
            h = _overlayCanvasActualHeight * h / factorSize;

            Debug.WriteLine($"\tScaled\tY: {y}, x: {x}, Height: {h}, Width: {w}");

            var rectStroke = _lineBrushGreen;
            switch (prediction.TagName)
            {
                case "spot 1":
                    rectStroke = _lineBrushGray;
                    break;
                case "spot 2":
                    rectStroke = _lineBrushYellow;
                    break;
                case "spot 3":
                    rectStroke = _lineBrushGreen;
                    break;
            }

            var r = new Windows.UI.Xaml.Shapes.Rectangle
            {
                Tag = prediction,
                Width = w,
                Height = h,
                Fill = _fillBrush,
                Stroke = rectStroke,
                StrokeThickness = _lineThickness,
                Margin = new Thickness(x, y, 0, 0)
            };

            var tb = new TextBlock
            {
                Margin = new Thickness(x + 4, y + 4, 0, 0),
                Text = $"{prediction}",
                FontWeight = FontWeights.Bold,
                Width = 126,
                Height = 21,
                HorizontalTextAlignment = TextAlignment.Center
            };

            var textBack = new Windows.UI.Xaml.Shapes.Rectangle
            {
                Width = 150,
                Height = 29,
                Fill = rectStroke,
                Margin = new Thickness(x, y, 0, 0)
            };

            overlayCanvas.Children.Add(textBack);
            overlayCanvas.Children.Add(tb);
            overlayCanvas.Children.Add(r);
        }
    }
}
