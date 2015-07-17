using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using WuaBleRollingSpider.Drone;
using WuaBleRollingSpider.Model;
using WuaBleRs03.Annotations;

namespace WuaBleRs03
{
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private async void ButtonAction1_Click(object sender, RoutedEventArgs e)
        {
            DevicesInformation = "Load devices started";
            _rsServiceDiscovery = new RsServiceDiscovery();
            _dicCharacteristics = await _rsServiceDiscovery.InitServices();
            DevicesInformation = $@"Load devices completed
Chars Found:";
            foreach (var rsCharacteristic in _dicCharacteristics)
            {
                DevicesInformation += $"  {rsCharacteristic.Value.CharName}{Environment.NewLine}";
            }

        }

        private async Task WriteConfigurationInChar(KeyValuePair<string, RsCharacteristic> rsCharacteristic, GattClientCharacteristicConfigurationDescriptorValue charConfigValue)
        {
            try
            {
                DevicesInformation = $"  {charConfigValue} {Environment.NewLine}";
                await
                   rsCharacteristic.Value.Characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(charConfigValue);
            }
            catch (Exception exception)
            {
                DevicesInformation = $"  Exception {exception}";
            }
        }

        private async void ButtonAction2_Click(object sender, RoutedEventArgs e)
        {
            DevicesInformation = $"Start enabling chars{Environment.NewLine}";

            foreach (var rsCharacteristic in _dicCharacteristics)
            {
                DevicesInformation = $"  Char {rsCharacteristic.Value.CharName}{ Environment.NewLine}";
                await WriteConfigurationInChar(rsCharacteristic, GattClientCharacteristicConfigurationDescriptorValue.Indicate);
                await WriteConfigurationInChar(rsCharacteristic, GattClientCharacteristicConfigurationDescriptorValue.Notify);
            }
            DevicesInformation += $"Done enabling chars{Environment.NewLine}";

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
        private Dictionary<string, RsCharacteristic> _svc;


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

        #endregion

        private string _devicesInformation;
        private Dictionary<string, RsCharacteristic> _dicCharacteristics;
        private RsServiceDiscovery _rsServiceDiscovery;

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
    }
}
