using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json.Linq;
using Formatting = Newtonsoft.Json.Formatting;

namespace CodeMashConsole01
{
    class Program
    {
        static void Main(string[] args)
        {
            MakePred("IMG_20190104_114712.jpg");
            Console.ReadLine();
        }

        static async void MakePred(string imageFilePath)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Prediction-Key", "<Custom Vision Prediction Key>");
            var url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v2.0/Prediction/<Custom Vision AppId>/image?iterationId=<Custom Vision Iteration Id>";
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
