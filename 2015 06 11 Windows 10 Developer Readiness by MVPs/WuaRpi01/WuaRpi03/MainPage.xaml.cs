using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WuaRpi03.Annotations;

namespace WuaRpi03
{
    public sealed partial class MainPage : INotifyPropertyChanged
    {
        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            myCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Pen | Windows.UI.Core.CoreInputDeviceTypes.Touch | Windows.UI.Core.CoreInputDeviceTypes.Mouse;
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
        #endregion
    }
}
