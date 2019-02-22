using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;
using MLNetDemo010.Shared;

namespace MLNetGLUGConsole01
{
    class Program
    {

        static readonly string TrainDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "AgeRangeData01.csv");

        static void Main(string[] args)
        {
            var ml = new MLContext(seed: 1, conc: 1);

            // Read the data into a data view.
            var data = ml.Data.ReadFromTextFile<AgeRange>(TrainDataPath, hasHeader: true, separatorChar: ',');

            // Train
            var pipeline = ml.Transforms.Conversion.MapValueToKey("Label")
                .Append(ml.Transforms.Text.FeaturizeText("GenderFeat", "Gender"))
                .Append(ml.Transforms.Concatenate("Features", "Age", "GenderFeat"))
                .AppendCacheCheckpoint(ml)
                .Append(ml.MulticlassClassification.Trainers.StochasticDualCoordinateAscent(labelColumn: "Label", featureColumn: "Features"))
                .Append(ml.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            var model = pipeline.Fit(data);
            Console.WriteLine("Model trained");

            // Predict
            var engine = model.CreatePredictionEngine<AgeRange, AgeRangePrediction>(ml);
            PredictSimple("Jeff", 2, "M", engine);
            PredictSimple("Shelley", 9, "F", engine);
            PredictSimple("Jackie", 3, "F", engine);
            PredictSimple("Jim", 5, "M", engine);
            PredictSimple("T1", 14, "M", engine);
            PredictSimple("T2", 19, "F", engine);
            PredictSimple("T3", 27, "F", engine);
            PredictSimple("T4", 49, "M", engine);
            PredictSimple("T5", 5, "M", engine);
            PredictSimple("T6", 10, "F", engine);

            Console.ReadLine();
        }

        private static void PredictSimple(string name, float age, string gender, PredictionEngine<AgeRange, AgeRangePrediction> predictionFunction)
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
