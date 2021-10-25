using OpenCvSharp;

using var origImg = new Mat("img02.jpg"); 
using var effectImg = new Mat();
using var finalImg = new Mat();

// apply effect
Cv2.Canny(origImg, effectImg, 50, 200);

// resize
var newSize = new Size(640, 480);
Cv2.Resize(effectImg, finalImg, newSize);

// add text
Cv2.PutText(finalImg, "Hello from Canada", new Point(10, 20), HersheyFonts.HersheyComplexSmall, 1, Scalar.White);

using (new Window("orig image", origImg))
using (new Window("final image", finalImg))
{
    Cv2.WaitKey();
}
