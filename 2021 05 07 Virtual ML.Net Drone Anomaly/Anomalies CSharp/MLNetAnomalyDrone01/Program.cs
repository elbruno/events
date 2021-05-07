using System;
using System.Collections.Generic;
using Microsoft.ML;
using SpikeDetection.DataStructures;

namespace MLNetAnomalyDrone01
{
    class Program
    {
        static void Main(string[] args)
        {
            // define vars
            var mlContext = new MLContext();
            const int size = 150;

            // load data
            var dataView = mlContext.Data.LoadFromTextFile<DroneData>(path: "dronedata02.csv", hasHeader: true, separatorChar: ',');
            var droneTelemetryList = new List<DroneData>();
            var emptyDataView = mlContext.Data.LoadFromEnumerable(droneTelemetryList);

            // Create Estimator and build model
            var estimator = mlContext.Transforms.DetectIidSpike(outputColumnName: nameof(DronePrediction.Prediction), inputColumnName: nameof(DroneData.agx),confidence: 95, pvalueHistoryLength: size / 5);
            var transformedModel = estimator.Fit(emptyDataView);

            // Prediction
            var transformedData = transformedModel.Transform(dataView);
            var predictions = mlContext.Data.CreateEnumerable<DronePrediction>(transformedData, reuseRowObject: false);

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

                //if (p.Prediction[0] == 1)
                //{
                //    Console.WriteLine("{0}\t{1:0.00}\t{2:0.00}", p.Prediction[0], p.Prediction[1], p.Prediction[2]);
                //}
            }
        }
    }
}
