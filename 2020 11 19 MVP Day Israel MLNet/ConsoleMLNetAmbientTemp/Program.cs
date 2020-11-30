using System;
using System.Collections.Generic;
using Microsoft.ML;
using SpikeDetection.DataStructures;

namespace ConsoleMLNetAmbientTemp
{
    internal static class Program
    {
        static void Main()
        {
            var mlContext = new MLContext();
            const int size = 7267;

            // load data
            var dataView = mlContext.Data.LoadFromTextFile<AmbientTempData>(path: "ambient_temperature_system_failure.csv", hasHeader: true, separatorChar: ',');
            var productSalesList = new List<AmbientTempData>();
            var emptyDataView = mlContext.Data.LoadFromEnumerable(productSalesList);

            // Create Estimator and build model
            var estimator = mlContext.Transforms.DetectIidSpike(outputColumnName: nameof(AmbientTempPrediction.Prediction), inputColumnName: nameof(AmbientTempData.numSales), confidence: 95, pvalueHistoryLength: size / 4);
            var transformedModel = estimator.Fit(emptyDataView);

            // Prediction
            var transformedData = transformedModel.Transform(dataView);
            var predictions = mlContext.Data.CreateEnumerable<AmbientTempPrediction>(transformedData, reuseRowObject: false);

            // analyze predictions
            Console.WriteLine("Alert\tScore\tP-Value");
            foreach (var p in predictions)
            {
                if (p.Prediction[0] == 1)
                {
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.WriteLine("{0}\t{1:0.00}\t{2:0.00}", p.Prediction[0], p.Prediction[1], p.Prediction[2]);
                Console.ResetColor();
            }
            Console.WriteLine("");
            Console.ReadLine();
        }
    }
}