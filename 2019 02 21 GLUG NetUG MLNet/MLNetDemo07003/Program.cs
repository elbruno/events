using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Core.Data;
using Microsoft.ML.Runtime.Data;
using MLNetDemo070.Shared;

namespace MLNetDemo07003
{
    class Program
    {
        static readonly string ModelPath = Path.Combine(Environment.CurrentDirectory, "Data", "ModelAgeRange.zip");

        static void Main(string[] args)
        {
            var mlContext = new MLContext(seed: 0);

            ITransformer loadedModel;
            using (var stream = new FileStream(ModelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                loadedModel = mlContext.Model.Load(stream);
            }

            var predictionFunction = loadedModel.MakePredictionFunction<AgeRange, AgeRangePrediction>(mlContext);

            PredictSimple("Jeff", 2, "M", predictionFunction);
            PredictSimple("Shelley", 9, "F", predictionFunction);
            PredictSimple("Jackie", 3, "F", predictionFunction);
            PredictSimple("Jim", 5, "M", predictionFunction);


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
