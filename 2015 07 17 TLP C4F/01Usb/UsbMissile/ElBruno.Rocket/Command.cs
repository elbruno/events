namespace ElBruno.Rocket
{
    public class Command
    {
        public const byte Noop = 255;
        public const byte Stop = 0;
        public const byte Fire = 32;
        public const byte Up = 2;
        public const byte Down = 1;
        public const byte Left = 4;
        public const byte Right = 8;
        public const byte UpSlow = 2;
        public const byte DownSlow = 1;
        public const byte LeftSlow = 4;
        public const byte RightSlow = 8;
        public const byte UpAndLeft = Up + Left;
        public const byte UpAndRight = Up + Right;
        public const byte DownAndLeft = Down + Left;
        public const byte DownAndRight = Down + Right;
    }
}