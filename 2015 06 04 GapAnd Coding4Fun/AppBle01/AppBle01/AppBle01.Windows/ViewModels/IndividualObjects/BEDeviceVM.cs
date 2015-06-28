using System;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace AppBle01.ViewModels.IndividualObjects
{
    /// <summary>
    /// Glue between the Device View and Model
    /// </summary>
    public class BEDeviceVM : BEGattVMBase<BluetoothLEDevice>
    {
        #region Properties
        // Funnels the model's properties to the XAML UI.
        public BEDeviceModel DeviceM { get; private set; }
        
        public string Name
        {
            get
            {
                return DeviceM.Name.Trim();
            }
        }

        public UInt64 BluetoothAddress
        {
            get
            {
                return DeviceM.BluetoothAddress;
            }
        }

        public String DeviceId
        {
            get
            {
                return DeviceM.DeviceId;
            }
        }
        #region Connectivity 
        
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
        #endregion
        #endregion

        public void Initialize(BEDeviceModel deviceM)
        {
            Model = deviceM; 
            DeviceM = deviceM;
            DeviceM.Register(this);
        }
    }

}