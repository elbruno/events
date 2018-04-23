using System;
using System.IO;
using System.Net.Http;
using System.Xml;
using Newtonsoft.Json;

namespace ConsoleApp4
{
    class Program
    {
        const string ComputerVisionKey = "< Computer Vision Key goes here ... >";
        const string CVUriBase = "https://westus.api.cognitive.microsoft.com/vision/v1.0/";
        const string CVApiMethod = "analyze";
        const string FileSource = @"< Image File path ... >";

        static void Main(string[] args)
        {
            var imageFilePath = FileSource;
            MakeAnalysisRequest(imageFilePath);
            Console.ReadLine();
        }

        public static async void MakeAnalysisRequest(string imageFilePAth)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ComputerVisionKey);

            var requestParameters = "visualFeatures=Categories,Description,Color&language=en";
            var uri = CVUriBase + CVApiMethod + "?" + requestParameters;
            //string uri = CVUriBase + "?" + requestParameters;

            var byteData = GetImageAsByteArray(imageFilePAth);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                var response = await client.PostAsync(uri, content);
                var contentstring = await response.Content.ReadAsStringAsync();
                Console.WriteLine("\nResponse:\n");
                Console.WriteLine(contentstring);
                Console.WriteLine();
                //Console.WriteLine(CoolJsonFormat(contentstring));
            }
        }

        public static byte[] GetImageAsByteArray(string imageFilePath)
        {
            var fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            var binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        private static string CoolJsonFormat(string json)
        {
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Newtonsoft.Json.Formatting.Indented);
        }
    }
}

