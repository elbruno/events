using OpenCvSharp;

var image = new Mat("02.jpg");

// resize
var newSize = new Size(640, 480);
Cv2.Resize(image, image, newSize);

// effect
var imageNew = new Mat();
Cv2.Canny(image, imageNew, 50, 200);

using (new Window("El Bruno - Show Image", imageNew))
{
    Cv2.WaitKey();
}
