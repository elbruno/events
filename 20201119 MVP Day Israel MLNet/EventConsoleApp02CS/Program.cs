﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EventConsoleApp02CS
{
    class Program
    {
        static readonly string subscriptionKey = "7eac144857254ad98dbfc625491c8084";
        static readonly string endpoint = "https://devnextanomalydetector.cognitiveservices.azure.com/";
        const string dataPath = "product-sales.json";
        const string latestPointDetectionUrl = "/anomalydetector/v1.0/timeseries/last/detect";
        const string batchDetectionUrl = "/anomalydetector/v1.0/timeseries/entire/detect";

        static void Main(string[] args)
        {
            var requestData = File.ReadAllText(dataPath);
            detectAnomaliesBatch(requestData);
            detectAnomaliesLatest(requestData);
            Console.WriteLine("Done at Reactor!");
            Console.ReadLine();
        }

        static void detectAnomaliesBatch(string requestData)
        {
            // Detect Anomalies Batch
            Console.WriteLine("Detecting anomalies as a batch");

            var result = Request(
                endpoint,
                batchDetectionUrl,
                subscriptionKey,
                requestData).Result;
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
            Console.WriteLine(jsonObj);

            if (jsonObj["code"] != null)
            {
                Console.WriteLine($"Detection failed. ErrorCode:{jsonObj["code"]}, ErrorMessage:{jsonObj["message"]}");
            }
            else
            {
                //Find and display the positions of anomalies in the data set
                bool[] anomalies = jsonObj["isAnomaly"].ToObject<bool[]>();
                Console.WriteLine("\nAnomalies detected in the following data positions:");
                for (var i = 0; i < anomalies.Length; i++)
                {
                    if (anomalies[i])
                    {
                        Console.Write(i + ", ");
                    }
                }
            }
        }

        static void detectAnomaliesLatest(string requestData)
        {
            // Detect Anomalies Last Point
            Console.WriteLine("\n\nDetermining if latest data point is an anomaly");
            var result = Request(
                endpoint,
                latestPointDetectionUrl,
                subscriptionKey,
                requestData).Result;
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
            Console.WriteLine(jsonObj);
        }

        static async Task<string> Request(string apiAddress, string endpoint, string subscriptionKey, string requestData)
        {
            using (HttpClient client = new HttpClient { BaseAddress = new Uri(apiAddress) })
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                var content = new StringContent(requestData, Encoding.UTF8, "application/json");
                var res = await client.PostAsync(endpoint, content);
                return await res.Content.ReadAsStringAsync();
            }
        }
    }
}
