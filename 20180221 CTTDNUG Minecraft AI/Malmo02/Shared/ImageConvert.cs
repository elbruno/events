using Microsoft.Research.Malmo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace MalmoCSharpLauncher.Shared
{
    // This converts a Malmo ByteVector to a Bitmap
    class ImageConvert
    {
        private static readonly ImageConverter _imageConverter = new ImageConverter();
        public static Bitmap GetImageFromByteArray(ByteVector objByteVector)
        {
            int width = 320;
            int height = 240;

            // Convert the Malmo ByteVector to a byteArray
            byte[] byteArray = objByteVector.ToArray();

            // Create a Bitmap 
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            int pos = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    bmp.SetPixel(x, y, Color.FromArgb(byteArray[pos], byteArray[pos + 1], byteArray[pos + 2]));
                    pos += 3;
                }  
            }

            // Return the Bitmap
            return bmp;
        }
    }
}
