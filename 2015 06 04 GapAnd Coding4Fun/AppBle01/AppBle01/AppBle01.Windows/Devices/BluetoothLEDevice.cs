using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Foundation;

namespace AppBle01.Devices
{
    public class BluetoothLeDevice
    {
        public enum BluetoothConnectionStatus
        {
            Disconnected,
            Connected,
        }
        public GattDeviceService GetGattService([In] Guid serviceUuid)
        {
            return null;
        }

        public static IAsyncOperation<BluetoothLeDevice> FromIdAsync([In] string deviceId)
        {
            return null;
        }

        public static IAsyncOperation<BluetoothLeDevice> FromBluetoothAddressAsync([In] ulong bluetoothAddress)
        {
            return null;
        }
        public static string GetDeviceSelector()
        {
            return null;
        }
        public ulong BluetoothAddress {get; set; }
        public BluetoothConnectionStatus ConnectionStatus {get; set; }

        public string DeviceId {get; set; }
        public IReadOnlyList<GattDeviceService> GattServices {get; set; }
        public string Name {get; set; }

        public event TypedEventHandler<BluetoothLeDevice, object> ConnectionStatusChanged;
        public event TypedEventHandler<BluetoothLeDevice, object> GattServicesChanged;
        public event TypedEventHandler<BluetoothLeDevice, object> NameChanged;

        protected virtual void OnConnectionStatusChanged(object args)
        {
            ConnectionStatusChanged?.Invoke(this, args);
        }

        protected virtual void OnGattServicesChanged(object args)
        {
            GattServicesChanged?.Invoke(this, args);
        }

        protected virtual void OnNameChanged(object args)
        {
            NameChanged?.Invoke(this, args);
        }
    }
}
