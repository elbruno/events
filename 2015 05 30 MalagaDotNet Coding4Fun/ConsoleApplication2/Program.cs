using System;
using System.IO;
using Microsoft.ProjectOxford.Face;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            string subscriptionKey = "4c138b4d82b947beb2e2926c92d1e514";
            var client = new Microsoft.ProjectOxford.Face.FaceServiceClient(subscriptionKey);

            GetFaces(client);

            Console.ReadLine();
        }

        private static async void GetFaces(FaceServiceClient client)
        {
            string imagePath = @"C:\SD\OneDrive\Event Materials\2015 05 30 MalagaDotNet Coding4Fun\Face Samples\princesas.jpg";
            using (var img = File.OpenRead(imagePath))
            {
                var faces = await client.DetectAsync(img, false, true, true);

                foreach (var face in faces)
                {
                    Console.WriteLine("age:" + face.Attributes.Age);
                    Console.WriteLine("gender:" + face.Attributes.Gender);
                }
            }
        }
    }
}
