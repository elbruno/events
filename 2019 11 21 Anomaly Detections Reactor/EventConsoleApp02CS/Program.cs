using System;
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
        static readonly string subscriptionKey = "Ac2dd639ea144e30980b5a5e895b8ff4";
        static readonly string endpoint = "https://tecanomalydetectionlabs.cognitiveservices.azure.com/";
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
        }

        static void detectAnomaliesLatest(string requestData)
        {
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
