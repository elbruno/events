using OpenCvSharp;

using var capture = new VideoCapture(1);
using var window = new Window("El Bruno - OpenCVSharp demo");

var image = new Mat();

// background
bool showBg = false;
var background = new Mat("avaWaves.png");
var newSize = new Size(640, 480);
Cv2.Resize(background, background, newSize);


var run = true;
while (run)
{
    // read camera frame
    capture.Read(image);
    if (image.Empty())
        break;
    Cv2.Resize(image, image, newSize);

    // add background
    if (showBg)
        Cv2.AddWeighted(background, 0.5, image, 0.5, 0, image);
    window.ShowImage(image);

    var key = (char)Cv2.WaitKey(10);
    switch ((char)Cv2.WaitKey(10))
    {
        case (char)27: // ESC
            run = false;
            break;
        case 'b':
            showBg = !showBg;
            break;
    }
}