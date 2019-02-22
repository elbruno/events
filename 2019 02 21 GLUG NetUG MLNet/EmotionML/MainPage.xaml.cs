using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.AI.MachineLearning;
using Windows.Graphics.Imaging;
using Windows.Media;
using Windows.Storage;
using Windows.UI.Xaml.Navigation;

namespace WinMLDemo
{

    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await MyWebCam.StartAsync();
            MyWebCam.CameraHelper.FrameArrived += CameraHelper_FrameArrived;
        }

        private async void CameraHelper_FrameArrived(object sender, Microsoft.Toolkit.Uwp.Helpers.FrameEventArgs e)
        {
            if (e.VideoFrame.SoftwareBitmap == null) return;
            await LoadAndEvaluateModelAsync(e.VideoFrame, "model.onnx");
        }

        LearningModel _model;
        private TensorFeatureDescriptor _outputTensorDescriptor;
        private TensorFeatureDescriptor _inputImageDescriptor;

        private async Task LoadAndEvaluateModelAsync(VideoFrame _inputFrame, string _modelFileName)
        {
            LearningModelBinding _binding = null;
            VideoFrame _outputFrame = null;
            LearningModelSession _session;

            try
            {
                //Load and create the model
                if (_model == null)
                {
                    var modelFile =
                      await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///{_modelFileName}"));
                    _model = await LearningModel.LoadFromStorageFileAsync(modelFile);
                }

                // Create the evaluation session with the model
               _session = new LearningModelSession(_model);

                // Get input and output features of the model
                var inputFeatures = _model.InputFeatures.ToList();
                var outputFeatures = _model.OutputFeatures.ToList();

                // Create binding and then bind input/ output features
                 _binding = new LearningModelBinding(_session);

                _inputImageDescriptor =
                    inputFeatures.FirstOrDefault(feature => feature.Kind == LearningModelFeatureKind.Tensor) as TensorFeatureDescriptor;

                _outputTensorDescriptor =
                    outputFeatures.FirstOrDefault(feature => feature.Kind == LearningModelFeatureKind.Tensor) as TensorFeatureDescriptor;

                TensorFloat outputTensor = TensorFloat.Create(_outputTensorDescriptor.Shape);
                ImageFeatureValue imageTensor = ImageFeatureValue.CreateFromVideoFrame(_inputFrame);

                // Bind inputs +outputs
                _binding.Bind(_inputImageDescriptor.Name, imageTensor);
                _binding.Bind(_outputTensorDescriptor.Name, outputTensor);


                // Evaluate and get the results
                var results = await _session.EvaluateAsync(_binding, "test");
                Debug.WriteLine("ResultsEvaluated: " + results.ToString());

                var outputTensorList = outputTensor.GetAsVectorView();
                var resultsList = new List<float>(outputTensorList.Count);
                for (int i = 0; i < outputTensorList.Count; i++)
                {
                    resultsList.Add(outputTensorList[i]);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"error: {ex.Message}");
                _model = null;
            }
        }
    }
}
