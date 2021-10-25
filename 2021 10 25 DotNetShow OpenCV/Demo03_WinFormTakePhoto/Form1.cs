using System;
using System.Windows.Forms;
using OpenCvSharp;

namespace Demo03_WinForm
{
    public partial class Form1 : Form
    {
        bool run = true;
        private VideoCapture capture;
        private Mat image;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            capture = new VideoCapture(1);
            image = new Mat();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            run = true;
            ReadCameraAndShow();
        }

        private bool ReadCameraAndShow()
        {
            capture.Read(image);
            if (image.Empty()) return true;
            var newImage = new Mat();
            Cv2.Canny(image, newImage, 50, 200);
            var dd = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(newImage);
            pictureBox1.Image = dd;
            return false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            run = false;
        }
    }
}
