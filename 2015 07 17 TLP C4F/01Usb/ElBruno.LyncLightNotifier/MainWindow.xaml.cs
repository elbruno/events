using System;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using ElBruno.LightNotifier;
using Microsoft.Lync.Controls;

namespace ElBruno.LyncLightNotifier
{
    public partial class MainWindow
    {
        private readonly Timer _timer = new Timer(1000);
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindowLoaded;
            Unloaded += MainWindowUnloaded;
        }

        void MainWindowUnloaded(object sender, RoutedEventArgs e)
        {
            _timer.Elapsed -= TimerElapsed;
            _timer.Stop();
        }

        void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            _timer.Elapsed += TimerElapsed;
            _timer.Enabled = true;
        }

        void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(CheckLyncState));
        }
        void CheckLyncState()
        {
            var state = myPresence.AvailabilityState;
            var lightController = new LightController();
            lightController.TurnLight(state == ContactAvailability.DoNotDisturb);
        }
    }
}
