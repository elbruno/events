using OpenCvSharp;

using var window = new Window("El Bruno - OpenCV Video File");
var image = new Mat();
var newSize = new Size(640, 480);

using var capture = new VideoCapture(@"http://192.168.1.188:4747/videos");

var run = true;
while (run)
{

    // read camera frame
    capture.Read(image);
    if (image.Empty())
        break;
    Cv2.Resize(image, image, newSize);

    Cv2.PutText(image, "Hello from Canada", new Point(10, 20),
    HersheyFonts.HersheyComplexSmall, 1, Scalar.White);


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