using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace OfficeWatcher01
{
    public sealed partial class MainPage : Page
    {
        private Stopwatch _stopwatch = new Stopwatch();
        private string _ourOnnxFileName = "model.onnx";
        private ObjectDetection _objectDetection;


        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            IList<string> labels = new List<string> { "Chair", "Goku", "Guitar", "Monitor" };
            _objectDetection = new ObjectDetection(labels, 20);
            base.OnNavigatedTo(e);
        }

        private async Task LoadModelAsync()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => StatusBlock.Text = $"Loading {_ourOnnxFileName} ... patience ");

            try
            {
                _stopwatch = Stopwatch.StartNew();
                var modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/{_ourOnnxFileName}"));
                await _objectDetection.Init(modelFile);
                _stopwatch.Stop();
                Debug.WriteLine($"Loaded {_ourOnnxFileName}: Elapsed time: {_stopwatch.ElapsedMilliseconds} ms");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"error: {ex.Message}");
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => StatusBlock.Text = $"error: {ex.Message}");
                //_model = null;
            }
        }

        private async void ButtonRun_Click(object sender, RoutedEventArgs e)
        {
            ButtonRun.IsEnabled = false;
            UIPreviewImage.Source = null;
            try
            {
                if (_objectDetection == null)
                {
                    // Load the model
                    await LoadModelAsync();
                }

                // Trigger file picker to select an image file
                var fileOpenPicker = new FileOpenPicker
                {
                    SuggestedStartLocation = PickerLocationId.PicturesLibrary
                };
                fileOpenPicker.FileTypeFilter.Add(".bmp");
                fileOpenPicker.FileTypeFilter.Add(".jpg");
                fileOpenPicker.FileTypeFilter.Add(".png");
                fileOpenPicker.FileTypeFilter.Add(".jpeg");
                fileOpenPicker.FileTypeFilter.Add(".gif");
                fileOpenPicker.ViewMode = PickerViewMode.Thumbnail;
                var selectedStorageFile = await fileOpenPicker.PickSingleFileAsync();

                SoftwareBitmap softwareBitmap;
                using (var stream = await selectedStorageFile.OpenAsync(FileAccessMode.Read))
                {
                    // Create the decoder from the stream 
                    var decoder = await BitmapDecoder.CreateAsync(stream);

                    // Get the SoftwareBitmap representation of the file in BGRA8 format
                    softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                    softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                }

                // Display the image
                var imageSource = new SoftwareBitmapSource();
                await imageSource.SetBitmapAsync(softwareBitmap);
                UIPreviewImage.Source = imageSource;
                // Encapsulate the image in the WinML image type (VideoFrame) to be bound and evaluated
                var inputImage = VideoFrame.CreateWithSoftwareBitmap(softwareBitmap);
                // Evaluate the image
                await EvaluateVideoFrameAsync(inputImage);
                
            }
            catch (Exception ex)
            {
                var err_message = $"error: {ex.Message}";
                Debug.WriteLine(err_message);
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => StatusBlock.Text = err_message);
            }
            finally
            {
                ButtonRun.IsEnabled = true;
            }
        }

        private async Task EvaluateVideoFrameAsync(VideoFrame frame)
        {
            if (frame != null)
            {
                try
                {
                    _stopwatch.Restart();
                    var output = await _objectDetection.PredictImageAsync(frame);

                    var sb = new StringBuilder();
                    foreach (var predictionModel in output)
                    {
                        var line = $"{predictionModel.TagName} [{(predictionModel.Probability * 100.0f):#0.00} %]";
                        sb.AppendLine(line);
                    }

                    _stopwatch.Stop();

                    var message = $"Evaluation took {_stopwatch.ElapsedMilliseconds}ms to execute, Predictions: {sb}.";
                    Debug.WriteLine(message);
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => StatusBlock.Text = message);
                }
                catch (Exception ex)
                {
                    var err_message = $"error: {ex.Message}";
                    Debug.WriteLine(err_message);
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => StatusBlock.Text = err_message);
                }
            }
        }
    }
}
