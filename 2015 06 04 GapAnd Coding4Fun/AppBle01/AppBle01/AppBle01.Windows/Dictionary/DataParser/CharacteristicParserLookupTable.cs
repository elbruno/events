using System;
using System.Collections.Generic;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using AppBle01.Dictionary.DataParser.BLE_Specification;
using TIUuids = AppBle01.Extras.TI_BLESensorTagGattUuids.TISensorTagCharacteristicUUIDs;
using TIParsers = AppBle01.Extras.TI_BLESensorTagCharacteristicParsers;

namespace AppBle01.Dictionary.DataParser
{
    public class CharacteristicParserLookupTable : Dictionary<Guid, Func<IBuffer, string>>
    {
        // Contains the default string parsers for the characteristics. 
        public CharacteristicParserLookupTable()
        {
            /* WinRT implemented BTLE specification characteristics */
            //Add(GattCharacteristicUuids.AlertCategoryId, AlertCategoryId.ParseBuffer);
            //Add(GattCharacteristicUuids.AlertCategoryIdBitMask, AlertCategoryIdBitMask.ParseBuffer);
            //Add(GattCharacteristicUuids.AlertLevel, AlertLevel.ParseBuffer);
            //Add(GattCharacteristicUuids.AlertNotificationControlPoint, AlertNotificationControlPoint.ParseBuffer);
            //Add(GattCharacteristicUuids.AlertStatus, AlertStatus.ParseBuffer);
            Add(GattCharacteristicUuids.BatteryLevel, BasicParsers.ParseUInt8);
            Add(GattCharacteristicUuids.BloodPressureFeature, BloodPressureFeature.ParseBuffer);
            Add(GattCharacteristicUuids.BodySensorLocation, BodySensorLocation.ParseBuffer);
            //Add(GattCharacteristicUuids.BootKeyboardInputReport, BasicParsers.ParseUInt8Multi);
            //Add(GattCharacteristicUuids.BootKeyboardOutputReport, BasicParsers.ParseUInt8Multi);
            //Add(GattCharacteristicUuids.BootMouseInputReport, BasicParsers.ParseUInt8Multi);
            Add(GattCharacteristicUuids.CscFeature, CscFeature.ParseBuffer);
            //Add(GattCharacteristicUuids.DayOfWeek, DayOfWeek.ParseBuffer);
            //Add(GattCharacteristicUuids.GapDeviceName, BasicParsers.ParseString);
            //Add(GattCharacteristicUuids.HardwareRevisionString, BasicParsers.ParseString);
            Add(GattCharacteristicUuids.HeartRateMeasurement, HeartRateMeasurement.ParseBuffer);
            //Add(GattCharacteristicUuids.ManufacturerNameString, BasicParsers.ParseString);
            //Add(GattCharacteristicUuids.ModelNumberString, BasicParsers.ParseString);
            //Add(GattCharacteristicUuids.SerialNumberString, BasicParsers.ParseString);
            //Add(GattCharacteristicUuids.SoftwareRevisionString, BasicParsers.ParseString);
            //Add(GattCharacteristicUuids.FirmwareRevisionString, BasicParsers.ParseString);


            /* TI Characteristics */
            Add(TIUuids.IRTemperature_Data, TIParsers.Parse_Temperature_Data);
            Add(TIUuids.IRTemperature_Config, TIParsers.Parse_Temperature_Configuration);
            Add(TIUuids.IRTemperature_Period, TIParsers.Parse_Temperature_Period);

            Add(TIUuids.Accelerometer_Data, TIParsers.Parse_Accelerometer_Data);
            Add(TIUuids.Accelerometer_Config, TIParsers.Parse_Accelerometer_Configuration);
            Add(TIUuids.Accelerometer_Period, TIParsers.Parse_Accelerometer_Period);

            Add(TIUuids.Humidity_Data, TIParsers.Parse_Humidity_Data);
            Add(TIUuids.Humidity_Config, TIParsers.Parse_Humidity_Configuration);
            Add(TIUuids.Humidity_Period, TIParsers.Parse_Humidity_Period);

            Add(TIUuids.Magnetometer_Data, TIParsers.Parse_Magnetometer_Data);
            Add(TIUuids.Magnetometer_Config, TIParsers.Parse_Magnetometer_Configuration);
            Add(TIUuids.Magnetometer_Period, TIParsers.Parse_Magnetometer_Period);

            Add(TIUuids.Barometer_Data, TIParsers.Parse_Barometer_Data);
            Add(TIUuids.Barometer_Config, TIParsers.Parse_Barometer_Configuration);
            Add(TIUuids.Barometer_Calibration, TIParsers.Parse_Barometer_Calibration);
            Add(TIUuids.Barometer_Period, TIParsers.Parse_Barometer_Period);

            Add(TIUuids.Gyroscope_Data, TIParsers.Parse_Gyroscope_Data);
            Add(TIUuids.Gyroscope_Config, TIParsers.Parse_Gyroscope_Configuration);
            Add(TIUuids.Gyroscope_Period, TIParsers.Parse_Gyroscope_Period);
            

            /*
             * FUTURE
             * 
             * Add parsers for the rest of the Bluetooth-SIG defined characteristics
             * 
             */
        }
    }
}
