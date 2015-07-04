using System;
using System.Diagnostics;
using System.Linq;
using USBHIDDRIVER;

namespace ElBruno.LightNotifier
{
    public class LightController
    {
        private readonly byte[] _cmdData = new byte[] { 0, 0, 0, 0, 0, 0 };
        private readonly USBInterface _device;
        private readonly bool _connected; 

        public LightController()
        {
            const string vendorId = "vid_1294"; 
            const string productId = "pid_1320"; 
            _device = new USBInterface(vendorId, productId);

            if (_connected)
            {
                return;
            }
            _device.enableUsbBufferEvent(UsbDeviceEventCacher);
            _connected = true;
        }


        public void TurnLight(bool lightOn)
        {
            _cmdData[1] = 0;
            if (lightOn)
            {
                _cmdData[1] = 1;
            }
            _device.UsbDevice.writeDataSimple(_cmdData);
        }

        private void UsbDeviceEventCacher(object sender, EventArgs e)
        {
            if (USBInterface.usbBuffer.Count <= 0) return;
            const int counter = 0;
            while (USBInterface.usbBuffer[counter] == null)
            {
                lock (USBInterface.usbBuffer.SyncRoot)
                {
                    USBInterface.usbBuffer.RemoveAt(0);
                }
            }
            var currentRecord = (byte[])USBInterface.usbBuffer[0];
            lock (USBInterface.usbBuffer.SyncRoot)
            {
                USBInterface.usbBuffer.RemoveAt(0);
            }
            if (currentRecord == null) return;
            var msg = currentRecord.Aggregate("current record:", (current, t) => current + t);
            msg += "\r\n";
            Trace.WriteLine(msg);
        }
    }
}
