using System;
using System.Collections.Generic;
using Microsoft.ML;
using SpikeDetection.DataStructures;

namespace MLNetAnomaly01
{
    class Program
    {
        static void Main(string[] args)
        {
            // define vars
            var mlContext = new MLContext();
            const int size = 36;

            // load data
            var dataView = mlContext.Data.LoadFromTextFile<ProductSalesData>(path: "product-sales.csv", hasHeader: true, separatorChar: ',');
            var productSalesList = new List<ProductSalesData>();
            var emptyDataView = mlContext.Data.LoadFromEnumerable(productSalesList);

            // Create Estimator and build model
            var estimator = mlContext.Transforms.DetectIidSpike(outputColumnName: nameof(ProductSalesPrediction.Prediction), inputColumnName: nameof(ProductSalesData.numSales), confidence: 85, pvalueHistoryLength: size / 4);
            var transformedModel = estimator.Fit(emptyDataView);

            // Prediction
            var transformedData = transformedModel.Transform(dataView);
            var predictions = mlContext.Data.CreateEnumerable<ProductSalesPrediction>(transformedData, reuseRowObject: false);

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
        }
    }
}
