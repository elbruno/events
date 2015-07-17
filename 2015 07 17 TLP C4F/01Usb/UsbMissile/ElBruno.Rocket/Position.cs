namespace ElBruno.Rocket
{
    public class Position
    {
        private Rocket.HorizPos _horizontal = Rocket.HorizPos.Unknown;
        
        private Rocket.VertPos _vertical = Rocket.VertPos.Unknown;
        private int _positionY;
        private int _positionX;

        public int PositionX
        {
            get { return _positionX; }
            set { _positionX = value; }
        }

        public int PositionY
        {
            get { return _positionY; }
            set { _positionY = value; }
        }

        public Rocket.HorizPos Horizontal
        {
            get { return _horizontal; }
            set { _horizontal = value; }
        }

        public Rocket.VertPos Vertical
        {
            get { return _vertical; }
            set { _vertical = value; }
        }
    }
}
