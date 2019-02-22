﻿using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Formatting = Newtonsoft.Json.Formatting;

namespace CustomVisionMarvelConsoleDocker01
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
            var url = "http://127.0.0.1:8080/image";
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