using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Security.Cryptography.Certificates;
using Windows.Storage.Streams;

namespace WuaBleRs02.Drone
{
    public class ParrotSendData : ParrotBase
    {
        private GattDeviceService _service;
        private byte _motorCounter = 0;
        private bool _shouldRun;
        private int _settingsCounter = 1;

        public async void SendTestData(List<DeviceInformation> parrotDevices)
        {
            DeviceInformation motorDevice = null;
            foreach (var device in parrotDevices)
            {
                Debug.WriteLine("device " + device.Id);

                // get device service
                _service = await GattDeviceService.FromIdAsync(device.Id);
                if (null == _service) return;

                try
                {

                //var characteristics = _service.GetCharacteristics(RollingSpiderCharacteristicUuids.Parrot_PowerMotors);
                var characteristics = _service.GetAllCharacteristics();
                TakeOff(characteristics);
                var res = await Motors(false, 0, 0, 0, 0, 0.0f, 18, characteristics);
                if (!res) continue;
                res = await Motors(true, 0, 100, 0, 0, 0.0f, 6, characteristics);
                if (res) continue;
                EmergencyStop(characteristics);
                }
                catch (Exception exception)
                {
                    Debug.WriteLine(exception);
                }
            }

        }

        public void Test02()
        {

        }


        public async Task<bool> Motors(bool @on, int tilt, int forward, int turn, int up, float scale, int steps, IReadOnlyList<GattCharacteristic> characteristics)
        {
            for (var i = 0; i < steps; i++)
            {
                await SendMotorCommand(@on, tilt, forward, turn, up, scale, characteristics);
                if (_shouldRun) continue;
                await SendMotorCommand(false, 0, 0, 0, 0, 0.0f, characteristics); // stop it
                return false; // TODO replace by exception
            }
            return true;
        }

        public async Task<bool> SendMotorCommand(bool @on, int tilt, int forward, int turn, int up, float scale, IReadOnlyList<GattCharacteristic> characteristics)
        {
            var res = false;
            var characteristic = characteristics[0];
            try
            {
                Debug.WriteLine("    Send Motor Command");
                Debug.WriteLine("    Try to write to " + CharacteristicUuidsResolver.GetNameFromUuid(characteristic.Uuid));
                Debug.WriteLine("    Char props" + characteristic.CharacteristicProperties);

                var writer = new DataWriter();
                writer.WriteByte(2);
                writer.WriteByte((byte)_motorCounter);
                writer.WriteByte(2);
                writer.WriteByte(0);
                writer.WriteByte(2);
                writer.WriteByte(0);
                if (on)
                {
                    writer.WriteByte(1);
                }
                else
                {
                    writer.WriteByte(0);
                }
                // is byte casting necessary???
                writer.WriteByte((byte)(tilt & 0xFF));
                writer.WriteByte((byte)(forward & 0xFF));
                writer.WriteByte((byte)(turn & 0xFF));
                writer.WriteByte((byte)(up & 0xFF));
                writer.WriteDouble(scale); // well, but I need different endian :(

                await characteristic.WriteValueAsync(writer.DetachBuffer());
                Debug.WriteLine("      Write sucessfull");
                res = true;
            }
            catch (Exception exception)
            {
                Debug.WriteLine("      Write error");
            }
            return res;
        }
        public void EmergencyStop(IReadOnlyList<GattCharacteristic> characteristics)
        {
            //var characteristics = _service.GetCharacteristics(RollingSpiderCharacteristicUuids.Parrot_EmergencyStop);
            byte[] commandToWrite = { 4, (byte)_settingsCounter, 2, 0, 4, 0 };
            SendCommandTo1StChar(characteristics, commandToWrite);
            _settingsCounter++;
        }
        public void TakeOff(IReadOnlyList<GattCharacteristic> characteristics)
        {
            //var characteristics = _service.GetCharacteristics(RollingSpiderCharacteristicUuids.Parrot_DateTime);
            byte[] commandToWrite = { 4, (byte)_settingsCounter, 2, 0, 1, 0 };
            SendCommandTo1StChar(characteristics, commandToWrite);
            _settingsCounter++;
        }

        public void Land()
        {
            var characteristics = _service.GetCharacteristics(RollingSpiderCharacteristicUuids.Parrot_DateTime);
            byte[] commandToWrite = { 4, (byte)_settingsCounter, 2, 0, 3, 0 };
            SendCommandTo1StChar(characteristics, commandToWrite);
            _settingsCounter++;
        }

        public async void SendCommandTo1StChar(IReadOnlyList<GattCharacteristic> characteristics, byte[] commandToWrite)
        {
            var characteristic = characteristics[0];

            Debug.WriteLine("    Send Command to First Char");
            Debug.WriteLine("    Try to write to " + CharacteristicUuidsResolver.GetNameFromUuid(characteristic.Uuid));
            Debug.WriteLine("    Char props" + characteristic.CharacteristicProperties);

            try
            {
                var writer = new DataWriter();
                writer.WriteBytes(commandToWrite);
                await characteristic.WriteValueAsync(writer.DetachBuffer());
                Debug.WriteLine("      Write sucessfull");
            }
            catch (Exception exception)
            {
                Debug.WriteLine("      Write error");
            }

        }
    }
}

// Write with transactions, does not work
//var gattTransaction = new GattReliableWriteTransaction();
//var writer = new DataWriter();
//writer.WriteBytes(commandToWrite);
//gattTransaction.WriteValue(characteristic, writer.DetachBuffer());
//var status = await gattTransaction.CommitAsync();
//switch (status)
//{
//    case GattCommunicationStatus.Success:
//        //StatusInformation2 = "Writing to your device OK !";
//        break;
//    case GattCommunicationStatus.Unreachable:
//        //StatusInformation2 = "Writing to your device failed !";
//        break;
//}
