using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ElBruno.LegoLeapWpf.Directions.Tests
{
    [TestClass]
    public class DirectionCalculatorTests
    {
        const int DefaultCoeficient = 1;
        const float ValueXRight = 51f;
        const float ValueXMaxCenter = 50f;
        const float ValueXMinCenter = -50f;
        const float ValueXLeft = -51;
        
        const float ValueYUp = 251f;
        const float ValueYMaxCenter = 250f;
        const float ValueYMinCenter = 150f;
        const float ValueYDown= 149f;

        // DIRECTION X
        [TestMethod]
        public void Calculate_Xdirection_for_bigger_50_equal_right()
        {
            var ret = DirectionCalculator.CalculateDirectionX(ValueXRight, DefaultCoeficient);
            Assert.AreEqual(HandMoveDirection.Right, ret);
        }
        [TestMethod]
        public void Calculate_Xdirection_for_50_equal_center()
        {
            var ret = DirectionCalculator.CalculateDirectionX(ValueXMaxCenter, DefaultCoeficient);
            Assert.AreEqual(HandMoveDirection.Center, ret);
        }
        [TestMethod]
        public void Calculate_Xdirection_for_minus50_equal_center()
        {
            var ret = DirectionCalculator.CalculateDirectionX(ValueXMinCenter, DefaultCoeficient);
            Assert.AreEqual(HandMoveDirection.Center, ret);
        }
        [TestMethod]
        public void Calculate_Xdirection_for_smaller_minus50_equal_left()
        {
            var ret = DirectionCalculator.CalculateDirectionX(ValueXLeft, DefaultCoeficient);
            Assert.AreEqual(HandMoveDirection.Left, ret);
        }

        // DIRECTION Y
        [TestMethod]
        public void Calculate_Ydirection_for_bigger_250_equal_up()
        {
            var ret = DirectionCalculator.CalculateDirectionY(ValueYUp, DefaultCoeficient);
            Assert.AreEqual(HandMoveDirection.Up, ret);
        }
        [TestMethod]
        public void Calculate_Ydirection_for_250_equal_center()
        {
            var ret = DirectionCalculator.CalculateDirectionY(ValueYMaxCenter, DefaultCoeficient);
            Assert.AreEqual(HandMoveDirection.Center, ret);
        }
        [TestMethod]
        public void Calculate_Ydirection_for_150_equal_center()
        {
            var ret = DirectionCalculator.CalculateDirectionY(ValueYMinCenter, DefaultCoeficient);
            Assert.AreEqual(HandMoveDirection.Center, ret);
        }
        [TestMethod]
        public void Calculate_Ydirection_for_smaller_150_equal_down()
        {
            var ret = DirectionCalculator.CalculateDirectionY(ValueYDown, DefaultCoeficient);
            Assert.AreEqual(HandMoveDirection.Down, ret);
        }
    }
}
