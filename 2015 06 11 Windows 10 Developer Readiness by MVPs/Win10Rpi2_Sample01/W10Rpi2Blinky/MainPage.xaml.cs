using System;
using System.Diagnostics;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;

namespace W10Rpi2Blinky
{
    public sealed partial class MainPage
    {
        private DispatcherTimer _timerBlink;

        public MainPage()
        {
            InitGpioController();
            InitializeComponent();
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            _timerBlink = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000) };
            _timerBlink.Tick += _timerBlink_Tick;
            _timerBlink.Start();
        }

        private GpioPin _pin;
        private bool _ledStatus;

        private void _timerBlink_Tick(object sender, object e)
        {
            TextBlockStatus.Text = $"Led Status is {_ledStatus}";
            _pin?.Write(_ledStatus ? GpioPinValue.High : GpioPinValue.Low);
            _ledStatus = !_ledStatus;
        }

        private void InitGpioController()
        {
            var gpio = GpioController.GetDefault();
            if (gpio == null)
            {
                _pin = null;
                return;
            }

            GpioOpenStatus openstatus;
            gpio.TryOpenPin(LedPin, GpioSharingMode.Exclusive, out _pin, out openstatus);
            Debug.WriteLine("pin: " + LedPin + " status: " + openstatus);

            if (_pin == null)
            {
                return;
            }

            _pin.Write(GpioPinValue.High);
            _pin.SetDriveMode(GpioPinDriveMode.Output);
        }
        private const int LedPin = 6;

    }
}
