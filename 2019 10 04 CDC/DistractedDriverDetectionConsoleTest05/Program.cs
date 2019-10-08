using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DistractedDriverDetectionShared;

namespace DistractedDriverDetectionConsoleTest05
{
    public static class Program
    {
        private static int _predictionCount;
        private static long _predictionTotal;

        const string CVKey = "[ .. you CV Key goes here ... >";
        //const string CVUrl = "[ .. you CV Url goes here ... >";
        //const string CVUrl = "http://127.0.0.1:8070/image";
        const string CVUrl = "http://192.168.137.123:80/image";
        
        public static void Main()
        {
            var files = Directory.GetFiles("Images");
            var j = 0;
            for (var i = 0; i < 100; i++)
                foreach (var file in files)
                {
                    MakePredictionRequest(file).Wait();
                    j++;
                    Console.WriteLine($">> Processed File Number {i}-{j}");
                    Console.WriteLine();
                }

            // average process
            Console.WriteLine(@$"Average for {_predictionCount}: {_predictionTotal} milliseconds");

            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }

        public static async Task MakePredictionRequest(string imageFilePath)
        {
            try
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
                var prediction = est.Predictions.OrderByDescending(x => x.Probability).FirstOrDefault();
                Console.WriteLine(@$"{prediction.TagName} - {prediction.Probability}");

                // times and average times
                _predictionCount++;
                if (_predictionTotal == 0)
                    _predictionTotal = timer.ElapsedMilliseconds;
                else
                    _predictionTotal = (_predictionTotal + timer.ElapsedMilliseconds) / 2;

                Console.WriteLine(@$"  > Elapsed Milliseconds {timer.ElapsedMilliseconds}");
                Console.WriteLine(@$"  > Average Milliseconds {_predictionTotal}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}