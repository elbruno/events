using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DistractedDriverDetectionShared;

namespace DistractedDriverDetectionConsoleTest04
{
    public static class Program
    {
        private static int _predictionCount;
        private static long _predictionTotal;

        //const string Url = "[ .. you CV Url goes here ... >";
        const string Url = "http://127.0.0.1:8070/image";
        //const string Url = "http://192.168.137.169:80/image";

        public static void Main()
        {
            var imageFilePath = "c1-01.jpg";
            MakePredictionRequest(imageFilePath).Wait();

            imageFilePath = "c2-01.jpg";
            MakePredictionRequest(imageFilePath).Wait();

            imageFilePath = "c3-01.jpg";
            MakePredictionRequest(imageFilePath).Wait();

            imageFilePath = "c5-01.jpg";
            MakePredictionRequest(imageFilePath).Wait();

            imageFilePath = "c6-01.jpg";
            MakePredictionRequest(imageFilePath).Wait();

            // average process
            Console.WriteLine(@$"Average for {_predictionCount}: {_predictionTotal} milliseconds");

            Console.WriteLine("\n\nHit ENTER to exit...");
            Console.ReadLine();
        }

        public static async Task MakePredictionRequest(string imageFilePath)
        {
            var byteData = GetImageAsByteArray(imageFilePath);
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Prediction-Key", "[ .. you CV Key goes here ... >");

            // prediction
            var timer = new Stopwatch();
            timer.Start();
            using var content = new ByteArrayContent(byteData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            var response = await client.PostAsync(Url, content);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            timer.Stop();

            // Display results
            Console.WriteLine(@$"{imageFilePath}");
            var est = CustomVisionEstimation.FromJson(jsonResponse);
            var prediction = est.Predictions.OrderByDescending(x => x.Probability).FirstOrDefault();
            Console.WriteLine(@$"{prediction.TagName} - {prediction.Probability}");
            Console.WriteLine(@$"Elapsed Milliseconds {timer.ElapsedMilliseconds}");
            Console.WriteLine("");

            // average times
            _predictionCount++;
            if (_predictionTotal == 0)
                _predictionTotal = timer.ElapsedMilliseconds;
            else
                _predictionTotal = (_predictionTotal + timer.ElapsedMilliseconds) / 2;
        }

        private static byte[] GetImageAsByteArray(string imageFilePath)
        {
            FileStream fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            BinaryReader binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
    }
}