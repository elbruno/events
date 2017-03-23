using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;
using Microsoft.ProjectOxford.Vision;
using Urho;
using Urho.Actions;
using Urho.Gui;
using Urho.Resources;
using Urho.Shapes;
using Urho.Urho2D;
using Urho08.Model;
using Color = Urho.Color;

namespace Urho08.HoloCognitiveServicesFlow.Processors
{
    public class ComputerVisionProcessor
    {
        
        public async Task<Tuple<string, StorageFile>> CaptureAndAnalyze(HoloCsFlowApp cognitiveServicesApp, MediaCapture mediaCapture, bool withPreview, bool readText = false)
        {
            var analysisResult = string.Empty;
            var imgFormat = ImageEncodingProperties.CreateJpeg();

            var file = await KnownFolders.CameraRoll.CreateFileAsync($"MCS_Photo{DateTime.Now:HH-mm-ss}.jpg", CreationCollisionOption.GenerateUniqueName);
            await mediaCapture.CapturePhotoToStorageFileAsync(imgFormat, file);
            await file.OpenStreamForReadAsync();

            // Capture a frame and put it to MemoryStream
            var memoryStream = new MemoryStream();
            using (var ras = new InMemoryRandomAccessStream())
            {
                await mediaCapture.CapturePhotoToStreamAsync(imgFormat, ras);
                ras.Seek(0);
                using (var stream = ras.AsStreamForRead())
                    stream.CopyTo(memoryStream);
            }

            var imageBytes = memoryStream.ToArray();
            memoryStream.Position = 0;

            Node childImagePreview = null;


            if (withPreview)
            {
                Application.InvokeOnMain(() =>
                {
                    var image = new Image();
                    image.Load(new Urho.MemoryBuffer(imageBytes));

                    childImagePreview = cognitiveServicesApp.Scene.CreateChild();
                    childImagePreview.Position = cognitiveServicesApp.LeftCamera.Node.WorldPosition + cognitiveServicesApp.LeftCamera.Node.WorldDirection * 2f;
                    childImagePreview.LookAt(cognitiveServicesApp.LeftCamera.Node.WorldPosition, Vector3.Up, TransformSpace.World);

                    childImagePreview.Scale = new Vector3(1f, image.Height / (float)image.Width, 0.1f) / 10;
                    var texture = new Texture2D();
                    texture.SetData(image, true);

                    var material = new Material();
                    material.SetTechnique(0, CoreAssets.Techniques.Diff, 0, 0);
                    material.SetTexture(TextureUnit.Diffuse, texture);

                    var box = childImagePreview.CreateComponent<Box>();
                    box.SetMaterial(material);

                    childImagePreview.RunActions(new EaseBounceOut(new ScaleBy(1f, 5)));
                });
            }

            try
            {
                var client = new VisionServiceClient(Config.VisionApiKey);
                if (readText)
                {
                    var ocrResult = await client.RecognizeTextAsync(memoryStream, detectOrientation: false);
                    var words = ocrResult.Regions.SelectMany(region => region.Lines).SelectMany(line => line.Words).Select(word => word.Text);
                    analysisResult = "it says: " + string.Join(" ", words);
                }
                else
                {
                    // just describe the picture, you can also use cleint.AnalyzeImageAsync method to get more info
                    var describeResult = await client.DescribeAsync(memoryStream);
                    analysisResult = describeResult?.Description?.Captions?.FirstOrDefault()?.Text;
                }
            }
            catch (ClientException exc)
            {
                analysisResult = exc?.Error?.Message ?? "Failed";
            }
            catch (Exception exc)
            {
                analysisResult = "Failed";
            }

            if (withPreview)
            {
                Application.InvokeOnMain(() =>
                {
                    // Display Results
                    if (childImagePreview != null)
                    {
                        var textNode = childImagePreview.CreateChild();
                        var text3D = textNode.CreateComponent<Text3D>();
                        text3D.HorizontalAlignment = HorizontalAlignment.Center;
                        text3D.VerticalAlignment = VerticalAlignment.Top;
                        text3D.ViewMask = 0x80000000; //hide from raycasts
                        text3D.Text = analysisResult;
                        text3D.SetFont(CoreAssets.Fonts.AnonymousPro, 22);
                        text3D.SetColor(Color.White);
                        textNode.Translate(new Vector3(0, 1f, -0.5f));
                        textNode.SetScale(0.5f);
                        textNode.Rotate(new Quaternion(0, 180, 0), TransformSpace.World);
                    }
                });
            }

            return new Tuple<string, StorageFile>(analysisResult, file);
        }
    }
}