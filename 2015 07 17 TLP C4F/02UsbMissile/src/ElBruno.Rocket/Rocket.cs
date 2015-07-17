using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using USBHIDDRIVER;

namespace ElBruno.Rocket
{
    public class Rocket
    {
        #region HorizDir enum

        public enum HorizDir
        {
            None,
            Left,
            Right
        };

        #endregion

        #region HorizPos enum

        public enum HorizPos
        {
            Unknown,
            LeftLimit,
            Middle,
            RightLimit
        };

        #endregion

        #region Speed enum

        public enum Speed
        {
            Normal,
            Slow
        };

        #endregion

        #region VertDir enum

        public enum VertDir
        {
            None,
            Up,
            Down
        };

        #endregion

        #region VertPos enum

        public enum VertPos
        {
            Unknown,
            UpLimit,
            Middle,
            DownLimit
        };

        #endregion

        #region MoveMode enum

        public enum MoveMode
        {
            Continuous,
            StepbyStep
        };

        #endregion


        private readonly byte[] _cmdData = new byte[] { 0, 0 };
        private readonly USBInterface _device;
        private readonly string _vendorId;
        private readonly MoveMode _moveMode;

        public Movement CurrentMotion = new Movement();
        public Position CurrentPosition = new Position();
        protected byte[] LastData = new byte[2];
        protected byte LastCommand = Command.Noop;

        public Rocket(string vendorID, string productID, MoveMode moveMode = MoveMode.Continuous)
        {
            _vendorId = vendorID;
            _moveMode = moveMode;
            _device = new USBInterface(_vendorId, productID);
        }

        public bool Connected { get; private set; }

        public event LimitReachedDelegate LimitReached;

        public virtual void OnLimitReached()
        {
            if (LimitReached != null)
            {
                LimitReached(new LimitReachedArgs());
            }
        }

        public event MissileFiredDelegate MissileFired;

        public virtual void OnMissileFired()
        {
            if (MissileFired != null)
            {
                MissileFired(new MissileFiredArgs());
            }
        }

        //TODO: Update this class to allow writing directly to propertise to modify movements also


        public void MoveLeft()
        {
            MoveLeft(Speed.Normal);
        }

        public void MoveLeft(Speed aSpeed)
        {
            Move(HorizDir.Left, VertDir.None, aSpeed);
            switch (aSpeed)
            {
                case Speed.Normal:
                    CurrentPosition.PositionX = CurrentPosition.PositionX - 2;
                    break;
                case Speed.Slow:
                    CurrentPosition.PositionX--;
                    break;
            }
        }

        public void MoveRight()
        {
            MoveRight(Speed.Normal);
        }

        public void MoveRight(Speed aSpeed)
        {
            Move(HorizDir.Right, VertDir.None, aSpeed);
        }

        public void MoveUp()
        {
            MoveUp(Speed.Normal);
        }

        public void MoveUp(Speed aSpeed)
        {
            Move(HorizDir.None, VertDir.Up, aSpeed);
        }

        public void MoveDown()
        {
            MoveDown(Speed.Normal);
        }

        public void MoveDown(Speed aSpeed)
        {
            Move(HorizDir.None, VertDir.Down, aSpeed);
        }

        public void Move(HorizDir aHorizDir, VertDir aVertDir)
        {
            Move(aHorizDir, aVertDir, Speed.Normal);
        }

        public void Move(HorizDir aHorizDir, VertDir aVertDir, Speed aSpeed)
        {
            // Device doesnt stop motor at limits, will just keep grinding gears away.
            // So we have to prevent the user from issuing commands that would cause this.
            if ((CurrentPosition.Horizontal == HorizPos.LeftLimit && aHorizDir == HorizDir.Left)
                || (CurrentPosition.Horizontal == HorizPos.RightLimit && aHorizDir == HorizDir.Right))
            {
                aHorizDir = HorizDir.None;
            }
            if ((CurrentPosition.Vertical == VertPos.UpLimit && aVertDir == VertDir.Up)
                || (CurrentPosition.Vertical == VertPos.DownLimit && aVertDir == VertDir.Down))
            {
                aVertDir = VertDir.None;
            }

            byte xCommand = Command.Noop;
            switch (aHorizDir)
            {
                case HorizDir.None:
                    switch (aVertDir)
                    {
                        case VertDir.None:
                            xCommand = Command.Stop;
                            aSpeed = Speed.Normal;
                            break;
                        case VertDir.Up:
                            xCommand = aSpeed == Speed.Normal ? Command.Up : Command.UpSlow;
                            break;
                        case VertDir.Down:
                            xCommand = aSpeed == Speed.Normal ? Command.Down : Command.DownSlow;
                            break;
                    }
                    break;
                case HorizDir.Left:
                    switch (aVertDir)
                    {
                        case VertDir.None:
                            xCommand = aSpeed == Speed.Normal ? Command.Left : Command.LeftSlow;
                            break;
                        case VertDir.Up:
                            xCommand = Command.UpAndLeft;
                            aSpeed = Speed.Normal;
                            break;
                        case VertDir.Down:
                            xCommand = Command.DownAndLeft;
                            aSpeed = Speed.Normal;
                            break;
                    }
                    break;
                case HorizDir.Right:
                    switch (aVertDir)
                    {
                        case VertDir.None:
                            xCommand = aSpeed == Speed.Normal ? Command.Right : Command.RightSlow;
                            break;
                        case VertDir.Up:
                            xCommand = Command.UpAndRight;
                            aSpeed = Speed.Normal;
                            break;
                        case VertDir.Down:
                            xCommand = Command.DownAndRight;
                            aSpeed = Speed.Normal;
                            break;
                    }
                    break;
            }
            CurrentMotion.Horizontal = aHorizDir;
            CurrentMotion.Vertical = aVertDir;
            CurrentMotion.Speed = aSpeed;

            if (CurrentMotion.Firing)
            {
                // performs fire in Unattended mode
                SendCmd(64, true);
                SendCmd(16, true);
                SendCmd(64, true);
                SendCmd(64, true);
                SendCmd(32, true);
            }
            else
            {
                SendCmd(xCommand);
                if (_moveMode == MoveMode.StepbyStep)
                {
                    // after a movement, sent stop if the move mode is step by step
                    xCommand = Command.Stop;
                    SendCmd(xCommand);
                }
            }

            // calculate position
            var positionDif = aSpeed == Speed.Slow ? 1 : 2;
            switch (aHorizDir)
            {
                case HorizDir.Left:
                    CurrentPosition.PositionX = CurrentPosition.PositionX - positionDif;
                    break;
                case HorizDir.Right:
                    CurrentPosition.PositionX = CurrentPosition.PositionX + positionDif;
                    break;
            }
            switch (aVertDir)
            {
                case VertDir.Down:
                    CurrentPosition.PositionY = CurrentPosition.PositionY - positionDif;
                    break;
                case VertDir.Up:
                    CurrentPosition.PositionY = CurrentPosition.PositionY + positionDif;
                    break;
            }

            Debug.Print(GetCurrentPosition());
        }

        public void Connect()
        {
            if (Connected)
            {
                return;
            }

            _device.enableUsbBufferEvent(UsbDeviceEventCacher);
            Connected = true;
        }

        public void UsbDeviceEventCacher(object sender, EventArgs e)
        {
            Console.Out.WriteLine("Event caught");
            if (USBInterface.usbBuffer.Count <= 0) return;
            const int counter = 0;
            while (USBInterface.usbBuffer[counter] == null)
            {
                //Remove this report from list
                lock (USBInterface.usbBuffer.SyncRoot)
                {
                    USBInterface.usbBuffer.RemoveAt(0);
                }
            }
            //since the remove statement at the end of the loop take the first element
            var currentRecord = (byte[])USBInterface.usbBuffer[0];
            lock (USBInterface.usbBuffer.SyncRoot)
            {
                USBInterface.usbBuffer.RemoveAt(0);
            }

            if (currentRecord == null) return;
            var msg = currentRecord.Aggregate("current record:", (current, t) => current + t);
            msg += "\r\n";
            Debug.WriteLine(msg);
        }


        public void StopAll()
        {
            CurrentMotion.Firing = false;
            StopMovements();
        }

        public void StopMovements()
        {
            Move(HorizDir.None, VertDir.None);
        }

        public void FireOnce()
        {
            StartFiring();
            // Must set after StartFiring
        }

        public void StartFiring()
        {
            CurrentMotion.Firing = true;
            Move(CurrentMotion.Horizontal, CurrentMotion.Vertical, CurrentMotion.Speed);
        }

        public void StopFiring()
        {
            CurrentMotion.Firing = false;
            Move(CurrentMotion.Horizontal, CurrentMotion.Vertical, CurrentMotion.Speed);
        }

        private void SendCmd(byte aCmd, bool sleepAfterCommand = false)
        {
            // We check against the last command to prevent rapid fire.
            // That is, in a keydown routine or polling we get the same command
            // over and over. On some slower PC's it causes the motor to "stutter" while procesing
            // if sent at certain intervals
            if (aCmd != Command.Noop && aCmd != LastCommand)
            {
                CheckConnected();
                _cmdData[1] = aCmd;
                _device.UsbDevice.writeDataSimple(_cmdData);
                if (sleepAfterCommand)
                {
                    Thread.Sleep(50);
                }
            }
        }

        //protected void SpecifiedDevice_DataRecieved(object aSender, DataRecievedEventArgs aData)
        //{
        //    //Firing
        //    //0 128
        //    //0 0
        //    //0 128
        //    //0 0
        //    // This occurs at fire - first one at end of prime?
        //    bool xTriggerLimitReachedEvent = false;

        //    var xData = aData.data;
        //    if (xData.Length != 9)
        //    {
        //        return;
        //    }
        //    if (true) DebugReceivedData(xData);

        //    HorizDir xHorizDir = CurrentMotion.Horizontal;
        //    VertDir xVertDir = CurrentMotion.Vertical;

        //    // Check for vertical status
        //    int xVertStatus = xData[1];
        //    if (xVertStatus == 64)
        //    {
        //        CurrentPosition.mVertical = VertPos.DownLimit;
        //        // Dont combine this with:
        //        // if (xVertStatus == 64) {
        //        // else when its first run and Vertical = unknown, it can allow a quick move
        //        if (CurrentMotion.Vertical == VertDir.Down)
        //        {
        //            xTriggerLimitReachedEvent = true;
        //            xVertDir = VertDir.None;
        //        }
        //    }
        //    else if (xVertStatus == 128)
        //    {
        //        CurrentPosition.mVertical = VertPos.UpLimit;
        //        if (CurrentMotion.Vertical == VertDir.Up)
        //        {
        //            xTriggerLimitReachedEvent = true;
        //            xVertDir = VertDir.None;
        //        }
        //    }
        //    else if (xVertStatus == 0)
        //    {
        //        // Don't rely on else alone - above checks also check direction, else alone will cause bug
        //        // Old ifs used to read:
        //        // } else if (xVertStatus == 128 && CurrentMotion.Vertical == VertDir.Up) {
        //        // But could fall through to here if starting (ie no direction) and then allow cracking of gears
        //        // Should not be possble now that logic has changed, however 0 is the only valid value for middle
        //        // and we should stick to that and not assume any value can be middle.
        //        CurrentPosition.mVertical = VertPos.Middle;
        //    }

        //    // Check for horiontal status
        //    int xHorizStatus = xData[2] & 15; // Important - Fire sets bit and can be combined (tested)
        //    if (xHorizStatus == 4)
        //    {
        //        CurrentPosition.mHorizontal = HorizPos.LeftLimit;
        //        if (CurrentMotion.Horizontal == HorizDir.Left)
        //        {
        //            xTriggerLimitReachedEvent = true;
        //            xHorizDir = HorizDir.None;
        //        }
        //    }
        //    else if (xHorizStatus == 8)
        //    {
        //        CurrentPosition.mHorizontal = HorizPos.RightLimit;
        //        if (CurrentMotion.Horizontal == HorizDir.Right)
        //        {
        //            xTriggerLimitReachedEvent = true;
        //            xHorizDir = HorizDir.None;
        //        }
        //    }
        //    else if (xHorizStatus == 0)
        //    {
        //        CurrentPosition.mHorizontal = HorizPos.Middle;
        //    }

        //    // Program can start up as 0 128 if shut down that way!
        //    // Because of this we need to check previous as well.
        //    if (_lastData[2] > 127 && xData[2] < 127)
        //    {
        //        OnMissileFired();
        //        if (_firingAutoStop)
        //        {
        //            StopFiring();
        //        }
        //    }

        //    // Take comiled directions, and if they are different than current motions, modify them
        //    if (xHorizDir != CurrentMotion.Horizontal || xVertDir != CurrentMotion.Vertical)
        //    {
        //        Move(xHorizDir, xVertDir, CurrentMotion.Speed);
        //    }

        //    if (xTriggerLimitReachedEvent)
        //    {
        //        OnLimitReached();
        //    }

        //    xData.CopyTo(_lastData, 0);
        //}

        private void CheckConnected()
        {
            if (!Connected)
            {
                throw new Exception("Rocket is not connected.");
            }
        }

        public string GetCurrentPosition()
        {
            return string.Format("X: {0} - Y: {1}", CurrentPosition.PositionX, CurrentPosition.PositionY);
        }
    }
}