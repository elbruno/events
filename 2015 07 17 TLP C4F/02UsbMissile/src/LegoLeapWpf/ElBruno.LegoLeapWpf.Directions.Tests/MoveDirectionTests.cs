using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ElBruno.LegoLeapWpf.Directions.Tests
{
    [TestClass]
    public class MoveDirectionTests
    {
        //UP
        [TestMethod]
        public void combinedXy_equal_upLeftFar_when_X_leftFar_Y_upFar()
        {
            InitMoveDirectionAndAssert(HandMoveDirection.LeftFar, HandMoveDirection.UpFar, HandMoveDirection.UpLeftFar);
        }
        [TestMethod]
        public void combinedXy_equal_upLeft_when_X_leftFar_Y_up()
        {
            InitMoveDirectionAndAssert(HandMoveDirection.LeftFar, HandMoveDirection.Up, HandMoveDirection.UpLeft);
        }
        [TestMethod]
        public void combinedXy_equal_upLeft_when_X_left_Y_upFar()
        {
            InitMoveDirectionAndAssert(HandMoveDirection.Left, HandMoveDirection.UpFar, HandMoveDirection.UpLeft);
        }
        [TestMethod]
        public void combinedXy_equal_upLeft_when_X_left_Y_up()
        {
            InitMoveDirectionAndAssert(HandMoveDirection.Left, HandMoveDirection.Up, HandMoveDirection.UpLeft);
        }
        [TestMethod]
        public void combinedXy_equal_upFar_when_X_center_Y_upFar()
        {
            InitMoveDirectionAndAssert(HandMoveDirection.Center, HandMoveDirection.UpFar, HandMoveDirection.UpFar);
        }
        [TestMethod]
        public void combinedXy_equal_up_when_X_center_Y_up()
        {
            InitMoveDirectionAndAssert(HandMoveDirection.Center, HandMoveDirection.Up, HandMoveDirection.Up);
        }
        [TestMethod]
        public void combinedXy_equal_upRightFar_when_X_rightFar_Y_upFar()
        {
            InitMoveDirectionAndAssert(HandMoveDirection.RightFar, HandMoveDirection.UpFar, HandMoveDirection.UpRightFar);
        }
        [TestMethod]
        public void combinedXy_equal_upRight_when_X_right_Y_up()
        {
            InitMoveDirectionAndAssert(HandMoveDirection.Right, HandMoveDirection.Up, HandMoveDirection.UpRight);
        }

        // CENTER
        [TestMethod]
        public void combinedXy_equal_left_when_X_left_Y_up()
        {
            InitMoveDirectionAndAssert(HandMoveDirection.Left, HandMoveDirection.Center, HandMoveDirection.Left);
        }
        [TestMethod]
        public void combinedXy_equal_center_when_X_center_Y_center()
        {
            InitMoveDirectionAndAssert(HandMoveDirection.Center, HandMoveDirection.Center, HandMoveDirection.Center);
        }
        [TestMethod]
        public void combinedXy_equal_right_when_X_right_Y_center()
        {
            InitMoveDirectionAndAssert(HandMoveDirection.Right, HandMoveDirection.Center, HandMoveDirection.Right);
        }

        // DOWN
        [TestMethod]
        public void combinedXy_equal_downLeft_when_X_left_Y_down()
        {
            InitMoveDirectionAndAssert(HandMoveDirection.Left, HandMoveDirection.Down, HandMoveDirection.DownLeft);
        }
        [TestMethod]
        public void combinedXy_equal_down_when_X_center_Y_down()
        {
            InitMoveDirectionAndAssert(HandMoveDirection.Center, HandMoveDirection.Down, HandMoveDirection.Down);
        }
        [TestMethod]
        public void combinedXy_equal_downRight_when_X_right_Y_down()
        {
            InitMoveDirectionAndAssert(HandMoveDirection.Right, HandMoveDirection.Down, HandMoveDirection.DownRight);
        }

        private static void InitMoveDirectionAndAssert(HandMoveDirection handMoveDirectionX, HandMoveDirection handMoveDirectionY, HandMoveDirection handMoveDirection)
        {
            var md = new MoveDirection()
            {
                X = handMoveDirectionX,
                Y = handMoveDirectionY
            };
            Assert.AreEqual(handMoveDirection, md.CombinedXy);
        }
    }
}
