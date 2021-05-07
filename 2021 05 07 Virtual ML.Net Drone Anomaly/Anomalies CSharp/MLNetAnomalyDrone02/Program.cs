using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using SpikeDetection.DataStructures;

namespace MLNetAnomalyDrone02
{
    class Program
    {
        static void Main(string[] args)
        {
            // output data
            var droneTelemetryCsv = "dronedatamlnet.csv";
            var droneTelemetryAnomaliesCsv = "dronedataanomaliesmlnet.csv";
            var droneTelemetryLines = File.ReadAllLines(droneTelemetryCsv);

            // define vars
            var mlContext = new MLContext();
            var size = droneTelemetryLines.Count() - 1;
            
            // load data
            var dataView = mlContext.Data.LoadFromTextFile<DroneData>(path: droneTelemetryCsv, hasHeader: true, separatorChar: ',');
            var droneTelemetryList = new List<DroneData>();
            var emptyDataView = mlContext.Data.LoadFromEnumerable(droneTelemetryList);

            // Create Estimator and build model
            var estimator = mlContext.Transforms.DetectChangePointBySsa(outputColumnName: nameof(DronePrediction.Prediction), inputColumnName: nameof(DroneData.agx), 
                confidence: 85, changeHistoryLength: size / 4);
            
            var transformedModel = estimator.Fit(emptyDataView);

            // Prediction
            var transformedData = transformedModel.Transform(dataView);
            var predictions = mlContext.Data.CreateEnumerable<DronePrediction>(transformedData, reuseRowObject: false);

            // analyze predictions using Spike Detection
            var i = 0;
            var droneTelemetryAnalysis = $"{droneTelemetryLines[i]},Alert,Score,P-Value,{Environment.NewLine}";
            Console.Write(droneTelemetryAnalysis);
            foreach (var pred in predictions)
            {
                var droneTelemetryLine = string.Format("{0},{1},{2:0.00},{3:0.00}, {4}", droneTelemetryLines[i + 1], pred.Prediction[0], pred.Prediction[1], pred.Prediction[2], Environment.NewLine); ;
                droneTelemetryAnalysis += droneTelemetryLine;
                i++;

                if (pred.Prediction[0] == 1)
                {
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                Console.Write(droneTelemetryLine);
                Console.ResetColor();
            }

            File.WriteAllText(droneTelemetryAnomaliesCsv, droneTelemetryAnalysis);
        }
    }
}
