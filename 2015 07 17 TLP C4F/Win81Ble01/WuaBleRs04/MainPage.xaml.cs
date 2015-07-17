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
using WuaBleRs04.Annotations;
using WuaBleRs04.Drone;

namespace WuaBleRs04
{
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        private DispatcherTimer _timer;
        public const string DeviceName = "RS_R056712";
        private GattDeviceService _service;
        private IReadOnlyList<GattCharacteristic> _characteristics;

        public MainPage()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            var bts = await DeviceInformation.FindAllAsync();
            const string deviceId = @"\\?\BTHLEDevice#{9a66fa00-0800-9191-11e4-012d1540cb8e}_e0144d4f3d49#a&1f09a1af&0&0020#{6e3bb679-4372-40c8-9eaa-4509df260cd8}";

            var device = bts.First(di => di.Name == DeviceName && di.Id == deviceId);
            if (null == device)
                return;
            _service = await GattDeviceService.FromIdAsync(device.Id);
            if (null == _service)
                return;
            _characteristics = _service.GetAllCharacteristics();
            if (null == _characteristics || _characteristics.Count <= 0)
                return;

            var characteristic = _characteristics.First(charact => charact.Uuid == RollingSpiderCharacteristicUuids.Parrot_PowerMotors);

            try
            {
                var charName = CharacteristicUuidsResolver.GetNameFromUuid(characteristic.Uuid);
                Debug.WriteLine(charName);
                for (int i = 0; i < 255; i++)
                {
                    Debug.WriteLine(i);
                    byte[] arr = { (byte)02, (byte)40, (byte)20, (byte)0D, 00, 09, 00, 04, 00, 52, 43, 00, 04, (byte)i, 02, 00, 01, 00, };
                    var writer = new DataWriter();
                    writer.WriteBytes(arr);
                    await characteristic.WriteValueAsync(writer.DetachBuffer(), GattWriteOption.WriteWithoutResponse);

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }
            catch
            {
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

        #endregion
    }
}
