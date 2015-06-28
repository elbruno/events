using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using AppBle01.Devices;
using AppBle01.ViewModels.IndividualObjects;

namespace AppBle01.Models
{
    public class BeDeviceModel : BeGattModelBase<BluetoothLeDevice>
    {
        private DeviceInformation _deviceInfo;
        public List<BEServiceModel> ServiceModels { get; }
        private BluetoothLeDevice _device { get; set; }
        
        public String Name
        {
            get
            {
                return _device.Name.Trim();
            }
        }  // sanitized to remove spaces

        public UInt64 BluetoothAddress
        {
            get
            {
                return _device.BluetoothAddress;
            }
        }

        public String DeviceId
        {
            get
            {
                return _device.DeviceId;
            }
        }
        
        public bool Connected;

        public BeDeviceModel()
        {
            ServiceModels = new List<BEServiceModel>();
            ViewModelInstances = new List<BeGattVmBase<BluetoothLeDevice>>();
        }

        public void Initialize(BluetoothLeDevice device, DeviceInformation deviceInfo)
        {
            // Check for valid input
            if (device == null)
            {
                throw new ArgumentNullException(@"In BEDeviceVM, BluetoothLeDevice cannot be null.");
            }
            if (deviceInfo == null)
            {
                throw new ArgumentNullException("In BEDeviceVM, DeviceInformation cannot be null.");
            }

            // Initialize variables
            _device = device;
            _deviceInfo = deviceInfo;
            if (_device.ConnectionStatus == BluetoothLeDevice.BluetoothConnectionStatus.Connected)
            {
                Connected = true;
            }

            foreach (var service in _device.GattServices)
            {
                var serviceM = new BEServiceModel();
                serviceM.Initialize(service, this);
                ServiceModels.Add(serviceM); 
            }

            // Register event handlers
            _device.ConnectionStatusChanged += OnConnectionStatusChanged;
            _device.NameChanged += OnNameChanged;
            _device.GattServicesChanged += OnGattervicesChanged; 

            // Register for notifications from the device, on a separate thread
            //
            // NOTE:
            // This has the effect of telling the OS that we're interested in
            // these devices, and for it to automatically connect to them when
            // they are advertising.
            Utilities.RunFuncAsTask(RegisterNotificationsAsync);
        }

        private void OnNameChanged(BluetoothLeDevice sender, Object obj)
        {
            SignalChanged("Name");
        }

        private void OnGattervicesChanged(BluetoothLeDevice sender, Object obj)
        {
            Utilities.MakeAlertBox("Services on '" + Name + "' has changed! Please navigate back to the main page and refresh devices if you would like to update the device.");

            // Slightly hacky way of making sure that 1) nothing breaks if services/characteristics of this device are currently
            // being viewed while 2) ensuring that everything gets refreshed properly upon pressing the button on the main page. 
            if (GlobalSettings.PairedDevices.Contains(this))
            {
                GlobalSettings.PairedDevices.Remove(this);
            }
        }

        private void OnConnectionStatusChanged(BluetoothLeDevice sender, Object obj)
        {
            bool value = _device.ConnectionStatus == BluetoothLeDevice.BluetoothConnectionStatus.Connected;
            if (value != Connected)
            {
                // Change internal boolean and signal UI
                Connected = value;
                SignalChanged("ConnectString");
                SignalChanged("ConnectColor");
            }
        }

        private bool _notificationsRegistered;
        public async Task RegisterNotificationsAsync()
        {
            // Don't need to register notifications multiple times. 
            if (_notificationsRegistered)
            {
                return; 
            }

            foreach (var serviceM in ServiceModels)
            {
                await serviceM.RegisterNotificationsAsync();
            }

            // Notifications now registered.
            _notificationsRegistered = true; 
        }

        public async Task UnregisterNotificationsAsync()
        {
            try 
            {
                foreach (var serviceM in ServiceModels)
                {
                    await serviceM.UnregisterNotificationsAsync();
                }
            }
            catch (Exception ex)
            {
                // There's a chance the unregister will fail, as the device has been removed.
                Utilities.OnExceptionWithMessage(ex, "This failure may be expected as we're trying to unregister a device upon removal.");
            }

            _notificationsRegistered = false; 
        }
    }
}
