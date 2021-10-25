using OpenCvSharp;

using var origImg = new Mat("img02.jpg"); 
using var effectImg = new Mat();
using var finalImg = new Mat();

// resize orig
var newSize = new Size(640, 480);
Cv2.Resize(origImg, origImg, newSize);


// apply effect
Cv2.Canny(origImg, effectImg, 50, 200);

using (new Window("orig image", origImg))
using (new Window("final image", finalImg))
{
    Cv2.WaitKey();
}
