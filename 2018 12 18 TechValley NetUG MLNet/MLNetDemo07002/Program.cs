using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Core.Data;
using Microsoft.ML.Runtime.Data;
using MLNetDemo070.Shared;


namespace MLNetDemo07002
{
    class Program
    {
        static readonly string TrainDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "AgeRangeData01.csv");
        static readonly string ModelPath = Path.Combine(Environment.CurrentDirectory, "Data", "ModelAgeRange.zip");
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

            // Train
            var dvTrain = _textLoader.Read(TrainDataPath);
            var pipeline =
                mlContext.Transforms.Concatenate("Features", "Age")
                    .Append(mlContext.Transforms.Categorical.MapValueToKey("Label"), TransformerScope.TrainTest)
                    .Append(mlContext.MulticlassClassification.Trainers.StochasticDualCoordinateAscent())
                    .Append(mlContext.Transforms.Conversion.MapKeyToValue(("PredictedLabel", "Label")));
            var model = pipeline.Fit(dvTrain);
            Console.WriteLine("Model trained");

            var predictionFunction = model.MakePredictionFunction<AgeRange, AgeRangePrediction>(mlContext);

            PredictSimple("Jeff", 2, "M", predictionFunction);
            PredictSimple("Shelley", 9, "F", predictionFunction);
            PredictSimple("Jackie", 3, "F", predictionFunction);
            PredictSimple("Jim", 5, "M", predictionFunction);

            // Save model
            using (var fs = new FileStream(ModelPath, FileMode.Create, FileAccess.Write, FileShare.Write))
                mlContext.Model.Save(model, fs);
            Console.WriteLine("The model is saved to {0}", ModelPath);
            
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
