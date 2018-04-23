using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace CsHandWriting01
{
    static class Program
    {
        const string SubscriptionKey = "<Computer Vision Key>";
        const string UriBase = "https://westus.api.cognitive.microsoft.com/vision/v1.0/recognizeText";
        const string ImageFilePath = @"c:\Temp\demo02.png";

        static void Main()
        {
            Console.WriteLine("Handwriting Recognition:");
            Console.Write("Enter the path to an image with handwritten text you wish to read: ");
            ReadHandwrittenText(ImageFilePath);

            Console.WriteLine("\nPlease wait a moment for the results to appear. Then, press Enter to exit...\n");
            Console.ReadLine();
        }

        static async void ReadHandwrittenText(string imageFilePath)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", SubscriptionKey);
            var requestParameters = "handwriting=true";
            var uri = UriBase + "?" + requestParameters;
            HttpResponseMessage response = null;
            string operationLocation = null;

            var byteData = GetImageAsByteArray(imageFilePath);
            var content = new ByteArrayContent(byteData);

            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            response = await client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
                operationLocation = response.Headers.GetValues("Operation-Location").FirstOrDefault();
            else
            {
                Console.WriteLine("\nError:\n");
                Console.WriteLine(JsonPrettyPrint(await response.Content.ReadAsStringAsync()));
                return;
            }

            string contentString;
            var i = 0;
            do
            {
                System.Threading.Thread.Sleep(1000);
                response = await client.GetAsync(operationLocation);
                contentString = await response.Content.ReadAsStringAsync();
                ++i;
            }
            while (i < 10 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1);
            if (i == 10 && contentString.IndexOf("\"status\":\"Succeeded\"") == -1)
            {
                Console.WriteLine("\nTimeout error.\n");
                return;
            }

            Console.WriteLine("\nResponse:\n");
            Console.WriteLine(JsonPrettyPrint(contentString));
        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            var fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            var binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
        
        static string JsonPrettyPrint(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            json = json.Replace(Environment.NewLine, "").Replace("\t", "");

            var INDENT_STRING = "    ";
            var indent = 0;
            var quoted = false;
            var sb = new StringBuilder();
            for (var i = 0; i < json.Length; i++)
            {
                var ch = json[i];
                switch (ch)
                {
                    case '{':
                    case '[':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, ++indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        break;
                    case '}':
                    case ']':
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, --indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        sb.Append(ch);
                        break;
                    case '"':
                        sb.Append(ch);
                        var escaped = false;
                        var index = i;
                        while (index > 0 && json[--index] == '\\')
                            escaped = !escaped;
                        if (!escaped)
                            quoted = !quoted;
                        break;
                    case ',':
                        sb.Append(ch);
                        if (!quoted)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, indent).ForEach(item => sb.Append(INDENT_STRING));
                        }
                        break;
                    case ':':
                        sb.Append(ch);
                        if (!quoted)
                            sb.Append(" ");
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }
    }
    static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var i in ie)
            {
                action(i);
            }
        }
    }
}