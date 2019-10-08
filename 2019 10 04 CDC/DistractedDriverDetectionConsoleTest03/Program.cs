using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DistractedDriverDetectionShared;

namespace DistractedDriverDetectionConsoleTest03
{
    public static class Program
    {
        const string Url = "http://127.0.0.1:8080/image";

        public static void Main()
        {
            var imageFilePath = "c1-01.jpg";
            MakePredictionRequest(imageFilePath).Wait();

            imageFilePath = "c2-01.jpg";
            MakePredictionRequest(imageFilePath).Wait();

            imageFilePath = "c3-01.jpg";
            MakePredictionRequest(imageFilePath).Wait();

            imageFilePath = "c5-03.jpg";
            MakePredictionRequest(imageFilePath).Wait();

            imageFilePath = "c6-01.jpg";
            MakePredictionRequest(imageFilePath).Wait();

            Console.WriteLine("\n\nHit ENTER to exit...");
            Console.ReadLine();
        }

        public static async Task MakePredictionRequest(string imageFilePath)
        {
            var client = new HttpClient();
            var byteData = GetImageAsByteArray(imageFilePath);

            // prediction
            using var content = new ByteArrayContent(byteData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            var response = await client.PostAsync(Url, content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            // Display results
            Console.WriteLine(@$"{imageFilePath}");
            var est = CustomVisionEstimation.FromJson(jsonResponse);
            var predictionsSorted = est.Predictions.OrderByDescending(x => x.Probability);
            foreach (var prediction in predictionsSorted)
                Console.WriteLine(@$"{prediction.TagName} - {prediction.Probability}");
            Console.WriteLine("");
        }

        private static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
    }
}