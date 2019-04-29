using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Runtime.Data;
using MLNetDemo080.Shared;

namespace MLNetTechValley01
{
    class Program
    {
        static readonly string TrainDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "AgeRangeData01.csv");
        static TextLoader _textLoader;

        static void Main(string[] args)
        {
            // Data
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
            var pipeline = 
                mlContext.Transforms.Text.FeaturizeText("Gender", "GenderFeat")
                .Append(mlContext.Transforms.Concatenate("Features", "Age", "GenderFeat"))
                .Append(mlContext.Transforms.Conversion.MapValueToKey("Label"), TransformerScope.TrainTest)
                .Append(mlContext.MulticlassClassification.Trainers.StochasticDualCoordinateAscent())
                .Append(mlContext.Transforms.Conversion.MapKeyToValue(("PredictedLabel", "Label")));
            var model = pipeline.Fit(dvTrain);
            Console.WriteLine("Model trained");


            // Predict
            var predictionFunction = model.MakePredictionFunction<AgeRange, AgeRangePrediction>(mlContext);
            PredictSimple("Jeff", 2, "M", predictionFunction);
            PredictSimple("Shelley", 9, "F", predictionFunction);
            PredictSimple("Jackie", 3, "F", predictionFunction);
            PredictSimple("Jim", 5, "M", predictionFunction);

            PredictSimple("TestA", 16, "M", predictionFunction);
            PredictSimple("TestB", 32, "F", predictionFunction);
            PredictSimple("TestC", 18, "M", predictionFunction);
            PredictSimple("TestD", 42, "M", predictionFunction);

            PredictSimple("TestE", 35, "F", predictionFunction);
            PredictSimple("TestF", 42, "F", predictionFunction);


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
