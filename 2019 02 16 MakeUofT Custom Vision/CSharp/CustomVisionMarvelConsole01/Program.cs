using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CustomVisionMarvelConsole01
{
    static class Program
    {
        static void Main()
        {
            MakePredictionRequest("IMG01.jpg").Wait();
            Console.ReadLine();
        }

        static async Task MakePredictionRequest(string imageFilePath)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Prediction-Key", "<Custom Vision Prediction Key>");
            var url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v2.0/Prediction/<Custom Vision AppKey>/image?iterationId=<Custom Vision IterationId>";
            var byteData = GetImageAsByteArray(imageFilePath);

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