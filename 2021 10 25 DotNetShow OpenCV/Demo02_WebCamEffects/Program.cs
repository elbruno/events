using System.Runtime.InteropServices.ComTypes;
using OpenCvSharp;
using Demo02_WebCamEffects;

var capture = new VideoCapture(1);
var window = new Window("El Bruno - OpenCVSharp Effects demo");
var image = new Mat();
var imageNew = new Mat();
var eff = new Effects();
bool run = true;

while (run)
{
    capture.Read(image);
    if (image.Empty()) break;
    imageNew = eff.ApplyEffect(image);
    window.ShowImage(imageNew);
    var key = (char)Cv2.WaitKey(100);
    if (key == (char)27) // Esc - Exit
        run = false;
    else
        eff.ApplyEffect(key);
}