using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

namespace WuaBleRollingSpider.Drone
{
    public class ParrotCharacteristics : ParrotBase
    {
        public async void LoadDevicesCharacteristics(List<DeviceInformation> parrotDevices)
        {
            foreach (var device in parrotDevices)
            {
                try
                {
                    var service = await GattDeviceService.FromIdAsync(device.Id);
                    if (null == service) continue;
                    var characteristics = service.GetAllCharacteristics();
                    if (null == characteristics || characteristics.Count <= 0) return;
                    Debug.WriteLine($"{characteristics.Count} chars found for {device.Id}");
                    AddLogAction($"{characteristics.Count} chars found for {device.Id}");

                    foreach (var characteristic in characteristics)
                    {
                        try
                        {
                            var charName = CharacteristicUuidsResolver.GetNameFromUuid(characteristic.Uuid);
                            Debug.WriteLine("char name: " + charName);
                            AddLogAction($"{characteristics.Count} chars found for {device.Id}");

                            // read properties
                            Debug.WriteLine("char name: " + characteristic.CharacteristicProperties);
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                    Debug.WriteLine("0 chars found for " + device.Id);
                }
            }
        }

        public async void LoadChars(string deviceId, IEnumerable parrotDevices)
        {
            foreach (var device in parrotDevices)
            {
                try
                {
                    // get device service
                    var service = await GattDeviceService.FromIdAsync(deviceId);
                    if (null == service) continue;

                    // get service characteristics
                    var characteristics = service.GetAllCharacteristics();
                    if (null == characteristics || characteristics.Count <= 0) return;
                    Debug.WriteLine($"{characteristics.Count} chars found for {deviceId}");

                    ValidateChars(deviceId, service, GattCharacteristicUuids.AlertCategoryId);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.AlertCategoryIdBitMask);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.AlertLevel);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.AlertNotificationControlPoint);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.AlertStatus);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.BatteryLevel);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.BloodPressureFeature);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.BloodPressureMeasurement);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.BodySensorLocation);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.BootKeyboardInputReport);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.BootKeyboardOutputReport);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.BootMouseInputReport);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.CscFeature);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.CscMeasurement);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.CurrentTime);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.CyclingPowerControlPoint);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.CyclingPowerFeature);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.CyclingPowerMeasurement);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.CyclingPowerVector);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.DateTime);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.DayDateTime);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.DayOfWeek);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.DstOffset);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.ExactTime256);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.FirmwareRevisionString);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.GapAppearance);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.GapDeviceName);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.GapPeripheralPreferredConnectionParameters);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.GapPeripheralPrivacyFlag);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.GapReconnectionAddress);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.GattServiceChanged);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.GlucoseFeature);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.GlucoseMeasurement);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.GlucoseMeasurementContext);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.HardwareRevisionString);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.HeartRateControlPoint);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.HeartRateMeasurement);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.HidControlPoint);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.HidInformation);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.Ieee1107320601RegulatoryCertificationDataList);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.IntermediateCuffPressure);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.IntermediateTemperature);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.LnControlPoint);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.LnFeature);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.LocalTimeInformation);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.LocationAndSpeed);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.ManufacturerNameString);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.MeasurementInterval);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.ModelNumberString);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.Navigation);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.NewAlert);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.PnpId);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.PositionQuality);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.ProtocolMode);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.RecordAccessControlPoint);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.ReferenceTimeInformation);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.Report);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.ReportMap);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.RingerControlPoint);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.RingerSetting);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.RscFeature);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.RscMeasurement);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.SCControlPoint);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.ScanIntervalWindow);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.ScanRefresh);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.SensorLocation);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.SerialNumberString);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.SoftwareRevisionString);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.SupportUnreadAlertCategory);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.SupportedNewAlertCategory);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.SystemId);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.TemperatureMeasurement);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.TemperatureType);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.TimeAccuracy);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.TimeSource);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.TimeUpdateControlPoint);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.TimeUpdateState);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.TimeWithDst);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.TimeZone);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.TxPowerLevel);
                    ValidateChars(deviceId, service, GattCharacteristicUuids.UnreadAlertStatus);
                }
                catch
                {
                }
            }
        }

        private static void ValidateChars(string deviceId, GattDeviceService service, Guid charsUuid)
        {
            var chars = service.GetCharacteristics(charsUuid);
            if (chars == null || chars.Count == 0) return;
            var charsName = CharacteristicUuidsResolver.GetNameFromUuid(charsUuid);
            Debug.WriteLine($"    {chars.Count} found for {charsName}");
            foreach (GattCharacteristic characteristic in chars)
            {
                var charName = CharacteristicUuidsResolver.GetNameFromUuid(characteristic.Uuid);
                var friendlyDesc = characteristic.UserDescription;
                Debug.WriteLine($"        {charName} - {friendlyDesc} - {characteristic.ProtectionLevel}");
            }
        }
    }
}
