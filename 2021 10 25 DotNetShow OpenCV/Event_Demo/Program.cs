using OpenCvSharp;

var image = new Mat(@"https://github.com/elbruno.png");
var imageNew = new Mat();

var newSize = new Size(640, 480);
Cv2.Resize(image, imageNew, newSize);

Cv2.PutText(imageNew, "Hello from Canada", new Point(10, 20), 
    HersheyFonts.HersheyComplexSmall, 1, Scalar.White);

var imageEffect = new Mat();
Cv2.Canny(imageNew, imageEffect, 50, 200);

using (new Window("El Bruno - Show Image", imageNew))
using (new Window("El Bruno - Show Image Effect", imageEffect))
{
    Cv2.WaitKey();
}