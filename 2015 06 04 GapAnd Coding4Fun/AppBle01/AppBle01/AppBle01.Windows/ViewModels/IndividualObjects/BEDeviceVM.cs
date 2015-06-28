using System;
using Windows.UI;
using Windows.UI.Xaml.Media;
using AppBle01.Devices;
using AppBle01.Models;

namespace AppBle01.ViewModels.IndividualObjects
{
    public class BeDeviceVm : BeGattVmBase<BluetoothLeDevice>
    {
        public BeDeviceModel DeviceM { get; private set; }
        
        public string Name => DeviceM.Name.Trim();

        public UInt64 BluetoothAddress => DeviceM.BluetoothAddress;

        public String DeviceId => DeviceM.DeviceId;
       
        public string ConnectString
        {
            get
            {
                if (DeviceM.Connected)
                {
                    return "connected";
                }
                else
                {
                    return "disconnected";
                }
            }
        }
        
        public Brush ConnectColor
        {
            get
            {
                if (DeviceM.Connected)
                {
                    return new SolidColorBrush(Colors.Green);
                }
                else
                {
                    return new SolidColorBrush(Colors.Red);
                }
            }
        }

        public void Initialize(BeDeviceModel deviceM)
        {
            Model = deviceM; 
            DeviceM = deviceM;
            DeviceM.Register(this);
        }
    }

}