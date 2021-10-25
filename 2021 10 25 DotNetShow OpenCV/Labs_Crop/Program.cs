using OpenCvSharp;

using var origImg = new Mat("MontTremblantBikes.jpg");


// resize
var newSize = new Size(640, 480);
Cv2.Resize(origImg, origImg, newSize);

var croppedImg = new Mat(origImg, new OpenCvSharp.Range(50, 250), new OpenCvSharp.Range(50, 250));

using (new Window("orig image", origImg))
using (new Window("final image", croppedImg))
{
    Cv2.WaitKey();
}
