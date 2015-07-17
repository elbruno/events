using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace USBHIDDRIVER.TESTS
{

    [TestFixture]
    public class USBTestFixture
    {
        USBHIDDRIVER.USBInterface usbI = new USBInterface("vid_0b6a", "pid_0022");

        /// <summary>
        /// Test if the device list works.
        /// </summary>
        [Test]
        public void deviceList()
        {
            USBHIDDRIVER.USBInterface usbI = new USBInterface("0");
            String[] list = usbI.getDeviceList();
            Assert.IsNotNull(list);
        }

        /// <summary>
        /// sends a start command
        /// </summary>
        [Test]
        public void sendStartCMD()
        {
            byte[] startCMD = new byte[8];
            //Start
            startCMD[0] = 255;
            //Mode
            startCMD[1] = 0;
            //USync
            startCMD[2] = 28;
            //ULine
            startCMD[3] = 20;
            //tSync
            startCMD[4] = 20;
            //tRepeat - High
            startCMD[5] = 0;
            //tRepeat - Low
            startCMD[6] = 0x01;
            //BusMode
            startCMD[7] = 0xF4;
            //send the command
            
            Assert.IsTrue(usbI.Connect());
            Assert.IsTrue(usbI.write(startCMD));
        }

        /// <summary>
        /// Starts the read.
        /// </summary>
        [Test]
        public void startRead()
        {
              
            sendStartCMD();
            usbI.enableUsbBufferEvent(new System.EventHandler(myEventCacher));
            Thread.Sleep(5);
            usbI.startRead();
            Thread.Sleep(5);
            for (int i = 0; i < 200; i++)
            {
                Assert.IsNotNull(USBHIDDRIVER.USBInterface.usbBuffer);
                Thread.Sleep(2);
            }
            usbI.stopRead();
            sendStopCMD(); 
        }

        /// <summary>
        /// Sends the stop command.
        /// </summary>
        [Test]
        public void sendStopCMD()
        {
            byte[] stopCMD = new byte[75];
            //Stop
            stopCMD[0] = 128;

            stopCMD[64] = 8;
            
            Assert.IsTrue(usbI.write(stopCMD));
        }

        /// <summary>
        /// Tests Users the definedevent handling.
        /// </summary>
        [Test]
        public void userDefinedeventHandling()
        {
            sendStartCMD();

            usbI.enableUsbBufferEvent(new System.EventHandler(myEventCacher));
            usbI.startRead();

            //wait a little bit
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(4);
            }

            sendStopCMD();
        }

        /// <summary>
        /// The event cacher.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public void myEventCacher(object sender, System.EventArgs e)
        {
            Console.Out.WriteLine("Event caught");
            if (USBHIDDRIVER.USBInterface.usbBuffer.Count > 0)
            {
                byte[] currentRecord = null;
                int counter = 0;
                while ((byte[])USBHIDDRIVER.USBInterface.usbBuffer[counter] == null)
                {
                    //Remove this report from list
                    lock (USBHIDDRIVER.USBInterface.usbBuffer.SyncRoot)
                    {
                        USBHIDDRIVER.USBInterface.usbBuffer.RemoveAt(0);
                    }
                }
                //since the remove statement at the end of the loop take the first element
                currentRecord = (byte[])USBHIDDRIVER.USBInterface.usbBuffer[0];
                lock (USBHIDDRIVER.USBInterface.usbBuffer.SyncRoot)
                {
                    USBHIDDRIVER.USBInterface.usbBuffer.RemoveAt(0);
                }

                //DO SOMETHING WITH THE RECORD HERE
            }
        }
    }
}
