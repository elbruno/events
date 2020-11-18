using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MugArCSAnomalyDetector01
{
    class Program
    {
        static readonly string subscriptionKey = "59b1b52f157c41c687e30d52b140c442";
        static readonly string endpoint = "https://anomalydetectormugar.cognitiveservices.azure.com/";
        const string dataPath = "product-sales.json";
        const string latestPointDetectionUrl = "/anomalydetector/v1.0/timeseries/last/detect";
        const string batchDetectionUrl = "/anomalydetector/v1.0/timeseries/entire/detect";

        static void Main(string[] args)
        {
            var requestData = File.ReadAllText(dataPath);
            //detectAnomaliesBatch(requestData);
            detectAnomaliesLatest(requestData);
            Console.WriteLine("\nPress any key to exit ");
            Console.ReadKey();
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

        static void detectAnomaliesBatch(string requestData)
        {
            Console.WriteLine("Detecting anomalies as a batch");

            dynamic jsonObjSource = Newtonsoft.Json.JsonConvert.DeserializeObject(requestData);
            var series = jsonObjSource["series"];

            //construct the request
            var result = Request(
                endpoint,
                batchDetectionUrl,
                subscriptionKey,
                requestData).Result;

            //deserialize the JSON object, and display it
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
            Console.WriteLine(jsonObj);

            if (jsonObj["code"] != null)
            {
                Console.WriteLine(
                    $"Detection failed. ErrorCode:{jsonObj["code"]}, ErrorMessage:{jsonObj["message"]}");
            }
            else
            {
                string output = $"index\tcurrentValue\tisAnomaly\tisNegativeAnomaly\tisPositiveAnomaly\texpectedValues\tlowerMargins\tupperMargins\tcalcLowerMargin\tcalcUpperMargin\r\n";
                Console.WriteLine(output);
                bool[] anomalies = jsonObj["isAnomaly"].ToObject<bool[]>();
                bool[] negativeAnomalies = jsonObj["isNegativeAnomaly"].ToObject<bool[]>();
                bool[] positiveAnomalies = jsonObj["isPositiveAnomaly"].ToObject<bool[]>();
                long[] expectedValues = jsonObj["expectedValues"].ToObject<long[]>();
                long[] lowerMargins = jsonObj["lowerMargins"].ToObject<long[]>();
                long[] upperMargins = jsonObj["upperMargins"].ToObject<long[]>();
                for (var i = 0; i < anomalies.Length; i++)
                {
                    var currentValue = series[i]["value"];
                    var lMar = expectedValues[i] - lowerMargins[i];
                    var uMar = expectedValues[i] + upperMargins[i];
                    string line = $"{i}\t{currentValue}\t{anomalies[i]}\t{negativeAnomalies[i]}\t{positiveAnomalies[i]}\t{expectedValues[i]}\t{lowerMargins[i]}\t{upperMargins[i]}\t{lMar}\t{uMar}\r\n";
                    Console.WriteLine(line);
                    output += line;
                }
                File.WriteAllText("output.tsv", output);
            }

        }

        static void detectAnomaliesLatest(string requestData)
        {
            Console.WriteLine("\n\nDetermining if latest data point is an anomaly");
            //construct the request
            var result = Request(
                endpoint,
                latestPointDetectionUrl,
                subscriptionKey,
                requestData).Result;

            //deserialize the JSON object, and display it
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(result);
            Console.WriteLine(jsonObj);
        }
    }
}