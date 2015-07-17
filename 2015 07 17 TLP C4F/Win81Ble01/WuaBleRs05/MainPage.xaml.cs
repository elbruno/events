using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using WuaBleRollingSpider.Drone;
using WuaBleRollingSpider.Model;
using WuaBleRs05.Annotations;

namespace WuaBleRs05
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
            DevicesInformation = string.Empty;
            foreach (var rsCharacteristic in _dicCharacteristics)
            {
                DevicesInformation += $"  Try to write to {rsCharacteristic.Value.CharName}{Environment.NewLine}";
                bool writeOk = true;
                try
                {
                    var charToTest = rsCharacteristic.Value.Characteristic;
                    byte[] arr = { 4, 1, 2, 0, 1, 0 };
                    var writer = new DataWriter();
                    writer.WriteBytes(arr);
                    await charToTest.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Indicate);
                    await charToTest.WriteValueAsync(writer.DetachBuffer());
                    DevicesInformation += $"  - OK{Environment.NewLine}";
                }
                catch
                {
                    DevicesInformation += $"  - ERROR{Environment.NewLine}";
                    writeOk = false;
                }

                var msg = $"{rsCharacteristic.Value.CharName}, {writeOk}, {rsCharacteristic.Value.Characteristic.CharacteristicProperties}, {rsCharacteristic.Value.Characteristic.Uuid}";
                Debug.WriteLine(msg);

                await Task.Delay(TimeSpan.FromMilliseconds(100));
            }
        }

        private async void ButtonAction3_Click(object sender, RoutedEventArgs e)
        {
            var writeableChars = new[]
            {"Parrot_FC1", "Parrot_D22", "Parrot_D23", "Parrot_D24", "Parrot_D52", "Parrot_D53", "Parrot_D54"};

            DevicesInformation = string.Empty;
            foreach (var writeableChar in writeableChars)
            {
                foreach (var rsCharacteristic in _dicCharacteristics.Where(rsCharacteristic => rsCharacteristic.Value.CharName == writeableChar))
                {
                    await rsCharacteristic.Value.Characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

                    DevicesInformation += $"  Test write {rsCharacteristic.Value.CharName}{Environment.NewLine}";
                    try
                    {
                        var charToTest = rsCharacteristic.Value.Characteristic;
                        byte[] arr1 = { 02, 40, 20, 09, 00, 05, 00, 04, 00, 12, 0xC0, 00, 01, 00 };
                        var writer = new DataWriter();
                        writer.WriteBytes(arr1);
                        await charToTest.WriteValueAsync(writer.DetachBuffer());
                        DevicesInformation += $"  - 1 OK{Environment.NewLine}";
                        await Task.Delay(TimeSpan.FromMilliseconds(100));

                        byte[] arr2 = { 02, 40, 20, 09, 00, 05, 00, 04, 00, 12, 0xBD, 00, 01, 00 };
                        writer = new DataWriter();
                        writer.WriteBytes(arr2);
                        await charToTest.WriteValueAsync(writer.DetachBuffer());
                        DevicesInformation += $"  - 2 OK{Environment.NewLine}";
                        await Task.Delay(TimeSpan.FromMilliseconds(100));

                        byte[] arr3 = { 02, 40, 20, 09, 00, 05, 00, 04, 00, 12, 0xE4, 00, 01, 00 };
                        writer = new DataWriter();
                        writer.WriteBytes(arr3);
                        await charToTest.WriteValueAsync(writer.DetachBuffer());
                        DevicesInformation += $"  - 3 OK{Environment.NewLine}";
                        await Task.Delay(TimeSpan.FromMilliseconds(100));

                        byte[] arr4 = { 02, 40, 20, 09, 00, 05, 00, 04, 00, 12, 0xE7, 00, 01, 00 };
                        writer = new DataWriter();
                        writer.WriteBytes(arr4);
                        await charToTest.WriteValueAsync(writer.DetachBuffer());
                        DevicesInformation += $"  - 4 OK{Environment.NewLine}";
                        await Task.Delay(TimeSpan.FromMilliseconds(100));

                        byte[] arr5 = { 02, 40, 20, 09, 00, 05, 00, 04, 00, 12, 52, 16, 01, 00 };
                        writer = new DataWriter();
                        writer.WriteBytes(arr5);
                        await charToTest.WriteValueAsync(writer.DetachBuffer());
                        DevicesInformation += $"  - 5 OK{Environment.NewLine}";
                        await Task.Delay(TimeSpan.FromMilliseconds(100));

                        byte[] arr6 = { 02, 40, 20, 09, 00, 05, 00, 04, 00, 12, 52, 26, 01, 00 };
                        writer = new DataWriter();
                        writer.WriteBytes(arr6);
                        await charToTest.WriteValueAsync(writer.DetachBuffer());
                        DevicesInformation += $"  - 6 OK{Environment.NewLine}";
                        await Task.Delay(TimeSpan.FromMilliseconds(100));
                    }
                    catch
                    {
                        DevicesInformation += $"  - ERROR{Environment.NewLine}";
                    }
                }
            }
            DevicesInformation += "Done";
        }

        private async void ButtonAction4_Click(object sender, RoutedEventArgs e)
        {
            var writeableChars = new[]
{"Parrot_FC1", "Parrot_D22", "Parrot_D23", "Parrot_D24", "Parrot_D52", "Parrot_D53", "Parrot_D54"};

            DevicesInformation = string.Empty;
            foreach (var writeableChar in writeableChars)
            {
                foreach (var rsCharacteristic in _dicCharacteristics.Where(rsCharacteristic => rsCharacteristic.Value.CharName == writeableChar))
                {
                    await rsCharacteristic.Value.Characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);

                    DevicesInformation += $"  Test write {rsCharacteristic.Value.CharName}{Environment.NewLine}";
                    try
                    {

                        var gattTransaction = new GattReliableWriteTransaction();

                        var charToTest = rsCharacteristic.Value.Characteristic;
                        byte[] arr1 = { 02, 40, 20, 09, 00, 05, 00, 04, 00, 12, 0xC0, 00, 01, 00 };
                        var writer = new DataWriter();
                        writer.WriteBytes(arr1);
                        gattTransaction.WriteValue(charToTest, writer.DetachBuffer());
                        DevicesInformation += $"  - 1 OK{Environment.NewLine}";

                        byte[] arr2 = { 02, 40, 20, 09, 00, 05, 00, 04, 00, 12, 0xBD, 00, 01, 00 };
                        writer = new DataWriter();
                        writer.WriteBytes(arr2);
                        gattTransaction.WriteValue(charToTest, writer.DetachBuffer());
                        DevicesInformation += $"  - 2 OK{Environment.NewLine}";

                        byte[] arr3 = { 02, 40, 20, 09, 00, 05, 00, 04, 00, 12, 0xE4, 00, 01, 00 };
                        writer = new DataWriter();
                        writer.WriteBytes(arr3);
                        gattTransaction.WriteValue(charToTest, writer.DetachBuffer());
                        DevicesInformation += $"  - 3 OK{Environment.NewLine}";

                        byte[] arr4 = { 02, 40, 20, 09, 00, 05, 00, 04, 00, 12, 0xE7, 00, 01, 00 };
                        writer = new DataWriter();
                        writer.WriteBytes(arr4);
                        gattTransaction.WriteValue(charToTest, writer.DetachBuffer());
                        DevicesInformation += $"  - 4 OK{Environment.NewLine}";

                        byte[] arr5 = { 02, 40, 20, 09, 00, 05, 00, 04, 00, 12, 52, 16, 01, 00 };
                        writer = new DataWriter();
                        writer.WriteBytes(arr5);
                        gattTransaction.WriteValue(charToTest, writer.DetachBuffer());
                        DevicesInformation += $"  - 5 OK{Environment.NewLine}";

                        byte[] arr6 = { 02, 40, 20, 09, 00, 05, 00, 04, 00, 12, 52, 26, 01, 00 };
                        writer = new DataWriter();
                        writer.WriteBytes(arr6);
                        gattTransaction.WriteValue(charToTest, writer.DetachBuffer());
                        DevicesInformation += $"  - 6 OK{Environment.NewLine}";

                        var status = await gattTransaction.CommitAsync();
                        switch (status)
                        {
                            case GattCommunicationStatus.Success:
                                DevicesInformation += "  Writing to your device OK !";
                                break;
                            case GattCommunicationStatus.Unreachable:
                                DevicesInformation += "  Writing to your device Failed !";
                                break;
                        }
                    }
                    catch (Exception exception)
                    {
                        DevicesInformation += $"  - ERROR {exception.Message}{Environment.NewLine}";
                    }
                }
            }
            DevicesInformation += "Done";
        }

        private async void ButtonAction5_Click(object sender, RoutedEventArgs e)
        {
            DevicesInformation = string.Empty;
            try
            {
                var motors = _dicCharacteristics["Parrot_PowerMotors"].Characteristic;
                await motors.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Indicate);
                byte[] arr1 = { 02, 40, 20, 09, 00, 05, 00, 04, 00, 12, 0xC0, 00, 01, 00 };


                var buffer = new DataWriter();
                buffer.WriteInt16(2);
                buffer.WriteInt16(1);
                buffer.WriteInt16(2);
                buffer.WriteInt16(0);
                buffer.WriteInt16(2);
                buffer.WriteInt16(0);
                buffer.WriteInt16(1);
                buffer.WriteInt16(1);
                buffer.WriteInt16(1);
                buffer.WriteInt16(1);
                buffer.WriteInt16(1);
                buffer.WriteDouble(0);

                await motors.WriteValueAsync(buffer.DetachBuffer(), GattWriteOption.WriteWithoutResponse);
                DevicesInformation += $"  - 1 OK{Environment.NewLine}";
            }
            catch (Exception exception)
            {
                DevicesInformation += $"  - ERROR {exception.Message}{Environment.NewLine}";
            }


        }

        private async void ButtonAction6_Click(object sender, RoutedEventArgs e)
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
