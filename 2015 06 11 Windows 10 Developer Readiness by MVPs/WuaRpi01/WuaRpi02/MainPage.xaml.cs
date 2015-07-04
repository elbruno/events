using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Devices.Enumeration;
using WuaRpi02.Annotations;

namespace WuaRpi02
{
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            ButtonGetDeviceInformation_Click(null, null);
        }

        private void ButtonHelloWorld_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            HelloWorld = $"Hello World. Current Time {DateTime.Now}";
        }

        private async void ButtonGetDeviceInformation_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var devices = await DeviceInformation.FindAllAsync();
            if (devices == null) return;

            LocalDevicesInformation = string.Empty;
            var filteredList = new Dictionary<string, DeviceInformation>();
            foreach (var device in devices)
            {
                if (filteredList.ContainsKey(device.Name)) continue;
                filteredList.Add(device.Name, device);
                LocalDevicesInformation += $"{device.Name} {Environment.NewLine}";
            }
        }

        #region Properties and Property Changed
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

        private string _helloWorld;
        public string HelloWorld
        {
            get { return _helloWorld; }
            set
            {
                if (value == _helloWorld) return;
                _helloWorld = value;
                OnPropertyChanged();
            }
        }

        private string _localDevicesInformation;
        public string LocalDevicesInformation
        {
            get { return _localDevicesInformation; }
            set
            {
                if (value == _localDevicesInformation) return;
                _localDevicesInformation = value;
                OnPropertyChanged();
            }
        }
        #endregion
    }
}
