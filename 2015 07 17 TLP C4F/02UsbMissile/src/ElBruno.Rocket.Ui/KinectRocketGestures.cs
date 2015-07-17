using Microsoft.Kinect;

namespace ElBruno.Rocket.Ui
{
    class KinectRocketGestures
    {
        private readonly Skeleton _skeleton;
        private readonly Rocket _rocket;

        public KinectRocketGestures(Skeleton skeleton, Rocket rocket)
        {
            _skeleton = skeleton;
            _rocket = rocket;
        }

        public string ValidateGestures()
        {
            var gesture = @"NOT DEFINED";
            // STOP
            // Right hand and Left hand hanging at the side
            if (_skeleton.Joints[JointType.HandRight].Position.Y < _skeleton.Joints[JointType.HipCenter].Position.Y &&
                _skeleton.Joints[JointType.HandLeft].Position.Y < _skeleton.Joints[JointType.HipCenter].Position.Y)
            {
                _rocket.StopAll();
                _rocket.StopFiring();
                _rocket.StopMovements();
                gesture = @"STOP";
                return gesture;
            }

            // FIRE
            if (_skeleton.Joints[JointType.HandLeft].Position.Y > _skeleton.Joints[JointType.Head].Position.Y)
            {
                gesture = @"FIRE";
                _rocket.FireOnce();
            }

            // MOVE RIGHT OR LEFT
            // Right hand in front of right shoulder
            // Right hand below shoulder height but above hip height
            if (
                (_skeleton.Joints[JointType.HandRight].Position.Z < _skeleton.Joints[JointType.ElbowRight].Position.Z &&
                _skeleton.Joints[JointType.HandLeft].Position.Y < _skeleton.Joints[JointType.HipCenter].Position.Y)
                &&
                (_skeleton.Joints[JointType.HandRight].Position.Y < _skeleton.Joints[JointType.Head].Position.Y &&
                _skeleton.Joints[JointType.HandRight].Position.Y > _skeleton.Joints[JointType.HipCenter].Position.Y)
                )
            {
                // Right hand right of right shoulder
                if (_skeleton.Joints[JointType.HandRight].Position.X > _skeleton.Joints[JointType.ShoulderRight].Position.X)
                {
                    gesture = @"MOVE RIGHT";
                    _rocket.MoveRight();
                }
                // Right hand left of left Shoulder
                if (_skeleton.Joints[JointType.HandRight].Position.X < _skeleton.Joints[JointType.ShoulderLeft].Position.X)
                {
                    gesture = @"MOVE LEFT";
                    _rocket.MoveLeft();
                }
            }

            // MOVE UP OR DOWN
            // Right hand in front of body with Left hand hanging at the side
            // Right hand between shoulders
            if (
                (_skeleton.Joints[JointType.HandRight].Position.Z < _skeleton.Joints[JointType.ShoulderCenter].Position.Z &&
                _skeleton.Joints[JointType.HandLeft].Position.Y < _skeleton.Joints[JointType.HipCenter].Position.Y)
                &&
                (_skeleton.Joints[JointType.HandRight].Position.X < _skeleton.Joints[JointType.ShoulderRight].Position.X &&
                _skeleton.Joints[JointType.HandRight].Position.X > _skeleton.Joints[JointType.ShoulderLeft].Position.X)
                )
            {
                // Right hand above the shoulders
                if (_skeleton.Joints[JointType.HandRight].Position.Y > _skeleton.Joints[JointType.ShoulderCenter].Position.Y)
                {
                    gesture = @"MOVE UP";
                    _rocket.MoveUp();
                }
                // Right hand below the chest/gut
                if (_skeleton.Joints[JointType.HandRight].Position.Y < _skeleton.Joints[JointType.Spine].Position.Y)
                {
                    gesture = @"MOVE DOWN";
                    _rocket.MoveDown();
                }
            }
            return gesture;
        }
    }
}
