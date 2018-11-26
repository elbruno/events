using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Runtime.Data;
using MLNetDemo070.Shared;

namespace MLNetDemo07006
{
    class Program
    {
        static readonly string TrainDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "AgeRangeData01.csv");
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
            var pipeline = mlContext.Transforms.Concatenate("Features", "Age")
                    .Append(mlContext.Transforms.Categorical.MapValueToKey("Label"), TransformerScope.TrainTest)
                    .Append(mlContext.MulticlassClassification.Trainers.StochasticDualCoordinateAscent())
                    .Append(mlContext.Transforms.Conversion.MapKeyToValue(("PredictedLabel", "Label")));

            // Peek
            Common.ConsoleHelper.PeekDataViewInConsole<AgeRange>(mlContext, dvTrain, pipeline, 3);
            var model = pipeline.Fit(dvTrain);

            Console.WriteLine("Model trained");

            var predictionFunction = model.MakePredictionFunction<AgeRange, AgeRangePrediction>(mlContext);

            PredictSimple("Jeff", 2, "M", predictionFunction);
            PredictSimple("Shelley", 9, "F", predictionFunction);
            PredictSimple("Jackie", 3, "F", predictionFunction);
            PredictSimple("Jim", 5, "M", predictionFunction);

            // eval model



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
            Console.WriteLine($"Name: {example.Name}, Age: {example.Age}, Gender: {example.Gender} >> Predicted Label: {prediction.PredictedLabel}");
        }
    }
}
