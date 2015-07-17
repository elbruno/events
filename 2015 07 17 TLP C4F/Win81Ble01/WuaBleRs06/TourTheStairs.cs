using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using WuaBleRollingSpider.Drone;

namespace WuaBleRs06
{
    public class TourTheStairs : ParrotBase
    {
        private readonly GattCharacteristic _motorChar;
        private readonly GattCharacteristic _datetimeChar;
        private readonly GattCharacteristic _initCount1To20Char;
        private readonly GattCharacteristic _emergencyStopChar;
        private bool _shouldRun = true;
        private int _motorCounter = 1;
        private int _settingsCounter = 1;
        private int _emergencyCounter = 1;
        private byte _battery = 1; // unknown
        private byte _status = 1; // unknown
        private string _debugInfo = "";

        public TourTheStairs(GattCharacteristic motorChar, GattCharacteristic datetimeChar,
            GattCharacteristic initCount1To20Char, GattCharacteristic emergencyStopChar)
        {
            _motorChar = motorChar;
            _datetimeChar = datetimeChar;
            _initCount1To20Char = initCount1To20Char;
            _emergencyStopChar = emergencyStopChar;
        }

        public async Task<bool> Sleep(int ms)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(ms));
            }
            catch
            {
                // ignored
            }
            return true;
        }

        public string Info()
        {
            return "Bat: " + _battery + "% (" + _status + ") " + _debugInfo;
        }

        public async Task<bool> Init()
        {
            var res = true;
            ClearExceptions();
            for (int i = 0; i < 20; i++)
            {
                try
                {
                    var value = new byte[3];
                    value[0] = 0x1;
                    value[1] = (byte)(i + 1);
                    value[2] = (byte)(i + 1);
                    var buffer = new DataWriter();
                    buffer.WriteBytes(value);
                    await _initCount1To20Char.WriteValueAsync(buffer.DetachBuffer(), GattWriteOption.WriteWithoutResponse);
                    await Sleep(50);
                }
                catch (Exception exception)
                {
                    res = false;
                    AddException(exception);
                }
            }
            return res;
        }

        public async Task<bool> Takeoff()
        {
            var result = true;
            ClearExceptions();
            try
            {
                byte[] arr = { 4, (byte)_settingsCounter, 2, 0, 1, 0 };
                var buffer = new DataWriter();
                buffer.WriteBytes(arr);
                await _datetimeChar.WriteValueAsync(buffer.DetachBuffer(), GattWriteOption.WriteWithoutResponse);
            }
            catch (Exception exception)
            {
                AddException(exception);
                result = false;
            }
            await Sleep(50);
            _settingsCounter++;
            return result;
        }

        public async Task<bool> Land()
        {
            byte[] arr = { 4, (byte)_settingsCounter, 2, 0, 3, 0 };
            var buffer = new DataWriter();
            buffer.WriteBytes(arr);
            await _datetimeChar.WriteValueAsync(buffer.DetachBuffer(), GattWriteOption.WriteWithoutResponse);
            await Sleep(50);
            _settingsCounter++;
            return true;
        }

        public async Task<bool> EmergencyStop()
        {
            // dangerous - stops all motors!
            byte[] arr = { 4, (byte)_emergencyCounter, 2, 0, 4, 0 };
            var buffer = new DataWriter();
            buffer.WriteBytes(arr);
            await _emergencyStopChar.WriteValueAsync(buffer.DetachBuffer(), GattWriteOption.WriteWithoutResponse);
            await Sleep(50);
            _emergencyCounter++;
            return true;
        }

        public async Task<bool> SendMotorCmd(bool on, int tilt, int forward, int turn, int up, float scale)
        {
            var packet = new DataWriter();
            try
            {
                packet.WriteByte(2);
                packet.WriteByte((byte)_motorCounter);
                packet.WriteByte(2);
                packet.WriteByte(0);
                packet.WriteByte(2);
                packet.WriteByte(0);
                if (on)
                {
                    packet.WriteByte(1);
                }
                else
                {
                    packet.WriteByte(0);
                }
                // is byte casting necessary???
                packet.WriteByte((byte)(tilt & 0xFF));
                packet.WriteByte((byte)(forward & 0xFF));
                packet.WriteByte((byte)(turn & 0xFF));
                packet.WriteByte((byte)(up & 0xFF));
                packet.WriteDouble(scale); // well, but I need different endian :(
                                           //byte[] tmpArr = stream.toByteArray();
                                           //byte tmp;
                                           //tmp = tmpArr[11]; // temporary hack - swapping float ordering
                                           //tmpArr[11] = tmpArr[14];
                                           //tmpArr[14] = tmp;
                                           //tmp = tmpArr[12];
                                           //tmpArr[12] = tmpArr[13];
                                           //tmpArr[13] = tmp;
                                           //characteristics.setValue(tmpArr);
                await _motorChar.WriteValueAsync(packet.DetachBuffer(), GattWriteOption.WriteWithoutResponse);
            }
            catch (Exception exception)
            {
                AddException(exception);
            }

            await Sleep(50);
            _motorCounter++;
            return true;
        }

        public async Task<bool> Motors(bool on, int tilt, int forward, int turn, int up, float scale, int steps)
        {
            for (int i = 0; i < steps; i++)
            {
                await SendMotorCmd(on, tilt, forward, turn, up, scale);
                if (!_shouldRun)
                {
                    await SendMotorCmd(false, 0, 0, 0, 0, 0.0f); // stop it
                    return false; // TODO replace by exception
                }
            }
            return true;
        }

        public void RequestStop()
        {
            _shouldRun = false;
        }

        public bool Completed()
        {
            return !_shouldRun;
        }

        public void NewInputs(byte[] data)
        {
            if (data.Length == 7 && data[3] == 5 && data[4] == 1 && data[5] == 0)
                _battery = data[6];
            if (data.Length == 10 && data[3] == 3 && data[4] == 1 && data[5] == 0)
                _status = data[6];
        }

        public async Task<bool> Ver0()
        {
            await Takeoff();
            if (await Motors(false, 0, 0, 0, 0, 0.0f, 18) == false) return false;
            if (await Motors(true, 0, 100, 0, 0, 0.0f, 6) == false) return false;
            EmergencyStop();
            return await Motors(false, 0, 0, 0, 0, 0.0f, 10);
        }

        public async Task<bool> ApproachStep()
        {
            // TODO use if _spiralStaircase
            if (await Motors(true, 0, 40, 0, 0, 0.0f, 10) == false) return false;
            if (await Motors(true, 0, 10, 0, 0, 0.0f, 20) == false) return false;
            return await Motors(false, 0, 0, 0, 0, 0.0f, 10);
        }

        public async Task<bool> Ver1()
        {
            for (int i = 0; i < 100; i++)
            {
                if (await Ver0() == false)
                {
                    EmergencyStop();
                    break;
                }
                if (await ApproachStep() == false)
                    break;
            }
            return true;
        }

        public async Task<bool> TestTakeoffLand()
        {
            //takeoff & land
            await Takeoff();
            while (_status == -1 || _status == 0 || _status == 1)
                await Motors(false, 0, 0, 0, 0, 0.0f, 1);
            await Land();
            while (_status == 1)
                await Motors(false, 0, 0, 0, 0, 0.0f, 1);
            await Motors(false, 0, 0, 0, 0, 0.0f, 20); // it has to land anyway
            return true;
        }

        public async Task<bool> Ver2()
        {
            var res = true;
            ClearExceptions();
            //takeoff & land
            res = await Takeoff();
            //while (_status == -1 || _status == 0 || _status == 1)
            //    await Motors(false, 0, 0, 0, 0, 0.0f, 1);
            await Motors(true, 0, 10, 0, 0, 0.0f, 15);
            for (int i = 0; i < 100; i++)
            {
                if (await Motors(true, 0, 10, 0, 0, 0.0f, 20) == false)
                    break;
            }
            await Land();
            while (_status == 1)
            {
                await Land();
                await Motors(false, 0, 0, 0, 0, 0.0f, 1);
            }
            await Motors(false, 0, 0, 0, 0, 0.0f, 20); // it has to land anyway

            return res;
        }


        public async Task<bool> Run()
        {
            await Init();
            await Ver0();
            _shouldRun = false;
            _debugInfo = "END";
            return true;
        }
    }
}