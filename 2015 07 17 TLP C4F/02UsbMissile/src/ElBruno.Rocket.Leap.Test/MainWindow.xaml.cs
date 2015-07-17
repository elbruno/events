using System.ComponentModel;
using System.Windows;
using ElBruno.Rocket.Leap.Test.Properties;

namespace ElBruno.Rocket.Leap.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public Rocket Rocket
        {
            get { return _rocket; }
            set
            {
                if (Equals(value, _rocket)) return;
                _rocket = value;
                CurrentPosition = Rocket.GetCurrentPosition();
                OnPropertyChanged("Rocket");
            }
        }

        private string _currentPosition;
        private Rocket _rocket;

        public string CurrentPosition
        {
            get { return _currentPosition; }
            set
            {
                if (value == _currentPosition) return;
                _currentPosition = value;
                OnPropertyChanged("CurrentPosition");
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            DataContext = this;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Rocket = new Rocket(@"vid_0a81", @"pid_ff01");
            Rocket.Connect();
        }

        private void buttonUp_Click(object sender, RoutedEventArgs e)
        {
            Rocket.MoveUp();
            CurrentPosition = Rocket.GetCurrentPosition();
        }

        private void buttonLeft_Click(object sender, RoutedEventArgs e)
        {
            Rocket.MoveLeft();
            CurrentPosition = Rocket.GetCurrentPosition();
        }

        private void buttonFire_Click(object sender, RoutedEventArgs e)
        {
            Rocket.FireOnce();
        }

        private void buttonRight_Click(object sender, RoutedEventArgs e)
        {
            Rocket.MoveRight();
            CurrentPosition = Rocket.GetCurrentPosition();
        }

        private void buttonDown_Click(object sender, RoutedEventArgs e)
        {
            Rocket.MoveDown();
            CurrentPosition = Rocket.GetCurrentPosition();
        }

        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            Rocket.StopAll();
        }

        private void CheckBoxMoveMode_OnValueChanged(object sender, RoutedEventArgs e)
        {
            if (CheckBoxMoveMode.IsChecked == true )
            {
                Rocket = new Rocket(@"vid_0a81", @"pid_ff01", Rocket.MoveMode.Continuous);
                Rocket.Connect();
            }
            else
            {
                Rocket = new Rocket(@"vid_0a81", @"pid_ff01", Rocket.MoveMode.StepbyStep);
                Rocket.Connect();

            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}