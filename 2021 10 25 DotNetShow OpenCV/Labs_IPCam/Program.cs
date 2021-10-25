using OpenCvSharp;


using var window = new Window("El Bruno - OpenCV IPCam");

var image = new Mat();
var newSize = new Size(640, 480);

var ipCam = @"http://192.168.1.188:4747/video";

using var capture = new VideoCapture(ipCam);

var run = true;
while (run)
{
    // read camera frame
    capture.Read(image);
    if (image.Empty())
        break;
    Cv2.Resize(image, image, newSize);

    // add background
    window.ShowImage(image);

    var key = (char)Cv2.WaitKey(10);
    switch ((char)Cv2.WaitKey(10))
    {
        case (char)27: // ESC
            run = false;
            break;
    }
}