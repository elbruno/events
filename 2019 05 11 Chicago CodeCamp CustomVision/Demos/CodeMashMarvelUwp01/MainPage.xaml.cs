using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace CodeMashMarvelUwp01
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
            if (_stopwatch != null && _stopwatch.IsRunning) return;
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
        private IList<PredictionModel> _predictions = new List<PredictionModel>();
        private readonly SolidColorBrush _lineBrushYellow = new SolidColorBrush(Windows.UI.Colors.Yellow);
        private readonly SolidColorBrush _lineBrushGreen = new SolidColorBrush(Windows.UI.Colors.Green);
        private readonly SolidColorBrush _lineBrushGray = new SolidColorBrush(Windows.UI.Colors.DarkGray);
        private readonly SolidColorBrush _fillBrush = new SolidColorBrush(Windows.UI.Colors.Transparent);
        private readonly double _lineThickness = 2.0;
        private Stopwatch _stopwatch;
        private uint _overlayCanvasActualWidth;
        private uint _overlayCanvasActualHeight;


        private async void InitModel()
        {
            _objectDetection = new ObjectDetection();
            var modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/model.onnx"));
            await _objectDetection.Init(modelFile);
        }

        private async Task LoadAndEvaluateModelAsync(VideoFrame videoFrame)
        {
            _objectDetection.ProbabilityThreshold = 0.5F;
            _stopwatch = Stopwatch.StartNew();
            _predictions = await _objectDetection.PredictImageAsync(videoFrame);
            _stopwatch.Stop();
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var message = $"{DateTime.Now.ToLongTimeString()} - {1000f / _stopwatch.ElapsedMilliseconds,4:f1} fps";
                TextBlockResults.Text = message; DrawFrames();
            });
        }

        private void DrawFrames()
        {
            OverlayCanvas.Children.Clear();

            if (_predictions.Count > 0)
                foreach (var prediction in _predictions)
                    DrawFrame(prediction, OverlayCanvas);
        }

        private void DrawFrame(PredictionModel prediction, Canvas overlayCanvas)
        {
            _overlayCanvasActualWidth = (uint)CameraPreview.ActualWidth;
            _overlayCanvasActualHeight = (uint)CameraPreview.ActualHeight;

            var x = (uint)Math.Max(prediction.Box.Left, 0);
            var y = (uint)Math.Max(prediction.Box.Top, 0);
            var w = (uint)Math.Min(_overlayCanvasActualWidth - x, prediction.Box.Width);
            var h = (uint)Math.Min(_overlayCanvasActualHeight - y, prediction.Box.Height);

            Debug.WriteLine($"\tOverLay Canvas \tActual Width: {_overlayCanvasActualWidth}, Actual Height: {_overlayCanvasActualHeight}");
            Debug.WriteLine(prediction.GetExtendedDescription());
            Debug.WriteLine($"\tOriginal\tY: {y}, x: {x}, Height: {h}, Width: {w}");

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
                case "Venom":
                    rectStroke = _lineBrushGray;
                    break;
                case "Rocket_Racoon":
                    rectStroke = _lineBrushYellow;
                    break;
                case "Iron_Fist":
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

        //private async void ButtonFile_OnClick(object sender, RoutedEventArgs e)
        //{
        //    ButtonFile.IsEnabled = false;
        //    UIPreviewImage.Source = null;
        //    try
        //    {
        //        // Trigger file picker to select an image file
        //        var fileOpenPicker = new FileOpenPicker { SuggestedStartLocation = PickerLocationId.PicturesLibrary };
        //        fileOpenPicker.FileTypeFilter.Add(".bmp");
        //        fileOpenPicker.FileTypeFilter.Add(".jpg");
        //        fileOpenPicker.FileTypeFilter.Add(".png");
        //        fileOpenPicker.FileTypeFilter.Add(".jpeg");
        //        fileOpenPicker.FileTypeFilter.Add(".gif");
        //        fileOpenPicker.ViewMode = PickerViewMode.Thumbnail;
        //        var selectedStorageFile = await fileOpenPicker.PickSingleFileAsync();

        //        SoftwareBitmap softwareBitmap;
        //        using (var stream = await selectedStorageFile.OpenAsync(FileAccessMode.Read))
        //        {
        //            // Create the decoder from the stream 
        //            var decoder = await BitmapDecoder.CreateAsync(stream);

        //            // Get the SoftwareBitmap representation of the file in BGRA8 format
        //            softwareBitmap = await decoder.GetSoftwareBitmapAsync();
        //            softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
        //        }

        //        // Display the image
        //        var imageSource = new SoftwareBitmapSource();
        //        await imageSource.SetBitmapAsync(softwareBitmap);
        //        UIPreviewImage.Source = imageSource;
        //        // Encapsulate the image in the WinML image type (VideoFrame) to be bound and evaluated
        //        var inputImage = VideoFrame.CreateWithSoftwareBitmap(softwareBitmap);

        //        _overlayCanvasActualWidth = (uint)OverlayCanvas.ActualWidth;
        //        _overlayCanvasActualHeight = (uint)OverlayCanvas.ActualHeight;

        //        // Evaluate the image
        //        await LoadAndEvaluateModelAsync(inputImage);

        //    }
        //    catch (Exception ex)
        //    {
        //        var errMessage = $"error: {ex.Message}";
        //        Debug.WriteLine(ex);
        //        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => TextBlockResults.Text = errMessage);
        //    }
        //    finally
        //    {
        //        ButtonFile.IsEnabled = true;
        //    }
        //}
    }
}
