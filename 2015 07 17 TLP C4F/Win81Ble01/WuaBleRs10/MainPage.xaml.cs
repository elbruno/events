using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.UI.Core;
using Windows.UI.Xaml;
using WuaBleRollingSpider.Drone;
using WuaBleRollingSpider.Model;
using WuaBleRs10.Annotations;

namespace WuaBleRs10
{
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        private DispatcherTimer _timer;

        public MainPage()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private async void ButtonAction1_Click(object sender, RoutedEventArgs e)
        {
            DevicesInformation = $"Device Id,Name,service.Uuid,service Name,service.AttributeHandle,characteristic.Uuid,Char Name,CharacteristicProperties,Char ProtectionLevel,UserDescription,,,{Environment.NewLine}";
            Debug.WriteLine(DevicesInformation);
            var bts = await DeviceInformation.FindAllAsync();
            foreach (var device in bts.Where(di => di.Name == RsServiceDiscovery.DeviceName))
            {
                try
                {
                    var service = await GattDeviceService.FromIdAsync(device.Id);
                    if (null == service) continue;
                    var characteristics = service.GetAllCharacteristics();
                    if (null == characteristics || characteristics.Count <= 0) return;

                    foreach (var characteristic in characteristics)
                    {
                        try
                        {
                            var serviceName = CharacteristicUuidsResolver.GetNameFromUuid(service.Uuid);
                            var charName = CharacteristicUuidsResolver.GetNameFromUuid(characteristic.Uuid);

                            string msg =
                                $"{device.Id}, {device.Name}, {service.Uuid}, {serviceName}, {service.AttributeHandle}, {characteristic.Uuid}, {charName}, {characteristic.CharacteristicProperties}, {characteristic.ProtectionLevel}, {characteristic.UserDescription}{Environment.NewLine}";
                            Debug.WriteLine(msg);
                            DevicesInformation += msg;
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                }
                catch
                {
                    string msg = $"{device.Id}, {device.Name}, , , , , , , , {Environment.NewLine}";
                    Debug.WriteLine(msg);
                    DevicesInformation += msg;
                }
            }
        }
        private async void ButtonAction2_Click(object sender, RoutedEventArgs e)
        {
            var serviceIds = new[]
            {
                "00001800-0000-1000-8000-00805F9B34FB",
                "00001801-0000-1000-8000-00805F9B34FB",
                "9A66FA00-0800-9191-11E4-012D1540CB8E",
                "9A66FB00-0800-9191-11E4-012D1540CB8E",
                "9A66FC00-0800-9191-11E4-012D1540CB8E",
                "9A66FD21-0800-9191-11E4-012D1540CB8E",
                "9A66FD51-0800-9191-11E4-012D1540CB8E",
                "9A66FE00-0800-9191-11E4-012D1540CB8E"
            };
            DevicesInformation = "";
            var bts = await DeviceInformation.FindAllAsync();
            foreach (var serviceId in serviceIds)
            {
                DevicesInformation += $"{serviceId}{Environment.NewLine}"; 

                foreach (var device in bts)
                {
                    if (!device.Id.ToUpper().Contains(serviceId)) continue;

                    DevicesInformation += $"    {device.Id}{Environment.NewLine}";
                    try
                    {
                        var service = await GattDeviceService.FromIdAsync(device.Id);
                        if (null == service) continue;
                        var characteristicsFa00 = service.GetAllCharacteristics();
                        DevicesInformation += $"    Allow Chars{Environment.NewLine}";
                    }
                    catch
                    {
                        DevicesInformation += $"    Don't Allow Chars{Environment.NewLine}";
                    }
                }
            }
        }
        private async void ButtonAction3_Click(object sender, RoutedEventArgs e)
        {
            var serviceIds = new[]
            {
                "00001800-0000-1000-8000-00805F9B34FB",
                "00001801-0000-1000-8000-00805F9B34FB",
                "9A66FA00-0800-9191-11E4-012D1540CB8E",
                "9A66FB00-0800-9191-11E4-012D1540CB8E",
                "9A66FC00-0800-9191-11E4-012D1540CB8E",
                "9A66FD21-0800-9191-11E4-012D1540CB8E",
                "9A66FD51-0800-9191-11E4-012D1540CB8E",
                "9A66FE00-0800-9191-11E4-012D1540CB8E"
            };
            DevicesInformation = @"// generated Ids on " + DateTime.Now + Environment.NewLine + "private List<string> deviceIds = new List<string>{" + Environment.NewLine;
            var bts = await DeviceInformation.FindAllAsync();
            foreach (var serviceId in serviceIds)
            {
                foreach (var device in bts)
                {
                    if (!device.Id.ToUpper().Contains(serviceId)) continue;
                    try
                    {
                        var service = await GattDeviceService.FromIdAsync(device.Id);
                        if (null == service) continue;
                        var characteristicsFa00 = service.GetAllCharacteristics();
                        DevicesInformation += $@"@""{device.Id}"", {Environment.NewLine}";
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            DevicesInformation += "};";
        }

        private async void ButtonAction4_Click(object sender, RoutedEventArgs e)
        {
            DevicesInformation = $"Load devices started{Environment.NewLine}";
            var rsServiceDiscovery = new RsServiceDiscovery();
            _dicCharacteristics = await rsServiceDiscovery.InitServices();
            DevicesInformation += $@"Load devices completed
Chars Found:{Environment.NewLine}";

            foreach (var rsCharacteristic in _dicCharacteristics)
            {
                DevicesInformation += $"  {rsCharacteristic.Value.CharName}{Environment.NewLine}";
            }

            DevicesInformation += $"Start enabling chars{Environment.NewLine}";

            foreach (var rsCharacteristic in _dicCharacteristics)
            {
                DevicesInformation += $"  Char {rsCharacteristic.Value.CharName}{ Environment.NewLine}";
                await WriteConfigurationInChar(rsCharacteristic, GattClientCharacteristicConfigurationDescriptorValue.Indicate);
                await WriteConfigurationInChar(rsCharacteristic, GattClientCharacteristicConfigurationDescriptorValue.Notify);
            }
            DevicesInformation += $"Done enabling chars{Environment.NewLine}";

        }

        private async Task WriteConfigurationInChar(KeyValuePair<string, RsCharacteristic> rsCharacteristic, GattClientCharacteristicConfigurationDescriptorValue charConfigValue)
        {
            try
            {
                DevicesInformation += $"  {charConfigValue} {Environment.NewLine}";
                await
                   rsCharacteristic.Value.Characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(charConfigValue);
                DevicesInformation += $"    - Ok{Environment.NewLine}";
            }
            catch (Exception exception)
            {
                DevicesInformation += $"    - Exception {exception.Message}{Environment.NewLine}";
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

        private string _devicesInformation;
        private Dictionary<string, RsCharacteristic> _dicCharacteristics;

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


        #endregion

    }
}
