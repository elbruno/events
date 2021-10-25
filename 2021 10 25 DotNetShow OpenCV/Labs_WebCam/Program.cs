using OpenCvSharp;

using var capture = new VideoCapture(1);
using var window = new Window("El Bruno - OpenCV WebCam");

var image = new Mat();
var newSize = new Size(640, 480);

var run = true;
while (run)
{
    // read camera frame
    capture.Read(image);
    if (image.Empty())
        break;
    Cv2.Resize(image, image, newSize);

    // show frame
    window.ShowImage(image);

    var key = (char)Cv2.WaitKey(10);
    switch ((char)Cv2.WaitKey(10))
    {
        case (char)27: // ESC
            run = false;
            break;
    }
}