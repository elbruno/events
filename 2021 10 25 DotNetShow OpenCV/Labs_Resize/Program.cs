using OpenCvSharp;

using var origImg = new Mat("img02.jpg");
using var finalImg = new Mat();

// resize
var newSize = new Size(640, 480);
Cv2.Resize(origImg, finalImg, newSize);

using (new Window("orig image", origImg))
using (new Window("final image", finalImg))
{
    Cv2.WaitKey();
}
