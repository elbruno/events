using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using Windows.AI.MachineLearning;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

/// <summary>
/// How to CHANGE ONNX model for this sample application.
/// 1) Copy new ONNX model to "Assets" subfolder.
/// 2) Add model to "Project" under Assets folder by selecting "Add existing item"; navigate to new ONNX model and add.
///    Change properties "Build-Action" to "Content"  and  "Copy to Output Directory" to "Copy if Newer"
/// 3) Update the inialization of the variable "_ourOnnxFileName" to the name of the new model.
/// 4) In the constructor for OnnxModelOutput update the number of expected output labels.
/// </summary>

namespace OfficeWatcher03
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Stopwatch _stopwatch = new Stopwatch();
        private modelModel _model = null;
        private string _ourOnnxFileName = "model.onnx";
        private List<string> _labels = new List<string> { "Chair", "Goku", "Guitar", "Monitor" };


        public MainPage()
        {
            this.InitializeComponent();
        }

        private async Task LoadModelAsync()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => StatusBlock.Text = $"Loading {_ourOnnxFileName} ... patience ");

            try
            {
                _stopwatch = Stopwatch.StartNew();

                var modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/{_ourOnnxFileName}"));
                _model = await modelModel.CreateFromStreamAsync(modelFile);

                _stopwatch.Stop();
                Debug.WriteLine($"Loaded {_ourOnnxFileName}: Elapsed time: {_stopwatch.ElapsedMilliseconds} ms");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"error: {ex.Message}");
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => StatusBlock.Text = $"error: {ex.Message}");
                _model = null;
            }
        }

        private async void ButtonRun_Click(object sender, RoutedEventArgs e)
        {
            ButtonRun.IsEnabled = false;
            UIPreviewImage.Source = null;
            try
            {
                if (_model == null)
                {
                    // Load the model
                    await LoadModelAsync();
                }

                // Trigger file picker to select an image file
                FileOpenPicker fileOpenPicker = new FileOpenPicker();
                fileOpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                fileOpenPicker.FileTypeFilter.Add(".bmp");
                fileOpenPicker.FileTypeFilter.Add(".jpg");
                fileOpenPicker.FileTypeFilter.Add(".png");
                fileOpenPicker.FileTypeFilter.Add(".jpeg");
                fileOpenPicker.FileTypeFilter.Add(".gif");
                fileOpenPicker.ViewMode = PickerViewMode.Thumbnail;
                StorageFile selectedStorageFile = await fileOpenPicker.PickSingleFileAsync();

                SoftwareBitmap softwareBitmap;
                using (IRandomAccessStream stream = await selectedStorageFile.OpenAsync(FileAccessMode.Read))
                {
                    // Create the decoder from the stream 
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

                    // Get the SoftwareBitmap representation of the file in BGRA8 format
                    softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                    softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                }

                // Display the image
                SoftwareBitmapSource imageSource = new SoftwareBitmapSource();
                await imageSource.SetBitmapAsync(softwareBitmap);
                UIPreviewImage.Source = imageSource;
                // Encapsulate the image in the WinML image type (VideoFrame) to be bound and evaluated
                VideoFrame inputImage = VideoFrame.CreateWithSoftwareBitmap(softwareBitmap);
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
                    //OnnxModelInput inputData = new OnnxModelInput();
                    modelInput inputData = new modelInput();
                    inputData.data = ImageFeatureValue.CreateFromVideoFrame(frame);
                    var output = await _model.EvaluateAsync(inputData);

                    var product = output.model_outputs0.GetAsVectorView()[0];


                    //var loss = output.Loss[0][product];
                    _stopwatch.Stop();
                    var lossStr = string.Join(",  ", product + " ");
                    string message = $"Evaluation took {_stopwatch.ElapsedMilliseconds}ms to execute, Predictions: {lossStr}.";


                    //var product = output.ClassLabel.GetAsVectorView()[0];
                    //var loss = output.Loss[0][product];
                    //_stopwatch.Stop();
                    //var lossStr = string.Join(",  ", product + " " + (loss * 100.0f).ToString("#0.00") + "%");
                    // string message = $"Evaluation took {_stopwatch.ElapsedMilliseconds}ms to execute, Predictions: {lossStr}.";

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
