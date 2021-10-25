using System;
using OpenCvSharp;

var videoFile = "01.mp4";
System.IO.Directory.CreateDirectory("frames");

var capture = new VideoCapture(videoFile);
var window = new Window("El Bruno - OpenCVSharp Save Video Frame by Frame");
var image = new Mat();

var i = 0;
while (capture.IsOpened())
{
    capture.Read(image);
    if (image.Empty())
        break;

    i++;
    var imgNumber = i.ToString().PadLeft(8, '0');

    var frameImageFileName = $@"frames\image{imgNumber}.png";
    Cv2.ImWrite(frameImageFileName, image);

    window.ShowImage(image);
    if (Cv2.WaitKey(1) == 113) // Q
        break;
}

Console.WriteLine("Complete !");