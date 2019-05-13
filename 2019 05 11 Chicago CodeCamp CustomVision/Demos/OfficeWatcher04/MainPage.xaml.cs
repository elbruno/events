using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Windows.AI.MachineLearning;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Storage;
using Windows.Storage.Pickers;
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

namespace OfficeWatcher04
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Stopwatch _stopwatch = new Stopwatch();
        private OnnxModel _model = null;
        private string _ourOnnxFileName = "model.onnx";

        public sealed class OnnxModelInput
        {
            public VideoFrame Data { get; set; }
        }

        public sealed class ModelOutput
        {
            public TensorFloat Model_outputs0 = TensorFloat.Create(new long[] { 1, 4 });
        }

        public sealed class OnnxModel
        {
            private LearningModel _learningModel = null;
            private LearningModelSession _session;

            public static async Task<OnnxModel> CreateOnnxModel(StorageFile file)
            {
                LearningModel learningModel = null;

                try
                {
                    learningModel = await LearningModel.LoadFromStorageFileAsync(file);
                }
                catch (Exception e)
                {
                    var exceptionStr = e.ToString();
                    Console.WriteLine(exceptionStr);
                    throw e;
                }
                return new OnnxModel()
                {
                    _learningModel = learningModel,
                    _session = new LearningModelSession(learningModel)
                };
            }

            public async Task<ModelOutput> EvaluateAsync(OnnxModelInput input)
            {
                var output = new ModelOutput();
                var binding = new LearningModelBinding(_session);
                binding.Bind("data", input.Data);
                binding.Bind("model_outputs0", output.Model_outputs0);
                var evalResult = await _session.EvaluateAsync(binding, "0");
                return output;
            }
        }

        public MainPage()
        {
            InitializeComponent();
        }

        private async Task LoadModelAsync()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => StatusBlock.Text = $"Loading {_ourOnnxFileName} ... patience ");

            try
            {
                _stopwatch = Stopwatch.StartNew();

                var modelFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/{_ourOnnxFileName}"));
                _model = await OnnxModel.CreateOnnxModel(modelFile);

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
                var fileOpenPicker = new FileOpenPicker();
                fileOpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
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
                    var inputData = new OnnxModelInput { Data = frame };
                    var output = await _model.EvaluateAsync(inputData);
                    _stopwatch.Stop();

                    // display Shape values
                    Debug.WriteLine($"Shape values: {output.Model_outputs0.Shape.Count}");
                    for (var i = 0; i < output.Model_outputs0.Shape.Count; i++)
                    {
                        Debug.WriteLine($"Index {i} Shape value: {output.Model_outputs0.Shape[i]}");
                    }

                    //// display full set of values
                    //// 45 * 13 * 13 = 7604 values
                    //var vars = output.Model_outputs0.GetAsVectorView();
                    //for (var i = 0; i < vars.Count; i++)
                    //{
                    //    Debug.WriteLine($"Index {i} value: {vars[i]}");
                    //}

                    //var product = output.Model_outputs0.GetAsVectorView()[0];
                    //var evalResult = string.Join(",  ", product);
                    //string message = $"Evaluation took {_stopwatch.ElapsedMilliseconds}ms to execute, Predictions: {evalResult}.";
                    //Debug.WriteLine(message);

                    var message = $"Evaluation took {_stopwatch.ElapsedMilliseconds}ms to execute, Predictions: ";

                    var ra = new ResultsAnalyzer();
                    var analRes = ra.Postprocess(output.Model_outputs0);
                    foreach (var predictionModel in analRes)
                    {
                        var pred = $" {predictionModel.TagName} {(predictionModel.Probability * 100.0f):#0.00} % /-/-/";
                        message += pred;
                    }

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
