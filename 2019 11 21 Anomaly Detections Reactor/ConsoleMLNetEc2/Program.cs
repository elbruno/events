using System;
using System.Collections.Generic;
using Microsoft.ML;
using Microsoft.ML.Transforms.TimeSeries;
using SpikeDetection.DataStructures;

namespace ConsoleMLNetEc2
{
    internal static class Program
    {
        static void Main()
        {
            var mlContext = new MLContext();
            const int size = 7267;

            // load data
            var dataView = mlContext.Data.LoadFromTextFile<Ec2Data>(path: "ec2_request_latency_system_failure.csv", hasHeader: true, separatorChar: ',');
            var productSalesList = new List<Ec2Data>();
            var emptyDataView = mlContext.Data.LoadFromEnumerable(productSalesList);

            // Create Estimator and build model
            string outputColumnName = nameof(Ec2Prediction.Prediction);
            string inputColumnName = nameof(Ec2Data.value);
            //var estimator = mlContext.Transforms.DetectIidSpike(outputColumnName, inputColumnName, confidence: 95, pvalueHistoryLength: size / 15);

            const int ConfidenceInterval = 98;
            const int PValueSize = size / 15;
            const int TrainingSize = size / 5;
            const int SeasonalitySize = size / 15;

            var estimator = mlContext.Transforms.DetectSpikeBySsa(
                outputColumnName,
                inputColumnName,
                confidence: ConfidenceInterval,
                pvalueHistoryLength: PValueSize,
                trainingWindowSize: TrainingSize,
                seasonalityWindowSize: SeasonalitySize);

            var transformedModel = estimator.Fit(emptyDataView);

            // Prediction
            var transformedData = transformedModel.Transform(dataView);
            var predictions = mlContext.Data.CreateEnumerable<Ec2Prediction>(transformedData, reuseRowObject: false, true);

            // analyze predictions
            Console.WriteLine("Index\tAlert\tScore\tP-Value");
            int i = 0;
            foreach (var p in predictions)
            {
                i++;
                if (p.Prediction[0] == 1 || p.Prediction[2] == (double)0 )
                {
                    Console.WriteLine("{0}\t{1}\t{2:0.00}\t{3:0.00}", i, p.Prediction[0], p.Prediction[1], p.Prediction[2]);
                }
            }
            Console.WriteLine("Done !");
            Console.ReadLine();
        }
    }
}