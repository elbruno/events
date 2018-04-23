using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;

namespace CsHandWriting02
{
    class Program
    {
        const string SubscriptionKey = "<Computer Vision Key>";

        static void Main(string[] args)
        {
            AnalyzeImage(@"c:\Temp\demo02.png");
            Console.ReadLine();
        }

        private static async void AnalyzeImage(string filePath)
        {
            string url;
            var client = new VisionServiceClient(SubscriptionKey);
            using (Stream imageFileStream = File.OpenRead(filePath))
            {
                var resultHw = await client.CreateHandwritingRecognitionOperationAsync(imageFileStream);
                url = resultHw.Url;
            }

            var operation = new HandwritingRecognitionOperation {Url = url};
            var result = await client.GetHandwritingRecognitionOperationResultAsync(operation);

            foreach (var line in result.RecognitionResult.Lines)
            {
                foreach (var word in line.Words)
                {
                    Console.WriteLine(word.Text);
                }
            }
        }
    }
}
