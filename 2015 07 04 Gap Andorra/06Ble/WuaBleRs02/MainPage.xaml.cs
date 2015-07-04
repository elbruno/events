using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml;
using WuaBleRs02.Annotations;
using WuaBleRs02.Drone;
using WuaBleRs02.Model;

namespace WuaBleRs02
{
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        private List<DeviceInformation> _parrotDrones;
        private readonly ParrotLoadDevices _parrotLoadDevices;
        private readonly ParrotCharacteristics _parrotCharacteristics;
        private readonly ParrotSendData _parrotSendData;

        public MainPage()
        {
            InitializeComponent();
            DataContext = this;
            _logData = new List<LogAction>();
            _parrotLoadDevices = new ParrotLoadDevices();
            _parrotCharacteristics = new ParrotCharacteristics();
            _parrotSendData = new ParrotSendData();
            Loaded += MainPage_Loaded;
        }
        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ButtonLoadData_Click(null, null);
        }
        private async void ButtonLoadData_Click(object sender, RoutedEventArgs e)
        {
            StatusInformation = "Start load parrot devices";
            _parrotDrones = await _parrotLoadDevices.LoadParrotDevices();
            //_parrotDrones = await _parrotLoadDevices.LoadAllDevices();
            MergeLogActions(_parrotLoadDevices.LogActions);
            StatusInformation = "Finished load parrot devices";
        }
        private void ButtonSendLoadCharsClick(object sender, RoutedEventArgs e)
        {
            StatusInformation = "Start load drones characteristics";
            _parrotCharacteristics.LoadDevicesCharacteristics(_parrotDrones);
            MergeLogActions(_parrotCharacteristics.LogActions);
            StatusInformation = "End load drones characteristics";
        }
        private void ButtonSendTestDataClick(object sender, RoutedEventArgs e)
        {
            StatusInformation = "Start send test data";
            _parrotSendData.SendTestData(_parrotDrones);
            MergeLogActions(_parrotSendData.LogActions);
            StatusInformation = "End send test data";
        }

        private void MergeLogActions(IEnumerable<LogAction> logActions)
        {
            foreach (var logAction in logActions)
            {
                LogData.Add(logAction);
            }
            listViewLog.ItemsSource = LogData;
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

        private string _statusInformation3;
        public string StatusInformation3
        {
            get { return _statusInformation3; }
            set
            {
                if (value == _statusInformation3) return;
                _statusInformation3 = value;
                OnPropertyChanged();
            }
        }

        private List<LogAction> _logData;
        public List<LogAction> LogData
        {
            get { return _logData; }
            set
            {
                if (value == _logData) return;
                _logData = value;
                OnPropertyChanged();
            }
        }
        #endregion

    }
}
