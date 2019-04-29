using System;
using System.IO;
using Microsoft.ML;
using MLNetDemo010.Shared;

namespace MLNetDemo01002
{
    class Program
    {
        static readonly string TrainDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "AgeRangeData02.csv");

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
            PredictSimple("Jennie", 15, "F", engine);
            PredictSimple("Kendall", 8, "M", engine);
            PredictSimple("Maria", 7, "F", engine);
            PredictSimple("Ozgun", 16, "M", engine);
            PredictSimple("Greg", 35, "M", engine);
            PredictSimple("Andrea", 45, "F", engine);

            Console.ReadLine();
        }

        private static void PredictSimple(string name, float age, string gender, PredictionEngine<AgeRange, AgeRangePrediction> engine)
        {
            var example = new AgeRange()
            {
                Age = age,
                Name = name,
                Gender = gender
            };
            var prediction = engine.Predict(example);
            Console.WriteLine($"Name: {example.Name}\t Age: {example.Age:00}\t Gender: {example.Gender}\t >> Predicted Label: {prediction.PredictedLabel}");
        }
    }
}
