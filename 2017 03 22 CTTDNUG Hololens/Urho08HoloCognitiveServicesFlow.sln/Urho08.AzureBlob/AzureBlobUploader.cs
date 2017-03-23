using System.Threading.Tasks;
using Windows.Storage;
using Microsoft.WindowsAzure.Storage;
using Urho08.Model;

namespace Urho08.AzureBlob
{
    public class AzureBlobUploader
    {
        public static async Task<string> UploadFiletoAzureBlobReturnUri(StorageFile file)
        {
            var storageAccount = CloudStorageAccount.Parse(Config.AzureBlobConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(Config.AzureBlobContainerName);
            var blockBlob = container.GetBlockBlobReference(file.Name);
            await blockBlob.UploadFromFileAsync(file);
            return Config.AzureBlobUri + file.Name;
        }
    }
}
