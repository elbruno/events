using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Win10Rpi2_Sample01
{
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer _timer;

        public MainPage()
        {
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //_timer = new DispatcherTimer();
            //_timer.Interval = TimeSpan.FromSeconds(1);
            //_timer.Tick += _timer_Tick;
            //_timer.Start();
        }

        private void _timer_Tick(object sender, object e)
        {
            TextBlockMessage.Text = DateTime.Now.ToString();
        }
    }
}
