namespace ElBruno.LegoLeapWpf.Directions
{
    public class DirectionCalculator
    {
        public static HandMoveDirection CalculateDirectionZ(float z, int zCoeficient)
        {
            var locationZ = HandMoveDirection.Center;
            var currentLocationZ = z * zCoeficient;
            if (currentLocationZ > 97)
            {
                locationZ = HandMoveDirection.Forward;
            }
            else if (currentLocationZ < 97)
            {
                locationZ = HandMoveDirection.Backwards;
            }
            return locationZ;
        }

        public static HandMoveDirection CalculateDirectionY(float y, int yCoeficient)
        {
            var locationY = HandMoveDirection.DownFar;
            var currentLocationY = y * yCoeficient;
            if (currentLocationY > 350)
            {
                locationY = HandMoveDirection.UpFar;
            }
            else if (currentLocationY > 250)
            {
                locationY = HandMoveDirection.Up;
            }
            else if (currentLocationY > 150)
            {
                locationY = HandMoveDirection.Center;
            }
            else if (currentLocationY > 100)
            {
                locationY = HandMoveDirection.Down;
            }
            return locationY;
        }

        public static HandMoveDirection CalculateDirectionX(float x, int xCoeficient)
        {
            var locationX = HandMoveDirection.LeftFar;
            var currentLocationX = x * xCoeficient;
            if (currentLocationX > 100)
            {
                locationX = HandMoveDirection.RightFar;
            }
            else if (currentLocationX > 50)
            {
                locationX = HandMoveDirection.Right;
            }
            else if (currentLocationX > -50)
            {
                locationX = HandMoveDirection.Center;
            }
            else if (currentLocationX > -100)
            {
                locationX = HandMoveDirection.Left;
            }
            return locationX;
        }

    }
}
