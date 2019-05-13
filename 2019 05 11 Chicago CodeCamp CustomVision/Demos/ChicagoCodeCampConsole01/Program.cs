using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChicagoCodeCampConsole01
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
            // sample code goes here
        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            var fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            var binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }
    }
}
