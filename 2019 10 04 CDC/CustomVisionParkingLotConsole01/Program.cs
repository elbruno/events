using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DistractedDriverDetectionShared;

namespace CustomVisionParkingLotConsole01
{
    public static class Program
    {
        private static int _predictionCount;
        private static long _predictionTotal;

        //const string CVUrl = "[ .. you CV Url goes here ... >";
        const string CVUrl = "http://127.0.0.1:8071/image";
        //const string CVUrl = "http://10.184.1.231:8080/image";
        const string CVKey = "[ .. you CV Key goes here ... >";

        public static void Main()
        {
            var files = Directory.GetFiles("Images");
            var j = 0;
            for (var i = 0; i < 100; i++)
                foreach (var file in files)
                {
                    MakePredictionRequest(file).Wait();
                    j++;
                    Console.WriteLine($"  >> Processed File Number {i}-{j}");
                    Console.WriteLine("");
                }

            // average process
            Console.WriteLine(@$"Average for {_predictionCount}: {_predictionTotal} milliseconds");

            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }

        public static async Task MakePredictionRequest(string imageFilePath)
        {
            var fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            var binaryReader = new BinaryReader(fileStream);
            var byteData = binaryReader.ReadBytes((int)fileStream.Length);
            
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Prediction-Key", CVKey);

            // prediction
            var timer = new Stopwatch();
            timer.Start();
            using var content = new ByteArrayContent(byteData);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            var response = await client.PostAsync(CVUrl, content);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            timer.Stop();

            // Display results
            Console.WriteLine(@$"{imageFilePath}");
            var est = CustomVisionEstimation.FromJson(jsonResponse);
            var predictions = est.Predictions.OrderByDescending(x => x.Probability);
            foreach (var pred in predictions)
                Console.WriteLine(@$"  >> {pred.TagName} - {pred.Probability}");

            // average times
            _predictionCount++;
            if (_predictionTotal == 0)
                _predictionTotal = timer.ElapsedMilliseconds;
            else
                _predictionTotal = (_predictionTotal + timer.ElapsedMilliseconds) / 2;

            Console.WriteLine(@$"Elapsed Milliseconds {timer.ElapsedMilliseconds}");
            Console.WriteLine(@$"Average Milliseconds {_predictionTotal}");
        }
    }
}