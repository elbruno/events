using System;
using System.Collections.Generic;
using System.IO;

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
            foreach (var device in parrotDevices.Where(device => device.Id == ParrotConstants.DeviceIdMotor))
            {
                motorDevice = device;
            }
            if (motorDevice == null) return;

            // get device service
            _service = await GattDeviceService.FromIdAsync(motorDevice.Id);
            if (null == _service) return;

            // get service characteristics
            Test02();
        }

        public void Test02()
        {
            if (_service == null) return;
            var cs = _service.GetAllCharacteristics();
            byte[] commandToWrite = { 4, 1, 2, 0, 1, 0 };
            SendCommandToAllChar(cs, commandToWrite);
            _settingsCounter++;
        }


        public bool Test01()
        {
            //TakeOff();
            if (!Motors(false, 0, 0, 0, 0, 0.0f, 18)) return false;
            if (!Motors(true, 0, 100, 0, 0, 0.0f, 6)) return false;
            EmergencyStop();
            return Motors(false, 0, 0, 0, 0, 0.0f, 10);
        }

        public bool Motors(bool on, int tilt, int forward, int turn, int up, float scale, int steps)
        {
            for (var i = 0; i < steps; i++)
            {
                SendMotorCommand(on, tilt, forward, turn, up, scale);
                if (_shouldRun) continue;
                SendMotorCommand(false, 0, 0, 0, 0, 0.0f); // stop it
                return false; // TODO replace by exception
            }
            return true;
        }

        public async void SendMotorCommand(Boolean on, int tilt, int forward, int turn, int up, float scale)
        {
            var characteristics = _service.GetCharacteristics(RollingSpiderCharacteristicUuids.Parrot_PowerMotors);
            var characteristic = characteristics[0];
            var writer = new DataWriter();
            try
            {
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
                //writer.WriteDouble(scale); // well, but I need different endian :(

                await characteristic.WriteValueAsync(writer.DetachBuffer());
            }
            catch (IOException e)
            {
                Debug.WriteLine(e);
            }

            //var gattTransaction = new GattReliableWriteTransaction();
            //gattTransaction.WriteValue(characteristic, writer.DetachBuffer());
            //var status = await gattTransaction.CommitAsync();
            //switch (status)
            //{
            //    case GattCommunicationStatus.Success:
            //        AddLogAction("Writing to your device OK !");
            //        break;
            //    case GattCommunicationStatus.Unreachable:
            //        AddLogAction("Writing to your device Failed !");
            //        break;
            //}

        }
        public void EmergencyStop()
        {
            var characteristics = _service.GetCharacteristics(RollingSpiderCharacteristicUuids.Parrot_EmergencyStop);
            byte[] commandToWrite = { 4, (byte)_settingsCounter, 2, 0, 4, 0 };
            SendCommandTo1StChar(characteristics, commandToWrite);
            _settingsCounter++;
        }
        public void TakeOff()
        {
            var characteristics = _service.GetCharacteristics(RollingSpiderCharacteristicUuids.Parrot_DateTime);
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

            Debug.WriteLine("Try to write to " + CharacteristicUuidsResolver.GetNameFromUuid(characteristic.Uuid));

            try
            {
                var writer = new DataWriter();
                writer.WriteBytes(commandToWrite);
                await characteristic.WriteValueAsync(writer.DetachBuffer());
            }
            catch
            {
            }

        }

        public async void SendCommandToAllChar(IReadOnlyList<GattCharacteristic> characteristics, byte[] commandToWrite)
        {
            foreach (var characteristic in characteristics)
            {
                Debug.WriteLine("Try to write to " + CharacteristicUuidsResolver.GetNameFromUuid(characteristic.Uuid));

                try
                {
                    var writer = new DataWriter();
                    writer.WriteBytes(commandToWrite);
                    await characteristic.WriteValueAsync(writer.DetachBuffer());
                    Debug.WriteLine("Write sucessfull");
                }
                catch
                {
                    Debug.WriteLine("Write error");
                }
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
