using System;
using System.Diagnostics;
using System.Threading.Tasks;
using ElBruno.LegoLeapWpf.Directions;
using Leap;

namespace ElBruno.LegoLeapWpf
{
    public class MotionListener : Listener
    {
        public event Action<Vector, MoveDirection, int> OnHandMoveOn;

        public override void OnFrame(Controller controller)
        {
            var frame = controller.Frame();
            
            if (frame.Hands.IsEmpty) return;
            var hand = frame.Hands[0];
            var direction = hand.StabilizedPalmPosition;
            var moveDirection = new MoveDirection(hand.StabilizedPalmPosition.x, hand.StabilizedPalmPosition.y, hand.StabilizedPalmPosition.z);
            Task.Factory.StartNew(() => OnHandMoveOn(direction, moveDirection, hand.Fingers.Count));
        }
    }
}