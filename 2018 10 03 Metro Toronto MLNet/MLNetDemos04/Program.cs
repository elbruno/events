using System.IO;
using System.Text.RegularExpressions;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Models;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using MLNetAvaLL.Shared;

namespace MLNetDemos04
{
    class Program
    {
        const string FileName = "AgeRangeData.csv";
        const string OnnxPath = "SaveModelToOnnxTest.onnx";
        const string OnnxAsJsonPath = "SaveModelToOnnxTest.json";
        static void Main()
        {
            var pipeline = new LearningPipeline
            {
                new TextLoader(FileName).CreateFrom<AgeRange>(separator: ',', useHeader: true),
                new Dictionarizer("Label"),
                new TextFeaturizer("Gender", "Gender"),
                new ColumnConcatenator("Features", "Age", "Gender"),
                new StochasticDualCoordinateAscentClassifier(),
                new PredictedLabelColumnOriginalValueConverter {PredictedLabelColumn = "PredictedLabel"}
            };
            var model = pipeline.Train<AgeRange, AgeRangePrediction>();

            var converter = new OnnxConverter
            {
                Onnx = OnnxPath,
                Json = OnnxAsJsonPath,
                Domain = "com.elbruno"
            };

            converter.Convert(model);

            // Strip the version.
            var fileText = File.ReadAllText(OnnxAsJsonPath);
            fileText = Regex.Replace(fileText, "\"producerVersion\": \"([^\"]+)\"", "\"producerVersion\": \"##VERSION##\"");
            File.WriteAllText(OnnxAsJsonPath, fileText);
        }
    }
}
