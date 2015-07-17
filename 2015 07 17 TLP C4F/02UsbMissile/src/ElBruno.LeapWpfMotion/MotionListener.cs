using System;
using System.Threading.Tasks;
using Leap;

namespace ElBruno.LeapWpMotion
{
    public class MotionListener : Listener
    {
        public enum HandMoveDirection
        {
            Forward, Backwards, Up, Down, Right, Left, Center
        }

        public event Action<int> OnFingersCount;
        public event Action<HandMoveDirection> OnHandMoveOnX;
        public event Action<HandMoveDirection> OnHandMoveOnY;
        public event Action<HandMoveDirection> OnHandMoveOnZ;
        public event Action<Vector> OnHandMoveOn;

        public override void OnFrame(Controller controller)
        {
            var frame = controller.Frame();
            
            if (frame.Hands.IsEmpty) return;
            var hand = frame.Hands[0];
            Task.Factory.StartNew(() => OnFingersCount(hand.Fingers.Count));
            var direction = hand.Direction;
            Task.Factory.StartNew(() => OnHandMoveOn(direction));
            CalculateDirectionX(direction);
            CalculateDirectionY(direction);
            CalculateDirectionZ(direction);
        }

        private void CalculateDirectionZ(Vector direction)
        {
            var locationZ = HandMoveDirection.Center;
            var currentLocationZ = direction.z * 100;
            if (currentLocationZ > 97)
            {
                locationZ = HandMoveDirection.Forward;
            }
            else if (currentLocationZ < 97)
            {
                locationZ = HandMoveDirection.Backwards;
            }
            Task.Factory.StartNew(() => OnHandMoveOnZ(locationZ));
        }

        private void CalculateDirectionY(Vector direction)
        {
            var locationY = HandMoveDirection.Center;
            var currentLocationY = direction.y * 10;
            if (currentLocationY > 1)
            {
                locationY = HandMoveDirection.Up;
            }
            else if (currentLocationY < 0)
            {
                locationY = HandMoveDirection.Down;
            }
            Task.Factory.StartNew(() => OnHandMoveOnY(locationY));
        }

        private void CalculateDirectionX(Vector direction)
        {
            var locationX = HandMoveDirection.Center;
            var currentLocationX = direction.x * 10;
            if (currentLocationX > 1)
            {
                locationX = HandMoveDirection.Right;
            }
            else if (currentLocationX < 0)
            {
                locationX = HandMoveDirection.Left;
            }
            Task.Factory.StartNew(() => OnHandMoveOnX(locationX));
        }
    }
}