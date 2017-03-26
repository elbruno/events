using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Controls;
using System.Windows.Input;
using ElBruno.LegoLeapWpf.Annotations;
using ElBruno.LegoLeapWpf.Custom;
using ElBruno.LegoLeapWpf.Directions;
using Leap;
using Lego.Ev3.Core;
using Lego.Ev3.Desktop;

namespace ElBruno.LegoLeapWpf
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private Controller _controller;
        private MotionListener _listener;
        private string _fingersText;
        private string _handMoveVector;
        private Brick _brick;
        private DateTime _nextStatusUpdate = DateTime.MinValue;
        private string _statusText;
        private string _legoStatus;
        private string _legoMovementInformation;

        public MainWindow()
        {
            DataContext = this;
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            InitializeComponent();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            _controller.Dispose();
            StopLegoEv3();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitLegoControlButtons();
            InitLeapSensorAndRegisterToEvents();
        }

        private void _brick_BrickChanged(object sender, BrickChangedEventArgs e)
        {
            LegoStatus = "LEGO EV3 Connected";
        }

        private void InitLeapSensorAndRegisterToEvents()
        {
            _listener = new MotionListener();
            _controller = new Controller();

            _controller.Connect += _listener.OnServiceConnect;
            _controller.Device += _listener.OnConnect;
            _controller.FrameReady += _listener.OnFrame;

            _listener.OnHandMoveOn += _listener_OnHandMoveOn;
        }

        private void _listener_OnHandMoveOn(Leap.Vector obj, MoveDirection moveDirection, int fingersCount)
        {
            HandMoveVector = string.Format("Combined XY: {1}{0}X: {2} Y: {3} Z: {4}{0}fingers:{5}", Environment.NewLine,
                moveDirection.CombinedXy, moveDirection.X, moveDirection.Y, moveDirection.Z, fingersCount);
            HandMoveVector += string.Format("{3}x:{0} y:{1} z:{2}{3}x:{4} y:{5} z:{6}", obj.x, obj.y, obj.z,
                Environment.NewLine, obj.x * 10, obj.y * 10, obj.z * 100);
            bool fire = false || fingersCount == 0;
            Dispatcher.Invoke(() => DisplayMovementsInChart(moveDirection, fire));

            if (DateTime.Now < _nextStatusUpdate) return;
            _nextStatusUpdate = DateTime.Now + TimeSpan.FromSeconds(1);
            StatusText = string.Format("Next Process Lego in:{0}", _nextStatusUpdate);
            Dispatcher.Invoke(() => ProcessLegoMovements(moveDirection, fire));
        }

        private void DisplayMovementsInChart(MoveDirection moveDirection, bool fire)
        {
            InitLegoControlButtons();
            switch (moveDirection.CombinedXy)
            {
                // UP
                case HandMoveDirection.UpLeftFar:
                    Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonTopLeft, Wpf.LegoButtonStyle.leapFar);
                    break;
                case HandMoveDirection.UpLeft:
                    Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonTopLeft, Wpf.LegoButtonStyle.leap);
                    break;
                case HandMoveDirection.UpFar:
                    Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonTop, Wpf.LegoButtonStyle.leapFar);
                    break;
                case HandMoveDirection.Up:
                    Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonTop, Wpf.LegoButtonStyle.leap);
                    break;
                case HandMoveDirection.UpRightFar:
                    Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonTopRight, Wpf.LegoButtonStyle.leapFar);
                    break;
                case HandMoveDirection.UpRight:
                    Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonTopRight, Wpf.LegoButtonStyle.leap);
                    break;

                // CENTER
                case HandMoveDirection.LeftFar:
                    Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonCenterLeft, Wpf.LegoButtonStyle.leapFar);
                    break;
                case HandMoveDirection.Left:
                    Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonCenterLeft, Wpf.LegoButtonStyle.leap);
                    break;
                case HandMoveDirection.Center:
                    Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonCenter, Wpf.LegoButtonStyle.leap);
                    break;
                case HandMoveDirection.RightFar:
                    Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonCenterRight, Wpf.LegoButtonStyle.leapFar);
                    break;
                case HandMoveDirection.Right:
                    Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonCenterRight, Wpf.LegoButtonStyle.leap);
                    break;

                // DOWN
                case HandMoveDirection.DownLeftFar:
                    Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonDownLeft, Wpf.LegoButtonStyle.leapFar);
                    break;
                case HandMoveDirection.DownLeft:
                    Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonDownLeft, Wpf.LegoButtonStyle.leap);
                    break;
                case HandMoveDirection.DownFar:
                    Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonDown, Wpf.LegoButtonStyle.leapFar);
                    break;
                case HandMoveDirection.Down:
                    Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonDown, Wpf.LegoButtonStyle.leap);
                    break;
                case HandMoveDirection.DownRightFar:
                    Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonDownRight, Wpf.LegoButtonStyle.leapFar);
                    break;
                case HandMoveDirection.DownRight:
                    Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonDownRight, Wpf.LegoButtonStyle.leap);
                    break;
            }
            if (fire)
            {
                Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonFire, Wpf.LegoButtonStyle.leap);
            }
        }

        #region Lego Ev3

        private void StopLegoEv3()
        {
            try
            {
                if (null != _brick)
                {
                    _brick.Disconnect();
                    _brick = null;
                }
            }
            catch (Exception exception)
            {
                LegoStatus = string.Format("Cannot connect to LEGO EV3. Details:{0}", exception.ToString());
            }
        }

        private async void ProcessLegoMovements(MoveDirection moveDirection, bool fire)
        {
            
            if (_brick == null) return;
            int speedB = 50;
            int speedC = 50;


            switch (moveDirection.CombinedXy)
            {
                // UP
                case HandMoveDirection.UpLeft:
                    speedB = 25;
                    speedC = 50;
                    break;
                case HandMoveDirection.Up:
                    speedB = 50;
                    speedC = 50;
                    break;
                case HandMoveDirection.UpRight:
                    speedB = 50;
                    speedC = 25;
                    break;

                // CENTER
                case HandMoveDirection.Left:
                    speedB = -50;
                    speedC = 50;
                    break;
                case HandMoveDirection.Center:
                    speedB = 0;
                    speedC = 0;
                    break;
                case HandMoveDirection.Right:
                    speedB = 50;
                    speedC = -50;
                    break;

                // DOWN
                case HandMoveDirection.DownLeft:
                    speedB = -25;
                    speedC = -50;
                    break;
                case HandMoveDirection.Down:
                    speedB = -50;
                    speedC = -50;
                    break;
                case HandMoveDirection.DownRight:
                    speedB = -50;
                    speedC = -25;
                    break;
            }

            if (moveDirection.IsFarMode)
            {
                speedB = speedB * 2;
                speedC = speedC * 2;
            }

            _brick.BatchCommand.TurnMotorAtSpeedForTime(OutputPort.B, speedB, 1000, false);
            _brick.BatchCommand.TurnMotorAtPowerForTime(OutputPort.C, speedC, 1000, false);
            await _brick.BatchCommand.SendCommandAsync();


            LegoMovementInformation = string.Format("Direction:{0} -SpeedB:{1} -SpeedC:{2}", moveDirection.CombinedXy, speedB, speedC);
        }


        private async void ButtonConnect_OnClick(object sender, RoutedEventArgs e)
        {
            if (null == _brick)
            {
                _brick = new Brick(new BluetoothCommunication("COM4"));
                _brick.BrickChanged += _brick_BrickChanged;

                try
                {
                    await _brick.ConnectAsync();
                    ButtonConnect.Visibility = Visibility.Collapsed;
                    ButtonDisconnect.Visibility = Visibility.Visible;
                }
                catch (Exception exception)
                {
                    LegoStatus = string.Format("Cannot connect to LEGO EV3. Details:{0}", exception.ToString());
                }

            }
            else
            {
                StopLegoEv3();
                ButtonConnect.Visibility = Visibility.Visible;
                ButtonDisconnect.Visibility = Visibility.Collapsed;
            }
        }

        private void ButtonLegoStop_OnClick(object sender, RoutedEventArgs e)
        {
            StopLegoEv3();
        }
        #endregion

        #region Lego Button Controller
        private void LegoControlButtonOnMouseEnter(object sender, MouseEventArgs e)
        {
            Wpf.ChangeButtonStyleBasedOnMouseInteraction(sender, Wpf.LegoButtonStyle.hover);
        }

        private void LegoControlButtonOnMouseLeave(object sender, MouseEventArgs e)
        {
            Wpf.ChangeButtonStyleBasedOnMouseInteraction(sender, Wpf.LegoButtonStyle.normal);
        }

        private void LegoControlButtonOnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;
            var buttonName = button.Name;
            var moveDirection = new MoveDirection(0, 0, 0);
            var fire = false;
            if (buttonName == "ButtonTopLeft")
            {
                moveDirection.CombinedXy = HandMoveDirection.UpLeft;
            }
            else if (buttonName == "ButtonTop")
            {
                moveDirection.CombinedXy = HandMoveDirection.Up;
            }
            else if (buttonName == "ButtonTopRight")
            {
                moveDirection.CombinedXy = HandMoveDirection.UpRight;
            }
            else if (buttonName == "ButtonCenterLeft")
            {
                moveDirection.CombinedXy = HandMoveDirection.Left;
            }
            else if (buttonName == "ButtonCenter")
            {
                moveDirection.CombinedXy = HandMoveDirection.Center;
            }
            else if (buttonName == "ButtonCenterRight")
            {
                moveDirection.CombinedXy = HandMoveDirection.Right;
            }
            else if (buttonName == "ButtonDownLeft")
            {
                moveDirection.CombinedXy = HandMoveDirection.DownLeft;
            }
            else if (buttonName == "ButtonDown")
            {
                moveDirection.CombinedXy = HandMoveDirection.Down;
            }
            else if (buttonName == "ButtonDownRight")
            {
                moveDirection.CombinedXy = HandMoveDirection.DownRight;
            }
            else if (buttonName == "ButtonFire")
            {
                moveDirection.CombinedXy = HandMoveDirection.Center;
                fire = true;
            }
            ProcessLegoMovements(moveDirection, true);

        }

        private void InitLegoControlButtons()
        {
            Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonFire);
            Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonTopRight);
            Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonTop);
            Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonTopLeft);
            Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonCenterRight);
            Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonCenter);
            Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonCenterLeft);
            Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonDownRight);
            Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonDown);
            Wpf.ChangeButtonStyleBasedOnMouseInteraction(ButtonDownLeft);
        }
        #endregion

        #region Properties

        public string LegoMovementInformation
        {
            get { return _legoMovementInformation; }
            set
            {
                if (value == _legoMovementInformation) return;
                _legoMovementInformation = value;
                OnPropertyChanged();
            }
        }

        public string LegoStatus
        {
            get { return _legoStatus; }
            set
            {
                if (value == _legoStatus) return;
                _legoStatus = value;
                OnPropertyChanged();
            }
        }

        public string StatusText
        {
            get
            {
                return _statusText;
            }
            set
            {
                if (value == _statusText) return;
                _statusText = value;
                OnPropertyChanged();
            }
        }

        public string HandMoveVector
        {
            get
            {
                return _handMoveVector;
            }
            set
            {
                if (value == _handMoveVector) return;
                _handMoveVector = value;
                OnPropertyChanged();
            }
        }

        public string FingersText
        {
            get
            {
                return _fingersText;
            }

            set
            {
                if (value == _fingersText) return;
                _fingersText = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Notify Property Changed
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }

    public class MotionListener // : Listener
    {
        public event Action<Leap.Vector, MoveDirection, int> OnHandMoveOn;

        public void OnServiceConnect(object sender, ConnectionEventArgs args)
        {
            Console.WriteLine("Service Connected");
        }

        public void OnConnect(object sender, DeviceEventArgs args)
        {
            Console.WriteLine("Connected");
        }

        public void OnFrame(object sender, FrameEventArgs args)
        {
            Console.WriteLine("Frame Available.");
            var frame = args.frame;

            if (frame.Hands.Count == 0) return;
            var hand = frame.Hands[0];
            var direction = hand.StabilizedPalmPosition;
            var moveDirection = new MoveDirection(hand.StabilizedPalmPosition.x, hand.StabilizedPalmPosition.y, hand.StabilizedPalmPosition.z);
            Task.Factory.StartNew(() => OnHandMoveOn(direction, moveDirection, hand.Fingers.Count));
        }
    }
}
