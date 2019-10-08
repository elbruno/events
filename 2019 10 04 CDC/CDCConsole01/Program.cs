using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CDCConsole01
{
    class Program
    {
        const string CVUrl = "http://192.168.137.169:80/image";
        const string CVKey = "[ .. you CV Key goes here ... >";

        static void Main(string[] args)
        {
            MakePredictionRequest("c3-01.jpg").Wait();

            Console.WriteLine("\n\nHit ENTER to exit...");
            Console.ReadLine();
        }

        private static async Task MakePredictionRequest(string imageFilePath)
        {
            var byteData = File.ReadAllBytes(imageFilePath);

            var client = new HttpClient();
            //client.DefaultRequestHeaders.Add("Prediction-Key", CVKey);

            using var content = new ByteArrayContent(byteData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            var response = await client.PostAsync(CVUrl, content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            var prettyJson = JToken.Parse(jsonResponse).ToString(Formatting.Indented);
            Console.WriteLine(prettyJson);
        }
    }
}
