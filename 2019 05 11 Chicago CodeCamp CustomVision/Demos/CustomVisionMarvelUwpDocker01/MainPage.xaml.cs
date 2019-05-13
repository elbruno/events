using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using Windows.Media;

namespace CustomVisionMarvelUwpDocker01
{
    [ComImport]
    [Guid("5B0D3235-4DBA-4D44-865E-8F1D0E4FD04D")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    unsafe interface IMemoryBufferByteAccess
    {
        void GetBuffer(out byte* buffer, out uint capacity);
    }

    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            await CameraPreview.StartAsync();
            CameraPreview.CameraHelper.FrameArrived += CameraHelper_FrameArrived;
        }


        private Stopwatch _stopwatch;
        private async void CameraHelper_FrameArrived(object sender, Microsoft.Toolkit.Uwp.Helpers.FrameEventArgs e)
        {
            if (e.VideoFrame.SoftwareBitmap == null) return;
            try
            {
                string prettyJson = string.Empty;
                _stopwatch = Stopwatch.StartNew();

                var sb = e.VideoFrame.SoftwareBitmap;

                var newVf = new VideoFrame(BitmapPixelFormat.Rgba8, sb.PixelWidth, sb.PixelHeight);
                await e.VideoFrame.CopyToAsync(newVf);

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        var client = new HttpClient();
                        var url = "http://127.0.0.1:8080/image";
                        var tempPath = string.Empty;

                        //var storageFolder = ApplicationData.Current.LocalFolder;
                        //var of = await storageFolder.CreateFileAsync("cameraframe.jpg", CreationCollisionOption.ReplaceExisting);
                        //SaveSoftwareBitmapToFile(sb, of);
                        //tempPath = of.Path;
                        //var byteData = GetImageAsByteArray(tempPath);

                        var byteData = ConvertFrameToByteArray(newVf.SoftwareBitmap);

                        using (var content = new ByteArrayContent(byteData))
                        {
                            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                            var response = await client.PostAsync(url, content);
                            var jsonResponse = await response.Content.ReadAsStringAsync();
                            prettyJson = JToken.Parse(jsonResponse).ToString(Formatting.Indented);
                            Debug.WriteLine(prettyJson);
                        }
                    });

                _stopwatch.Stop();

                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    var message = $"{DateTime.Now.ToLongTimeString()} - {1000f / _stopwatch.ElapsedMilliseconds,4:f1} fps";
                    TextBlockResults.Text = message; ;
                });
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }

        static byte[] GetImageAsByteArray(string imageFilePath)
        {
            var fileStream = new FileStream(imageFilePath, FileMode.Open, FileAccess.Read);
            var binaryReader = new BinaryReader(fileStream);
            return binaryReader.ReadBytes((int)fileStream.Length);
        }

        private async void SaveSoftwareBitmapToFile(SoftwareBitmap softwareBitmap, StorageFile outputFile)
        {
            using (var stream = await outputFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                encoder.SetSoftwareBitmap(softwareBitmap);
                encoder.BitmapTransform.ScaledWidth = (uint)softwareBitmap.PixelWidth;
                encoder.BitmapTransform.ScaledHeight = (uint)softwareBitmap.PixelHeight;
                await encoder.FlushAsync();
            }
        }

        private byte[] ConvertFrameToByteArray(SoftwareBitmap bitmap)
        {
            byte[] bytes;
            WriteableBitmap newBitmap = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight);
            bitmap.CopyToBuffer(newBitmap.PixelBuffer);
            using (Stream stream = newBitmap.PixelBuffer.AsStream())
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }
            return bytes;
        }
    }
}
