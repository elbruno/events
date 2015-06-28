using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AppBle01.ViewModels.IndividualObjects;
using AppBle01.ViewModels.Lists;

namespace AppBle01
{
    /// <summary>
    /// Page representing a specific device, and all its services
    /// </summary>
    public sealed partial class DeviceInfo
    {

        private ListBox _serviceListBox;
        public BeDeviceVm DeviceVm { get; set; }
        public BEServiceListVM ServicesVm { get; set; }

        public DeviceInfo()
        {
            InitializeComponent();   // default init for page object

            // Handle back button press
            //Loaded += (sender, e) => { HardwareButtons.BackPressed += OnBackPressed; };
            // De-register back button when the page is no longer visible
            //Unloaded += (sender, e) => { HardwareButtons.BackPressed -= OnBackPressed; };

            // Create initial instances of page objects
            DeviceVm = new BeDeviceVm();
            ServicesVm = new BEServiceListVM();
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Reset list choice
            if (_serviceListBox != null)
            {
                _serviceListBox.SelectedIndex = -1;
            }

            // These must be initialized before the UI loads
            DeviceVm.Initialize(GlobalSettings.SelectedDevice);
            ServicesVm.Initialize(GlobalSettings.SelectedDevice.ServiceModels);

            // Complete remaining initialization without blocking this callback
            Utilities.RunFuncAsTask(GlobalSettings.SelectedDevice.RegisterNotificationsAsync);
        }

        // XAML elements inside the HubSection are template-based and cannot be directly access, hook the ListBox instance here
        private void ServiceListLoaded(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                _serviceListBox = (ListBox)sender;
            }
        }

        private void OnServiceSelectionChanged(object sender, RoutedEventArgs e)
        {
            var listBox = (ListBox)sender;

            if (listBox.SelectedIndex == -1)
            {
                return;
            }

            foreach (var listBoxItem in listBox.SelectedItems)
            {
                var service = (BEServiceVM) listBoxItem;
                GlobalSettings.SelectedService = service.ServiceM;
            }
            //Frame.Navigate(typeof(ServiceInfo));
        }
    }
}
