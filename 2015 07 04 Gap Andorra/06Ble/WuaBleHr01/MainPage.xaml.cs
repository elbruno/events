using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;
using Windows.UI.Core;
using WuaBleHr01.Annotations;

namespace WuaBleHr01
{
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        private DeviceInformation _devicePolar;
        public MainPage()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            
            var devices = await DeviceInformation.FindAllAsync(); // .FindAllAsync(GattDeviceService.GetDeviceSelectorFromUuid(GattServiceUuids.HeartRate));
            if (null == devices || devices.Count <= 0) return;
            //foreach (var device in devices.Where(device => device.Name == "Polar H7 498C1817"))
            foreach (var device in devices.Where(device => device.Name == "Charge HR"))

            {
                _devicePolar = device;
                StatusInformation = string.Format("Found {0}", _devicePolar.Name);
                break;
            }

            if (_devicePolar == null) return;
            var service = await GattDeviceService.FromIdAsync(_devicePolar.Id);
            if (null == service ) return;

            var characteristics = service.GetAllCharacteristics();
            if (null == characteristics || characteristics.Count <= 0) return;
            foreach (var characteristic in characteristics)
            {
                try
                {
                    characteristic.ValueChanged += GattCharacteristic_ValueChanged;
                    //await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

                    var val = await characteristic.ReadValueAsync();
                    

                    Debug.WriteLine(val.Value.ToString());
                }
                catch (Exception exception)
                {
                    StatusInformation2 = exception.ToString();
                }
            } 
        }

        private void GattCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            if (args == null) return;
            if (args.CharacteristicValue.Length == 0) return;

            int arrayLenght = (int) args.CharacteristicValue.Length;
            byte[] hrData = new byte[arrayLenght];
            DataReader.FromBuffer(args.CharacteristicValue).ReadBytes(hrData);

            //Convert to string  
            var hrValue = ProcessData(hrData);

            Debug.WriteLine(hrValue);
            HeartRateValue = hrValue.ToString();
        }

        private int ProcessData(byte[] data)
        {
            // Heart Rate profile defined flag values
            const byte HEART_RATE_VALUE_FORMAT = 0x01;
            const byte ENERGY_EXPANDED_STATUS = 0x08;

            byte currentOffset = 0;
            byte flags = data[currentOffset];
            bool isHeartRateValueSizeLong = ((flags & HEART_RATE_VALUE_FORMAT) != 0);

            currentOffset++;

            ushort heartRateMeasurementValue = 0;

            if (isHeartRateValueSizeLong)
            {
                heartRateMeasurementValue = (ushort)((data[currentOffset + 1] << 8) + data[currentOffset]);
                currentOffset += 2;
            }
            else
            {
                heartRateMeasurementValue = data[currentOffset];
                currentOffset++;
            }

              return  heartRateMeasurementValue;
        }

        private async void LoadBatteryLevel()
        {
            var batteryServices = await DeviceInformation.FindAllAsync(GattDeviceService.GetDeviceSelectorFromUuid(GattServiceUuids.Battery), null);

            if (batteryServices.Count > 0)
            {
                foreach (var di in batteryServices)
                {
                    Debug.WriteLine(di.Name);
                }
            }
        }

        #region Properties and Property Changed
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private async void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                 PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

             });
        }

        private string _statusInformation;
        public string StatusInformation
        {
            get { return _statusInformation; }
            set
            {
                if (value == _statusInformation) return;
                _statusInformation = value;
                OnPropertyChanged();
            }
        }

        private string _statusInformation2;
        public string StatusInformation2
        {
            get { return _statusInformation2; }
            set
            {
                if (value == _statusInformation2) return;
                _statusInformation2 = value;
                OnPropertyChanged();
            }
        }

        private string _heartRateValue;

        public string HeartRateValue
        {
            get { return _heartRateValue; }
            set
            {
                if (value == _heartRateValue) return;
                _heartRateValue = value;
                OnPropertyChanged();
            }
        }
        #endregion
    }
}
