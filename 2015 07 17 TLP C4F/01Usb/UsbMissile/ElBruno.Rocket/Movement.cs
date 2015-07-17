namespace ElBruno.Rocket
{
    public class Movement
    {
        private bool _firing;
        private Rocket.HorizDir _horizontal = Rocket.HorizDir.None;
        private Rocket.Speed _speed = Rocket.Speed.Normal;
        private Rocket.VertDir _vertical = Rocket.VertDir.None;

        public Rocket.HorizDir Horizontal
        {
            get { return _horizontal; }
            set { _horizontal = value; }
        }

        public Rocket.VertDir Vertical
        {
            get { return _vertical; }
            set { _vertical = value; }
        }

        public Rocket.Speed Speed
        {
            get { return _speed; }
            set { _speed = value; } 
        }

        public bool Firing
        {
            get { return _firing; }
            set { _firing = value; }
        }
    }
}
