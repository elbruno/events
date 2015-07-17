using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Leap;
using LeapWpf02.Properties;

namespace ElBruno.LeapWpMotion
{
    public partial class MainWindow : INotifyPropertyChanged
    {
        private string _moveXText;
        private string _moveYText;
        private string _moveZText;
        private Controller _controller;
        private MotionListener _listener;
        private string _fingersText;
        private string _handMoveVector;
        private string _nAme;

        public MainWindow()
        {
            DataContext = this;
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            InitializeComponent();
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            _controller.RemoveListener(_listener);
            _controller.Dispose();
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _listener = new MotionListener();
            _controller = new Controller();
            _controller.AddListener(_listener);
            _listener.OnFingersCount += _listener_OnFingersCount;
            _listener.OnHandMoveOnX += _listener_OnHandMoveOnX;
            _listener.OnHandMoveOnY += _listener_OnHandMoveOnY;
            _listener.OnHandMoveOnZ += _listener_OnHandMoveOnZ;
            _listener.OnHandMoveOn += _listener_OnHandMoveOn;
        }

        void _listener_OnHandMoveOn(Leap.Vector obj)
        {
            HandMoveVector = string.Format("x:{0} y:{1} z:{2}{3}x:{4} y:{5} z:{6}", obj.x, obj.y, obj.z, Environment.NewLine, obj.x * 10, obj.y * 10, obj.z * 100);
        }

        void _listener_OnHandMoveOnZ(MotionListener.HandMoveDirection obj)
        {
            MoveZText = obj.ToString();
        }
        void _listener_OnHandMoveOnY(MotionListener.HandMoveDirection obj)
        {
            MoveXText = obj.ToString();
        }

        void _listener_OnHandMoveOnX(MotionListener.HandMoveDirection obj)
        {
            MoveYText = obj.ToString();
        }

        void _listener_OnFingersCount(int fingersCount)
        {
            FingersText = "fingers: " + fingersCount;
        }

        #region Properties

        public string HandMoveVector
        {
            get
            {
                return _handMoveVector;
            }
            set
            {
                if (value == _handMoveVector ) return;
                _handMoveVector = value;
                OnPropertyChanged();
            }
        }

        public string MoveZText
        {
            get
            {
                return _moveZText;
            }

            set
            {
                if (value == _moveZText) return;
                _moveZText = value;
                OnPropertyChanged();
            }
        }

        public string MoveXText
        {
            get
            {
                return _moveXText;
            }

            set
            {
                if (value == _moveXText) return;
                _moveXText = value;
                OnPropertyChanged();
            }
        }

        public string MoveYText
        {
            get
            {
                return _moveYText;
            }

            set
            {
                if (value == _moveYText) return;
                _moveYText = value;
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
                if (value == _fingersText ) return;
                _fingersText = value;
                OnPropertyChanged();
            }
        }

        //public event PropertyChangedEventHandler PropertyChanged;

        //[NotifyPropertyChangedInvocator]
        //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    PropertyChangedEventHandler handler = PropertyChanged;
        //    if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        [Annotations.NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
