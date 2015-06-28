using System;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using AppBle01.ViewModels.IndividualObjects;
using AppBle01.ViewModels.Lists;

namespace AppBle01
{
    public sealed partial class MainPage
    {
        private ListBox _deviceListBox;
        public BEDeviceListVM DevicesVm { get; }

        public Visibility BackgroundAccessProblem
        {
            get
            {
                if (GlobalSettings.BackgroundAccessRequested)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }

        private bool _firstEntry;
        public MainPage()
        {
            InitializeComponent();
            // var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            DevicesVm = new BEDeviceListVM();
            _firstEntry = true;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Reset Device List selection
            if (_deviceListBox != null)
            {
                _deviceListBox.SelectedIndex = -1;
            }

            if (!_firstEntry) return;
            var backgroundAccessProblemVisibility = Visibility.Collapsed;
            _firstEntry = false;
            SetValue(BackgroundAccessProblemProperty, Visibility.Collapsed);

            await GlobalSettings.RequestBackgroundAccessAsync();
            if (!GlobalSettings.BackgroundAccessRequested)
            {
                backgroundAccessProblemVisibility = Visibility.Visible;
            }
            SetValue(BackgroundAccessProblemProperty, backgroundAccessProblemVisibility);
            Utilities.RunFuncAsTask(PopulateLeDeviceListAsync);
        }

        /// <summary>
        /// Retrieves the list of Bluetooth LE devices from the OS, initializes our internal
        /// data structures, and attempt to connect to them, if they are advertising.
        /// </summary>
        /// <returns></returns>
        private async Task PopulateLeDeviceListAsync()
        {
            await Utilities.RunActionOnUiThreadAsync(() => IsUserInteractionEnabled = false);

            await GlobalSettings.PopulateDeviceListAsync();

            await Utilities.RunActionOnUiThreadAsync(
                () =>
                {
                    DevicesVm.Initialize(GlobalSettings.PairedDevices);
                    IsUserInteractionEnabled = true;
                });
        }

        public static readonly DependencyProperty IsUserInteractionEnabledProperty =
            DependencyProperty.Register("IsUserInteractionEnabled", typeof(bool), typeof(MainPage), new PropertyMetadata(false));
        public static readonly DependencyProperty IsUpdatingDeviceListProperty =
            DependencyProperty.Register("IsUpdatingDeviceList", typeof(Visibility), typeof(MainPage), new PropertyMetadata(false));
        public static readonly DependencyProperty ShowDeviceListProperty =
            DependencyProperty.Register("ShowDeviceList", typeof(Visibility), typeof(MainPage), new PropertyMetadata(false));
        public static readonly DependencyProperty BackgroundAccessProblemProperty =
            DependencyProperty.Register("BackgroundAccessProblem", typeof(Visibility), typeof(MainPage), new PropertyMetadata(false));
        public bool IsUserInteractionEnabled
        {
            get
            {
                return (bool)GetValue(IsUserInteractionEnabledProperty);
            }
            set
            {
                SetValue(IsUserInteractionEnabledProperty, value);
                if (value)
                {
                    SetValue(IsUpdatingDeviceListProperty, Visibility.Collapsed);
                    SetValue(ShowDeviceListProperty, Visibility.Visible);
                }
                else
                {
                    SetValue(IsUpdatingDeviceListProperty, Visibility.Visible);
                    SetValue(ShowDeviceListProperty, Visibility.Collapsed);
                }
            }
        }

        private void OnDeviceListLoaded(object sender, RoutedEventArgs e)
        {
            if (sender != null)
            {
                _deviceListBox = (ListBox)sender;
            }
        }

        private void OnDeviceSelectionChanged(object sender, RoutedEventArgs e)
        {
            var listBox = (ListBox)sender;

            if (listBox.SelectedIndex == -1)
            {
                return;
            }

            foreach (var listBoxItem in listBox.SelectedItems)
            {
                var device = listBoxItem as BeDeviceVm;
                if (device != null) GlobalSettings.SelectedDevice = device.DeviceM;
            }
            Frame.Navigate(typeof(DeviceInfo));
        }

        private void deviceListRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Utilities.RunFuncAsTask(PopulateLeDeviceListAsync);
        }

        private async void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("ms-settings-bluetooth:"));
        }

        private void goToAbout_Click(object sender, RoutedEventArgs e)
        {
            //Frame.Navigate(typeof(AboutPage));
        }

        public void Connect(int connectionId, object target)
        {
            throw new NotImplementedException();
        }
    }
}
