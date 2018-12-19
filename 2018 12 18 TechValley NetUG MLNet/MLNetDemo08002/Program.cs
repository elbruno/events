using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Runtime.Data;
using MLNetDemo080.Shared;

namespace MLNetDemo08002
{
    class Program
    {
        static readonly string TrainDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "AgeRangeData02.csv");
        static TextLoader _textLoader;

        static void Main(string[] args)
        {
            var mlContext = new MLContext(seed: 0);
            _textLoader = mlContext.Data.TextReader(new TextLoader.Arguments()
            {
                Separator = ",",
                HasHeader = true,
                Column = new[]
                    {
                        new TextLoader.Column("Name", DataKind.Text, 0),
                        new TextLoader.Column("Age", DataKind.Num, 1),
                        new TextLoader.Column("Gender", DataKind.Text, 2),
                        new TextLoader.Column("Label", DataKind.Text, 3),
                    }
            });
            var dvTrain = _textLoader.Read(TrainDataPath);

            // Train
            var pipeline = mlContext.Transforms.Text.FeaturizeText("Gender", "GenderFeat")
                 .Append(mlContext.Transforms.Concatenate("Features", "Age", "GenderFeat"))
                 .Append(mlContext.Transforms.Conversion.MapValueToKey("Label"), TransformerScope.TrainTest)
                 .Append(mlContext.MulticlassClassification.Trainers.StochasticDualCoordinateAscent())
                 .Append(mlContext.Transforms.Conversion.MapKeyToValue(("PredictedLabel", "Label")));
            var model = pipeline.Fit(dvTrain);
            Console.WriteLine("Model trained");

            var predictionFunction = model.MakePredictionFunction<AgeRange, AgeRangePrediction>(mlContext);

            PredictSimple("Jeff", 2, "M", predictionFunction);
            PredictSimple("Shelley", 9, "F", predictionFunction);
            PredictSimple("Jackie", 3, "F", predictionFunction);
            PredictSimple("Jim", 5, "M", predictionFunction);
            PredictSimple("Jennie", 15, "F", predictionFunction);
            PredictSimple("Kendall", 8, "M", predictionFunction);
            PredictSimple("Maria", 7, "F", predictionFunction);
            PredictSimple("Ozgun", 16, "M", predictionFunction);

            Console.ReadLine();
        }

        private static void PredictSimple(string name, float age, string gender, PredictionFunction<AgeRange, AgeRangePrediction> predictionFunction)
        {
            var example = new AgeRange()
            {
                Age = age,
                Name = name,
                Gender = gender
            };
            var prediction = predictionFunction.Predict(example);
            Console.WriteLine($"Name: {example.Name}\t Age: {example.Age:00}\t Gender: {example.Gender}\t >> Predicted Label: {prediction.PredictedLabel}");
        }
    }
}
