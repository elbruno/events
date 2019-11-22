using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.AnomalyDetector;
using Microsoft.Azure.CognitiveServices.AnomalyDetector.Models;

namespace CSAnomalyDetector02
{
    class Program
    {
        static void Main(string[] args)
        {
            string endpoint = @"<Your endpoint goes here>";
            string key = "<Your key goes here>";
            string datapath = "request-data.csv";

            var client = new AnomalyDetectorClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint };

            // The request payload with points from the data file
            var list = File.ReadAllLines(datapath, Encoding.UTF8)
                .Where(e => e.Trim().Length != 0)
                .Select(e => e.Split(','))
                .Where(e => e.Length == 2)
                .Select(e => new Point(DateTime.Parse(e[0]), Double.Parse(e[1]))).ToList();
            var request = new Request(list, Granularity.Daily);

            EntireDetectSampleAsync(client, request, list).Wait(); // Async method for batch anomaly detection
            LastDetectSampleAsync(client, request).Wait(); // Async method for analyzing the latest data point in the set

            Console.WriteLine("\nPress ENTER to exit.");
            Console.ReadLine();
        }

        static async Task EntireDetectSampleAsync(IAnomalyDetectorClient client, Request request, List<Point> list)
        {
            Console.WriteLine("Detecting anomalies in the entire time series.");

            var result = await client.EntireDetectAsync(request).ConfigureAwait(false);

            if (result.IsAnomaly.Contains(true))
            {
                for (int i = 0; i < request.Series.Count; ++i)
                {
                    if (result.IsAnomaly[i])
                    {
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    Console.WriteLine("{0}\t{1}\t{2}", i, list[i].Timestamp,  list[i].Value);
                    Console.WriteLine("\tIs Anomaly:{0}\tIs Negative Anomaly:{1}\tIs Positive Anomaly:{2}", result.IsAnomaly[i], result.IsNegativeAnomaly[i], result.IsPositiveAnomaly[i]);
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine(" No anomalies detected in the series.");
            }
        }

        static async Task LastDetectSampleAsync(IAnomalyDetectorClient client, Request request)
        {

            Console.WriteLine("Detecting the anomaly status of the latest point in the series.");
            LastDetectResponse result = await client.LastDetectAsync(request).ConfigureAwait(false);

            if (result.IsAnomaly)
            {
                Console.WriteLine("The latest point was detected as an anomaly.");
            }
            else
            {
                Console.WriteLine("The latest point was not detected as an anomaly.");
            }
        }
    }
}
