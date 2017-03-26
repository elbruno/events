using Lego.Ev3.Core;
using Lego.Ev3.Desktop;
using System.Windows;

namespace ElBruno.LegoEv3.Controller
{
    public partial class MainWindow : Window
    {
        private Brick brick;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            brick = new Brick(new BluetoothCommunication("COM4"));

            await brick.ConnectAsync();
            await brick.DirectCommand.PlayToneAsync(0x50, 5000, 500);
        }

        async void Move(bool forward = true)
        {
            int speed = 50;
            if (!forward) speed = -50;

            brick.BatchCommand.TurnMotorAtSpeedForTime(OutputPort.B, speed, 1000, false);
            brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, speed, 1000, false);
            await brick.BatchCommand.SendCommandAsync();
        }
    }
}
