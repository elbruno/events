using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.UI.Core;
using Windows.UI.Xaml;
using WuaBleRollingSpider.Drone;
using WuaBleRollingSpider.Model;
using WuaBleRs06.Annotations;

namespace WuaBleRs06
{
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        private DispatcherTimer _timer;
        private RsServiceDiscovery _rsServiceDiscovery;

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

        private async void ButtonAction2_Click(object sender, RoutedEventArgs e)
        {
            _dicCharacteristics = await _rsServiceDiscovery.InitServices();

            DevicesInformation = $"Get Chars{Environment.NewLine}";

            var powerMotors = _dicCharacteristics["Parrot_PowerMotors"].Characteristic;
            var initCount1To20 = _dicCharacteristics["Parrot_InitCount1_20"].Characteristic;
            var dateTimeChar = _dicCharacteristics["Parrot_DateTime"].Characteristic;
            var emergencyStop = _dicCharacteristics["Parrot_EmergencyStop"].Characteristic;

            var indicate = GattClientCharacteristicConfigurationDescriptorValue.Indicate;
            var notify = GattClientCharacteristicConfigurationDescriptorValue.Notify;

            DevicesInformation += $"Start enabling chars{Environment.NewLine}";

            DevicesInformation += $"  Char PowerMotors { Environment.NewLine}";
            await WriteConfigurationInChar( powerMotors, indicate);
            await WriteConfigurationInChar(powerMotors, notify);

            DevicesInformation += $"  Char InitCount1To20 { Environment.NewLine}";
            await WriteConfigurationInChar(initCount1To20, indicate);
            await WriteConfigurationInChar(initCount1To20, notify);

            DevicesInformation += $"  Char DateTime { Environment.NewLine}";
            await WriteConfigurationInChar(dateTimeChar, indicate);
            await WriteConfigurationInChar(dateTimeChar, notify);

            DevicesInformation += $"  Char EmergencyStop { Environment.NewLine}";
            await WriteConfigurationInChar(emergencyStop, indicate);
            await WriteConfigurationInChar(emergencyStop, notify);

            DevicesInformation += $"Done enabling chars{Environment.NewLine}";

        }

        private async Task WriteConfigurationInChar(GattCharacteristic characteristic, GattClientCharacteristicConfigurationDescriptorValue charConfigValue)
        {
            try
            {
                DevicesInformation += $"  {charConfigValue} {Environment.NewLine}";
                await
                   characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(charConfigValue);
                DevicesInformation += $"    - Ok{Environment.NewLine}";
            }
            catch (Exception exception)
            {
                DevicesInformation += $"    - Exception {exception.Message}{Environment.NewLine}";
            }
        }

        private async void ButtonAction3_Click(object sender, RoutedEventArgs e)
        {
            DevicesInformation += $"Test started {Environment.NewLine}";

            var tourTheStairs = new TourTheStairs(_rsServiceDiscovery.PowerMotors, _rsServiceDiscovery.DateTime, _rsServiceDiscovery.InitCount1To20, _rsServiceDiscovery.EmergencyStop);

            var result = await tourTheStairs.Init();
            if (tourTheStairs.HasExceptionStack)
                DevicesInformation += tourTheStairs.GetExceptionStack();
            DevicesInformation += $"Init {result} {Environment.NewLine}";

            result = await tourTheStairs.Takeoff();
            if (tourTheStairs.HasExceptionStack)
                DevicesInformation += tourTheStairs.GetExceptionStack();
            DevicesInformation += $"TakeOff {result} {Environment.NewLine}";

            result = await tourTheStairs.Motors(true, 0, 10, 0, 0, 0.0f, 15);
            if (tourTheStairs.HasExceptionStack)
                DevicesInformation += tourTheStairs.GetExceptionStack();
            DevicesInformation += $"Motors 1 {result} {Environment.NewLine}";

            tourTheStairs.ClearExceptions();
            for (int i = 0; i < 100; i++)
            {
                if (await tourTheStairs.Motors(true, 0, 10, 0, 0, 0.0f, 20) == false)
                    break;
            }
            if (tourTheStairs.HasExceptionStack)
                DevicesInformation += tourTheStairs.GetExceptionStack();
            DevicesInformation += $"Motors 2 For i to 100 {result} {Environment.NewLine}";


            result = await tourTheStairs.Land();
            if (tourTheStairs.HasExceptionStack)
                DevicesInformation += tourTheStairs.GetExceptionStack();
            DevicesInformation += $"Land {result} {Environment.NewLine}";

            result = await tourTheStairs.Motors(false, 0, 0, 0, 0, 0.0f, 20); // it has to land anyway
            if (tourTheStairs.HasExceptionStack)
                DevicesInformation += tourTheStairs.GetExceptionStack();
            DevicesInformation += $"Motors 3 {result} {Environment.NewLine}";
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
