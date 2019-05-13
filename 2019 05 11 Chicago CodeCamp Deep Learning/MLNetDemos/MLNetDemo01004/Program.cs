using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms.Text;
using MLNetDemo010.Shared;

namespace MLNetDemo01004
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
                .Append(ml.Transforms.Text.FeaturizeText("Features", new List<string> { "Age", "Gender" },
                    new Action<TextFeaturizingEstimator.Settings>(). ..StochasticGradientDescentClassificationTrainer.Options { OutputTokens = true }));
                .Append(ml.MulticlassClassification.Trainers.StochasticDualCoordinateAscent(labelColumn: "Label", featureColumn: "Features"))
                .Append(ml.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            var model = pipeline.Fit(data);
            Console.WriteLine("Model trained");

            var ageColumn = data.GetColumn<string>(ml, "Age").Take(20);
            var transformedAgeColumn = data.GetColumn<string[]>(ml, "Features_TransformedText").Take(20);
            var features = data.GetColumn<float[]>(ml, "Features").Take(20);

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
