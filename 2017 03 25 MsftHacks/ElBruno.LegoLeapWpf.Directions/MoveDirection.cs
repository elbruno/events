using System.ComponentModel;
using System.Runtime.CompilerServices;
using ElBruno.LegoLeapWpf.Directions.Annotations;

namespace ElBruno.LegoLeapWpf.Directions
{
    public class MoveDirection : INotifyPropertyChanged
    {
        private HandMoveDirection _x;
        private HandMoveDirection _y;
        private HandMoveDirection _z;
        private HandMoveDirection _combinedXy;

        public MoveDirection()
            : this(0, 0, 0)
        {

        }
        public MoveDirection(float x, float y, float z, int xCoeficient = 1, int yCoeficient = 1, int zCoeficient = 1)
        {
            X = DirectionCalculator.CalculateDirectionX(x, xCoeficient);
            Y = DirectionCalculator.CalculateDirectionY(y, yCoeficient);
            Z = DirectionCalculator.CalculateDirectionZ(z, zCoeficient);
            CombinedXy = GetCombinedDirectionForXAndYAxis();
        }

        #region Properties

        public bool IsFarMode
        {
            get
            {
                var ret = false || (X == HandMoveDirection.LeftFar || X == HandMoveDirection.RightFar || Y == HandMoveDirection.UpFar || Y == HandMoveDirection.DownFar);
                return ret;
            }
        }
        public HandMoveDirection CombinedXy
        {
            get { return _combinedXy; }
            set
            {
                if (value == _combinedXy) return;
                _combinedXy = value;
                OnPropertyChanged();
            }
        }

        public HandMoveDirection Z
        {
            get { return _z; }
            set
            {
                if (value == _z) return;
                _z = value;
                OnPropertyChanged();
            }
        }

        public HandMoveDirection X
        {
            get { return _x; }
            set
            {
                if (value == _x) return;
                _x = value;
                GetCombinedDirectionForXAndYAxis();
                OnPropertyChanged();
            }
        }

        public HandMoveDirection Y
        {
            get { return _y; }
            set
            {
                if (value == _y) return;
                _y = value;
                GetCombinedDirectionForXAndYAxis();
                OnPropertyChanged();
            }
        }
        #endregion

        public HandMoveDirection GetCombinedDirectionForXAndYAxis()
        {
            var ret = HandMoveDirection.Center;

            // UP
            if (X == HandMoveDirection.RightFar && Y == HandMoveDirection.UpFar)
            {
                ret = HandMoveDirection.UpRightFar;
            }
            else if ((X == HandMoveDirection.Right || X == HandMoveDirection.RightFar) &&
                (Y == HandMoveDirection.Up || Y == HandMoveDirection.UpFar))
            {
                ret = HandMoveDirection.UpRight;
            }
            else if (X == HandMoveDirection.Center && Y == HandMoveDirection.UpFar)
            {
                ret = HandMoveDirection.UpFar;
            }
            else if (X == HandMoveDirection.Center && Y == HandMoveDirection.Up)
            {
                ret = HandMoveDirection.Up;
            }
            else if (X == HandMoveDirection.LeftFar && Y == HandMoveDirection.UpFar)
            {
                ret = HandMoveDirection.UpLeftFar;
            }
            else if ((X == HandMoveDirection.LeftFar || X == HandMoveDirection.Left) &&
                (Y == HandMoveDirection.Up || Y == HandMoveDirection.UpFar))
            {
                ret = HandMoveDirection.UpLeft;
            }

                // CENTER
            else if (X == HandMoveDirection.RightFar && Y == HandMoveDirection.Center)
            {
                ret = HandMoveDirection.RightFar;
            }
            else if (X == HandMoveDirection.Right && Y == HandMoveDirection.Center)
            {
                ret = HandMoveDirection.Right;
            }
            else if (X == HandMoveDirection.Center && Y == HandMoveDirection.Center)
            {
                ret = HandMoveDirection.Center;
            }
            else if (X == HandMoveDirection.LeftFar && Y == HandMoveDirection.Center)
            {
                ret = HandMoveDirection.LeftFar;
            }
            else if (X == HandMoveDirection.Left && Y == HandMoveDirection.Center)
            {
                ret = HandMoveDirection.Left;
            }

                // DOWN
            else if (X == HandMoveDirection.RightFar && Y == HandMoveDirection.DownFar)
            {
                ret = HandMoveDirection.DownRightFar;
            }
            else if ((X == HandMoveDirection.RightFar || X == HandMoveDirection.Right) &&
                (Y == HandMoveDirection.Down || Y == HandMoveDirection.DownFar))
            {
                ret = HandMoveDirection.DownRight;
            }
            else if (X == HandMoveDirection.Center && Y == HandMoveDirection.DownFar)
            {
                ret = HandMoveDirection.DownFar;
            }
            else if (X == HandMoveDirection.Center && Y == HandMoveDirection.Down)
            {
                ret = HandMoveDirection.Down;
            }
            else if (X == HandMoveDirection.LeftFar && Y == HandMoveDirection.DownFar)
            {
                ret = HandMoveDirection.DownLeftFar;
            }
            else if ((X == HandMoveDirection.LeftFar || X == HandMoveDirection.Left) &&
                (Y == HandMoveDirection.Down || Y == HandMoveDirection.DownFar))
            {
                ret = HandMoveDirection.DownLeft;
            }
            return ret;
        }

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
}
