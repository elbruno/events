using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Transforms;
using MLNetTestsRC010.Shared;

namespace MLNetDemoRc01001
{
    class Program
    {
        static readonly string TrainDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "AgeRangeData01.csv");

        static void Main(string[] args)
        {
            var mlContext = new MLContext(seed: 0);
            var trainingDataView = mlContext.Data.LoadFromTextFile<AgeRange>(path: TrainDataPath, hasHeader: true, separatorChar: ',');

            var dataProcessPipeline = mlContext.Transforms.Conversion
                .MapValueToKey(outputColumnName: "KeyLabel", inputColumnName: nameof(AgeRange.Label),
                    keyOrdinality: ValueToKeyMappingEstimator.KeyOrdinality.ByValue)
                .Append(mlContext.Transforms.Concatenate(outputColumnName: "Features", nameof(AgeRange.Age)))
                .AppendCacheCheckpoint(mlContext);

            ConsoleHelper.PeekDataViewInConsole(mlContext, trainingDataView, dataProcessPipeline, 4);

            var trainer = mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy("KeyLabel", "Features");

            var trainingPipeline = dataProcessPipeline.Append(trainer)
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel", "PredictedLabel"));
            var trainedModel = trainingPipeline.Fit(trainingDataView);

            Console.WriteLine("Model trained");

            var predEngine = mlContext.Model.CreatePredictionEngine<AgeRange, AgeRangePrediction>(trainedModel);

            PredictSimple("Jeff", 2, "M", predEngine);
            PredictSimple("Shelley", 9, "F", predEngine);
            PredictSimple("Jackie", 13, "F", predEngine);
            PredictSimple("Jim", 5, "M", predEngine);
            PredictSimple("Allie", 3, "F", predEngine);
            PredictSimple("Yonhoo", 15, "M", predEngine);

            Console.ReadLine();
        }

        private static void PredictSimple(string name, float age, string gender, PredictionEngine<AgeRange, AgeRangePrediction> predictionEngine)
        {
            var example = new AgeRange()
            {
                Age = age,
                Name = name,
                Gender = gender
            };
            var prediction = predictionEngine.Predict(example);
            Console.WriteLine($"Name: {example.Name}\t Age: {example.Age:00}\t Gender: {example.Gender}\t - Predicted Label: {prediction.Label}");
        }
    }
}
