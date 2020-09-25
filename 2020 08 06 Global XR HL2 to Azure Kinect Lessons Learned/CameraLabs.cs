using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Windows.WebCam;


public class CameraLabs : MonoBehaviour
{
    public TextMesh MessageText;
    public Component LabelOnlyToolTip;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!_processAllFrames) return;
        if (_isProcessing) return;
        TakePhoto();
    }

    public void StartCameraProcessing()
    {
        _processAllFrames = true;
        _isProcessing = false;
    }

    public void StopCameraProcessing()
    {
        _processAllFrames = false;
        _isProcessing = false;
    }

    PhotoCapture photoCaptureObject = null;
    private string _body;
    private long _processedFrames = 0;
    private bool _processAllFrames = false;
    private bool _isProcessing = false;
    private TextMeshProUGUI _labelTooltip;

    public void TakePhoto()
    {
        _isProcessing = true;
        PhotoCapture.CreateAsync(false, OnPhotoCaptureCreated);
    }

    void OnPhotoCaptureCreated(PhotoCapture captureObject)
    {
        _isProcessing = true;
        photoCaptureObject = captureObject;

        var cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();

        var cameraParameters = new CameraParameters();
        cameraParameters.hologramOpacity = 0.0f;
        cameraParameters.cameraResolutionWidth = cameraResolution.width;
        cameraParameters.cameraResolutionHeight = cameraResolution.height;
        cameraParameters.pixelFormat = CapturePixelFormat.JPEG; // .BGRA32;

        captureObject.StartPhotoModeAsync(cameraParameters, OnPhotoModeStarted);
    }

    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
    {
        _isProcessing = false;
        photoCaptureObject.Dispose();
        photoCaptureObject = null;
    }

    private void OnPhotoModeStarted(PhotoCapture.PhotoCaptureResult result)
    {
        if (result.success)
        {
            // no disk, photo captured in memory
            photoCaptureObject.TakePhotoAsync(OnCapturedPhotoToMemory);
        }
        else
        {
            Debug.LogError("Unable to start photo mode!");
        }
    }

    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
    {
        if (result.success)
        {
            try
            {
                var imageBufferList = new List<byte>();
                photoCaptureFrame.CopyRawImageDataIntoBuffer(imageBufferList);

                _body = string.Empty;
                _body = DetectImage(imageBufferList.ToArray());
                var bestPred = GetBestPrediction(_body);
                DisplayMessage(bestPred);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
    }

    void DisplayMessage(string message)
    {
        if (MessageText is null) return;
        MessageText.text = message;

        if (_labelTooltip is null)
        {
            var lblttmsg = GameObject.Find("LabelTTMsg");
            _labelTooltip = lblttmsg.GetComponentInChildren<TextMeshProUGUI>();
        }
        if (_labelTooltip != null)
            _labelTooltip.text = message;
    }

    string DetectImage(byte[] image)
    {
        var imageUrl = "http://192.168.1.157:8090/image";
        string body = string.Empty;

        using (var request = UnityWebRequest.Post(imageUrl, ""))
        {
            request.SetRequestHeader("Content-Type", "application/octet-stream");
            request.uploadHandler = new UploadHandlerRaw(image);
            request.SendWebRequest();

            while (request.isDone == false)
            {
                var wfs = new WaitForSeconds(1);
            }

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log(request.error);
            }
            else
            {
                body = request.downloadHandler.text;
            }
        }
        return body;
    }

    string GetBestPrediction(string body)
    {
        _processedFrames++;
        var bestPrediction = $"{_processedFrames} - No predictions";
        if (string.IsNullOrEmpty(body)) return bestPrediction;

        var jsonBody = JsonConvert.DeserializeObject<ResponseRoot>(body);
        var sortedPreds =
            from pred in jsonBody.predictions
            orderby pred.probability descending
            select pred;
        var topResult = sortedPreds.FirstOrDefault();
        if (topResult != null)
            bestPrediction = $"{_processedFrames} - {topResult.tagName} - {topResult.probability}";

        return bestPrediction;
    }
}