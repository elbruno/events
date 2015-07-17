using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ElBruno.LegoLeapWpf.Annotations;
using ElBruno.LegoLeapWpf.Custom;
using ElBruno.LegoLeapWpf.Directions;
using Leap;
using Vector = Leap.Vector;

namespace ElBruno.LegoLeapWpf
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private Controller _controller;
        private MotionListener _listener;
        private string _fingersText;
        private string _handMoveVector;
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

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            InitLeapControlButtons();
            InitLeapSensorAndRegisterToEvents();
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            _controller.RemoveListener(_listener);
            _controller.Dispose();
        }

        private void InitLeapSensorAndRegisterToEvents()
        {
            _listener = new MotionListener();
            _controller = new Controller();
            _controller.AddListener(_listener);
            _listener.OnHandMoveOn += _listener_OnHandMoveOn;
        }

        private void _listener_OnHandMoveOn(Vector obj, MoveDirection moveDirection, int fingersCount)
        {
            HandMoveVector = string.Format("Combined XY: {1}{0}X: {2} Y: {3} Z: {4}{0}fingers:{5}", Environment.NewLine,
                moveDirection.CombinedXy, moveDirection.X, moveDirection.Y, moveDirection.Z, fingersCount);
            HandMoveVector += string.Format("{3}x:{0} y:{1} z:{2}{3}x:{4} y:{5} z:{6}", obj.x, obj.y, obj.z,
                Environment.NewLine, obj.x * 10, obj.y * 10, obj.z * 100);
            bool fire = false; //fingersCount == 0;
            Dispatcher.Invoke(() => DisplayMovementsInChart(moveDirection, fire));

            if (DateTime.Now < _nextStatusUpdate) return;
            _nextStatusUpdate = DateTime.Now + TimeSpan.FromSeconds(3);
            StatusText = $"Next Process Lego in:{_nextStatusUpdate}";
            Dispatcher.Invoke(() => ProcessLeapMovements(moveDirection, fire));
        }

        private void DisplayMovementsInChart(MoveDirection moveDirection, bool fire)
        {
            InitLeapControlButtons();
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


        #region Leap Button Controller

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (Rocket == null)
            {
                Rocket = new Rocket.Rocket(@"vid_0a81", @"pid_ff01");
                Rocket.Connect();
            }
            else
            {
                Rocket.StopAll();
                Rocket.StopMovements();
                Rocket = null;
            }

        }

        private void LeapControlButtonOnMouseEnter(object sender, MouseEventArgs e)
        {
            Wpf.ChangeButtonStyleBasedOnMouseInteraction(sender, Wpf.LegoButtonStyle.hover);
        }

        private void LeapControlButtonOnMouseLeave(object sender, MouseEventArgs e)
        {
            Wpf.ChangeButtonStyleBasedOnMouseInteraction(sender);
        }

        private void LeapControlButtonOnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;
            var buttonName = button.Name;
            var moveDirection = new MoveDirection(0, 0, 0);
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
            }
            ProcessLeapMovements(moveDirection, true);

        }

        private void ProcessLeapMovements(MoveDirection moveDirection, bool fire)
        {
            if (Rocket == null || !Rocket.Connected) return;
            sbyte speed = 30;
            if (moveDirection.IsFarMode)
                speed = 50;

            switch (moveDirection.CombinedXy)
            {
                // UP
                case HandMoveDirection.UpLeft:
                case HandMoveDirection.UpRight:
                case HandMoveDirection.Up:
                    Rocket.MoveUp();
                    CurrentPosition = Rocket.GetCurrentPosition();
                    break;

                // CENTER
                case HandMoveDirection.Left:
                    Rocket.MoveLeft();
                    CurrentPosition = Rocket.GetCurrentPosition();
                    break;
                case HandMoveDirection.Center:
                    Rocket.StopAll();
                    break;
                case HandMoveDirection.Right:
                    Rocket.MoveRight();
                    CurrentPosition = Rocket.GetCurrentPosition();
                    break;

                // DOWN
                case HandMoveDirection.DownLeft:
                case HandMoveDirection.DownRight:
                case HandMoveDirection.Down:
                    Rocket.MoveDown();
                    CurrentPosition = Rocket.GetCurrentPosition();
                    break;
            }

            if (fire)
            {
                Rocket.FireOnce();

            }

            LegoMovementInformation = $"Direction:{moveDirection.CombinedXy} -Speed:{speed} -Fire:{fire}";
        }

        private void InitLeapControlButtons()
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

        public Rocket.Rocket Rocket
        {
            get { return _rocket; }
            set
            {
                if (Equals(value, _rocket)) return;
                _rocket = value;
                CurrentPosition = Rocket.GetCurrentPosition();
                OnPropertyChanged();
            }
        }
        private string _currentPosition;
        private Rocket.Rocket _rocket;
        public string CurrentPosition
        {
            get { return _currentPosition; }
            set
            {
                if (value == _currentPosition) return;
                _currentPosition = value;
                OnPropertyChanged();
            }
        }

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
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
