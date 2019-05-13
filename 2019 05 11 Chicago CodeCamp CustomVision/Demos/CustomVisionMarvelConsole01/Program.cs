using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CustomVisionMarvelConsole01
{
    class Program
    {
        static void Main(string[] args)
        {
            MakePrediction();

            Console.ReadLine();
        }

        private static async Task MakePrediction()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Prediction-Key", "0b30992ecaa3487f92819e3028436848");
            var url =
                "https://southcentralus.api.cognitive.microsoft.com/customvision/v3.0/Prediction/8e062004-8f3b-4108-b838-ba3ae840314c/detect/iterations/Iteration7/image";

            var byteData = GetImageAsByteArray("IMG02.jpg");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                var response = await client.PostAsync(url, content);
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var prettyJson = JToken.Parse(jsonResponse).ToString(Formatting.Indented);
                Console.WriteLine(prettyJson);
            }
        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            var fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            var binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
    }
}
