using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using UsbLibrary;

namespace Indy.Rocket.Core
{
    public class LimitReachedArgs { };

    public delegate void LimitReachedDelegate(LimitReachedArgs e);

    public class MissileFiredArgs { };

    public delegate void MissileFiredDelegate(MissileFiredArgs e);

    public class Rocket
    {
        //TODO: Fire end detection - make seperate method, ie FireOnce
        //TODO: Events for edge Reached
        //TODO: Calibrate method, Move to XY
        UsbHidPort mDevice = new UsbHidPort();

        bool mFiringAutoStop;

        int mVendorID;
        int mProductID;

        public enum HorizDir { None, Left, Right };

        public enum VertDir { None, Up, Down };

        public enum Speed { Normal, Slow };

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
        public class Movement
        {
            internal HorizDir mHorizontal = HorizDir.None;

            public HorizDir Horizontal
            {
                get { return mHorizontal; }
            }

            internal VertDir mVertical = VertDir.None;

            public VertDir Vertical
            {
                get { return mVertical; }
            }

            internal Speed mSpeed = Speed.Normal;

            public Speed Speed
            {
                get { return mSpeed; }
            }

            internal bool mFiring = false;

            public bool Firing
            {
                get { return mFiring; }
            }
        }

        public Movement CurrentMotion = new Movement();

        public enum HorizPos { Unknown, LeftLimit, Middle, RightLimit };

        public enum VertPos { Unknown, UpLimit, Middle, DownLimit };

        public class Position
        {
            internal HorizPos mHorizontal = HorizPos.Unknown;

            public HorizPos Horizontal
            {
                get { return mHorizontal; }
            }

            internal VertPos mVertical = VertPos.Unknown;

            public VertPos Vertical
            {
                get { return mVertical; }
            }
        }

        public Position CurrentPosition = new Position();

        private class Command
        {
            public const byte Noop = 255;
            public const byte Stop = 0;
            public const byte Fire = 16;
            public const byte Up = 1;
            public const byte Down = 2;
            public const byte Left = 4;
            public const byte Right = 8;
            public const byte UpSlow = 13;
            public const byte DownSlow = 14;
            public const byte LeftSlow = 7;
            public const byte RightSlow = 11;
            public const byte UpAndLeft = Up + Left;
            public const byte UpAndRight = Up + Right;
            public const byte DownAndLeft = Down + Left;
            public const byte DownAndRight = Down + Right;
        }

        public void MoveLeft()
        {
            MoveLeft(Speed.Normal);
        }

        public void MoveLeft(Speed aSpeed)
        {
            Move(HorizDir.Left, VertDir.None, aSpeed);
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

        protected byte mLastCommand = Command.Noop;

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
            CurrentMotion.mHorizontal = aHorizDir;
            CurrentMotion.mVertical = aVertDir;
            CurrentMotion.mSpeed = aSpeed;

            if (CurrentMotion.mFiring)
            {
                if (xCommand == Command.Noop)
                {
                    xCommand = Command.Stop;
                }
                xCommand = (byte)(xCommand + Command.Fire);
            }
            SendCmd(xCommand);
        }

        bool mConnected = false;

        public bool Connected
        {
            get { return mConnected; }
        }

        public Rocket()
            : this(0x1941, 0x8021)
        {
        }

        public Rocket(int aVendorID, int aProductID)
        {
            mVendorID = aVendorID;
            mProductID = aProductID;
            mDevice.OnSpecifiedDeviceArrived += new EventHandler(this.usb_OnSpecifiedDeviceArrived);
            mDevice.OnSpecifiedDeviceRemoved += new EventHandler(this.usb_OnSpecifiedDeviceRemoved);
        }

        public void Connect()
        {
            if (mConnected)
            {
                return;
            }

            mDevice.VendorId = mVendorID;
            mDevice.ProductId = mProductID;
            mDevice.CheckDevicePresent();
            mDevice.SpecifiedDevice.DataRecieved += new DataRecievedEventHandler(this.SpecifiedDevice_DataRecieved);
            mConnected = true;
        }

        public void StopAll()
        {
            CurrentMotion.mFiring = false;
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
            mFiringAutoStop = true;
        }

        public void StartFiring()
        {
            CurrentMotion.mFiring = true;
            mFiringAutoStop = false;
            Move(CurrentMotion.Horizontal, CurrentMotion.Vertical, CurrentMotion.Speed);
        }

        public void StopFiring()
        {
            CurrentMotion.mFiring = false;
            Move(CurrentMotion.Horizontal, CurrentMotion.Vertical, CurrentMotion.Speed);
        }

        byte[] mCmdData = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        private void SendCmd(byte aCmd)
        {
            // We check against the last command to prevent rapid fire.
            // That is, in a keydown routine or polling we get the same command
            // over and over. On some slower PC's it causes the motor to "stutter" while procesing
            // if sent at certain intervals
            if (aCmd != Command.Noop && aCmd != mLastCommand)
            {
                if (mDevice.SpecifiedDevice == null)
                {
                    throw new Exception("Device not present");
                }
                CheckConnected();
                mCmdData[1] = aCmd;
                mDevice.SpecifiedDevice.SendData(mCmdData);
            }
        }

        private string mLastDataString = "";

        private void DebugReceivedData(byte[] aData)
        {
            string s = "";
            foreach (byte x in aData)
            {
                s = s + x.ToString() + " ";
            }
            if (s != mLastDataString)
            {
                mLastDataString = s;
                System.Diagnostics.Debug.WriteLine(DateTime.Now.ToString() + ": " + s);
            }
        }

        //TODO: This event is called from a thread - need to synchronize event
        // Its working now by chance, but I didnt know until later this event was threaded
        protected byte[] mLastData = new byte[9];

        protected void SpecifiedDevice_DataRecieved(object aSender, DataRecievedEventArgs aData)
        {
            //Firing
            //0 128
            //0 0
            //0 128
            //0 0
            // This occurs at fire - first one at end of prime?
            bool xTriggerLimitReachedEvent = false;

            var xData = aData.data;
            if (xData.Length != 9)
            {
                return;
            }
            if (true) DebugReceivedData(xData);

            HorizDir xHorizDir = CurrentMotion.Horizontal;
            VertDir xVertDir = CurrentMotion.Vertical;

            // Check for vertical status
            int xVertStatus = xData[1];
            if (xVertStatus == 64)
            {
                CurrentPosition.mVertical = VertPos.DownLimit;
                // Dont combine this with:
                // if (xVertStatus == 64) {
                // else when its first run and Vertical = unknown, it can allow a quick move
                if (CurrentMotion.Vertical == VertDir.Down)
                {
                    xTriggerLimitReachedEvent = true;
                    xVertDir = VertDir.None;
                }
            }
            else if (xVertStatus == 128)
            {
                CurrentPosition.mVertical = VertPos.UpLimit;
                if (CurrentMotion.Vertical == VertDir.Up)
                {
                    xTriggerLimitReachedEvent = true;
                    xVertDir = VertDir.None;
                }
            }
            else if (xVertStatus == 0)
            {
                // Don't rely on else alone - above checks also check direction, else alone will cause bug
                // Old ifs used to read:
                // } else if (xVertStatus == 128 && CurrentMotion.Vertical == VertDir.Up) {
                // But could fall through to here if starting (ie no direction) and then allow cracking of gears
                // Should not be possble now that logic has changed, however 0 is the only valid value for middle
                // and we should stick to that and not assume any value can be middle.
                CurrentPosition.mVertical = VertPos.Middle;
            }

            // Check for horiontal status
            int xHorizStatus = xData[2] & 15; // Important - Fire sets bit and can be combined (tested)
            if (xHorizStatus == 4)
            {
                CurrentPosition.mHorizontal = HorizPos.LeftLimit;
                if (CurrentMotion.Horizontal == HorizDir.Left)
                {
                    xTriggerLimitReachedEvent = true;
                    xHorizDir = HorizDir.None;
                }
            }
            else if (xHorizStatus == 8)
            {
                CurrentPosition.mHorizontal = HorizPos.RightLimit;
                if (CurrentMotion.Horizontal == HorizDir.Right)
                {
                    xTriggerLimitReachedEvent = true;
                    xHorizDir = HorizDir.None;
                }
            }
            else if (xHorizStatus == 0)
            {
                CurrentPosition.mHorizontal = HorizPos.Middle;
            }

            // Program can start up as 0 128 if shut down that way!
            // Because of this we need to check previous as well.
            if (mLastData[2] > 127 && xData[2] < 127)
            {
                OnMissileFired();
                if (mFiringAutoStop)
                {
                    StopFiring();
                }
            }

            // Take comiled directions, and if they are different than current motions, modify them
            if (xHorizDir != CurrentMotion.Horizontal || xVertDir != CurrentMotion.Vertical)
            {
                Move(xHorizDir, xVertDir, CurrentMotion.Speed);
            }

            if (xTriggerLimitReachedEvent)
            {
                OnLimitReached();
            }

            xData.CopyTo(mLastData, 0);
        }

        private void CheckConnected()
        {
            if (!mConnected)
            {
                throw new Exception("Rocket is not connected.");
            }
        }

        private void usb_OnSpecifiedDeviceArrived(object sender, EventArgs e)
        {
            mConnected = true;
        }

        private void usb_OnSpecifiedDeviceRemoved(object sender, EventArgs e)
        {
            mConnected = false;
        }
    }
}