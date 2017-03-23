using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;
using Urho08.AzureBlob;
using Urho08.Model;

namespace Urho08.MsFlow
{
    public class FlowProcessor
    {
        public async Task FlowMessage(StorageFile file, string analysisResult, bool readText, bool completeAnalysis = false)
        {
            var flowNotificationMessage = new FlowMessage
            {
                HoloDescription = analysisResult,
                HoloReadText = readText.ToString(),
                HoloImage = GetDefaultImage()
            };

            if (file != null)
            {
                if (completeAnalysis)
                    await PerformCompleteImageAnalysis(file, flowNotificationMessage);

                try
                {
                    var uriName = await AzureBlobUploader.UploadFiletoAzureBlobReturnUri(file);
                    flowNotificationMessage.HoloImage = uriName;
                    Debug.WriteLine(flowNotificationMessage.HoloImage);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }

            var jsonBody = Newtonsoft.Json.JsonConvert.SerializeObject(flowNotificationMessage);
            var client = new HttpClient();
            HttpContent contentPost = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            await client.PostAsync(new Uri(Config.FlowUrl), contentPost).ContinueWith((postTask) => postTask.Result.EnsureSuccessStatusCode());
        }



        private string GetDefaultImage()
        {
            var rnd = new Random();
            var iFile = rnd.Next(1, 3);
            var fileName = $@"HololensMeme{iFile}.jpg";
            var file = Config.AzureBlobUri + fileName;
            return file;
        }

        public async Task PerformCompleteImageAnalysis(StorageFile file, FlowMessage flowNotificationMessage)
        {
            try
            {

                AnalysisResult result;
                await Task.Run(async () =>
                {
                    Task.Yield();

                    using (var fileStreamImage = File.OpenRead(file.Path))
                    {
                        var vsClient = new VisionServiceClient(Config.VisionApiKey);
                        VisualFeature[] visualFeatures = {
                            VisualFeature.Categories, VisualFeature.Description, VisualFeature.Faces, VisualFeature.Tags
                        };
                        result = await vsClient.AnalyzeImageAsync(fileStreamImage, visualFeatures);
                    }

                    flowNotificationMessage.FaceCount = result.Faces?.Length.ToString();
                    flowNotificationMessage.CategoriesCount = result.Categories?.Length.ToString();
                    flowNotificationMessage.TagCount = result.Tags?.Length.ToString();

                    if (result.Categories?.Length > 0)
                    {
                        foreach (var category in result.Categories)
                        {
                            flowNotificationMessage.Categories =
                                $"{category.Name}, {category.Detail}, {category.Score};{Environment.NewLine}";
                        }
                    }

                    if (result.Faces?.Length > 0)
                    {
                        foreach (var face in result.Faces)
                        {
                            flowNotificationMessage.Faces = $"{face.Age}, {face.Gender};{Environment.NewLine}";
                        }
                    }

                    if (result.Tags?.Length > 0)
                    {
                        foreach (var tag in result.Tags)
                        {
                            flowNotificationMessage.Tags = $"{tag.Name}, {tag.Confidence}, {tag.Hint};{Environment.NewLine}";
                        }
                    }
                });
            }
            catch (Exception exception)
            {
                flowNotificationMessage.Exception = exception.ToString();
            }
        }
    }
}