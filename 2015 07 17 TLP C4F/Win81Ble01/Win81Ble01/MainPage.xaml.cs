using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml;
using Win81Ble01.Annotations;

namespace Win81Ble01
{
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        private string _deviceName = "RS_R056712";
        private List<DeviceInformation> _parrotDevices;

        public MainPage()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            GetBtDevicesAndList();
        }

        private async Task GetBtDevicesAndList()
        {
            StatusInformation = "Start Get Devices";
            _parrotDevices = new List<DeviceInformation>();
            var bts = await DeviceInformation.FindAllAsync();
            var i = 0;
            foreach (var di in bts)
            {
                i++;
                StatusInformation2 = string.Format("{0} found. {1}", i, di.Name);
                if (di.Name != _deviceName) continue;
                _parrotDevices.Add(di);
                Debug.WriteLine(di.Id);
                DevicesInformation += string.Format("{0} - {1}{2}", i.ToString("0000"), di.Id, Environment.NewLine);
            }
            StatusInformation = string.Format("{0} Parrot Found", bts.Count);
        }
        private async void GetParrotDevicesAndList()
        {
            foreach (var deviceInformation in _parrotDevices)
            {
                try
                {
                    var service = await GattDeviceService.FromIdAsync(deviceInformation.Id);
                    if (null == service) return;

                    //var characteristics = service.GetCharacteristics();
                    //if (null == characteristics || characteristics.Count <= 0) return;
                    //foreach (var characteristic in characteristics)
                    //{
                    //    try
                    //    {
                    //        characteristic.ValueChanged += GattCharacteristic_ValueChanged;
                    //        await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

                    //    }
                    //    catch (Exception exception)
                    //    {
                    //        StatusInformation2 = exception.ToString();
                    //    }
                    //}

                }
                catch (Exception)
                {
                    Debug.WriteLine("Error");
                }
                Debug.WriteLine("------------------------------------------------------");
            }
        }
        private void ButtonGetParrotDevice_Click(object sender, RoutedEventArgs e)
        {
            GetParrotDevicesAndList();
        }

        private void ButtonGetDevices_Click(object sender, RoutedEventArgs e)
        {
            GetBtDevicesAndList();
        }
        private async void ButtonConnectToDevice_Click(object sender, RoutedEventArgs e)
        {
            foreach (var deviceInformation in _parrotDevices)
            {
                try
                {
                    Debug.WriteLine("Try get Gatt Device Services for : " + deviceInformation.Id);
                    var service = await GattDeviceService.FromIdAsync(deviceInformation.Id);
                    if (service == null) continue;
                    Debug.WriteLine("// Found Gatt Device Services !");

                    //Obtain the characteristic we want to interact with  
                    var characteristics = service.GetCharacteristics(GattCharacteristic.ConvertShortIdToUuid(0x2A00));
                    Debug.WriteLine("// Found {0} characteristics !", characteristics.Count);
                    foreach (var gattCharacteristic in characteristics)
                    {
                        //Read the value  
                        var deviceNameBytes = (await gattCharacteristic.ReadValueAsync()).Value.ToArray();
                        //Convert to string  
                        var deviceName = Encoding.UTF8.GetString(deviceNameBytes, 0, deviceNameBytes.Length);
                        Debug.WriteLine(deviceName);
                    }

                    //Obtain the characteristic we want to interact with  
                    var includedServices = service.GetIncludedServices(GattCharacteristic.ConvertShortIdToUuid(0x2A00));
                    Debug.WriteLine("// Found {0} includedServices !", includedServices.Count);
                    foreach (var isvc in includedServices)
                    {
                        Debug.WriteLine(isvc.AttributeHandle.ToString());
                    }

                }
                catch (Exception)
                {
                    Debug.WriteLine("Error");
                }
                Debug.WriteLine("------------------------------------------------------");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        private string _devicesInformation;
        public string DevicesInformation
        {
            get { return _devicesInformation; }
            set
            {
                if (value == _devicesInformation) return;
                _devicesInformation = value;
                OnPropertyChanged();
            }
        }

        private string _parrotDevicesIds;
        public string ParrotDevicesIds
        {
            get { return _parrotDevicesIds; }
            set
            {
                if (value == _parrotDevicesIds) return;
                _parrotDevicesIds = value;
                OnPropertyChanged();
            }
        }



    }
}
