using System;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;
using MLNetDemo010.Shared;

namespace MLNetDemo01003
{
    class Program
    {
        static readonly string TrainDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "AgeRangeData03.csv");

        static readonly string TestDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "AgeRangeData03Test.csv");

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

            var testData = ml.CreateEnumerable<AgeRange>(
                ml.Data.ReadFromTextFile<AgeRange>(TestDataPath, hasHeader: true, separatorChar: ','), false);
            foreach (var input in testData)
                PredictSimple(input, engine);

            Console.ReadLine();
        }

        private static void PredictSimple(AgeRange example, PredictionEngine<AgeRange, AgeRangePrediction> engine)
        {
            var prediction = engine.Predict(example);
            Console.WriteLine($"Name: {example.Name}\t Age: {example.Age:00}\t Gender: {example.Gender}\t Original Label: {example.Label}\t>> Predicted Label: {prediction.PredictedLabel}");
        }
    }
}
