using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;


namespace Labs_ListWebCams
{
    class OpenCVDeviceEnumerator
    {

        static void Main(string[] args)
        {

            for (int i = 0; i < 10; i++)
            {
                VideoCapture cap = new VideoCapture(i);  // open the camera

                string name;
                try
                {
                    name = cap.GetBackendName();
                }
                catch (Exception e)
                {
                    name = e.ToString();
                }

                string type;
                string name2;
                string ts;
                try
                {
                    type = cap.GetType().FullName;
                    name2 = cap.GetType().Name;
                    ts = cap.GetType().ToString();
                }
                catch (Exception e)
                {
                    type = e.ToString();
                    name2 = "";
                    ts = "";
                }

                var msg = $@"{i} - {name} - {type} - {name2} - {ts}";
                Console.WriteLine(msg); 
            }
        }
    }
}
