using System;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace WuaBleRs02.Drone
{
    public class CharacteristicUuidsResolver
    {
        public static string GetNameFromUuid(Guid charsUuid)
        {
            var result = "undefined";

            if (charsUuid == RollingSpiderCharacteristicUuids.Parrot_AIF) result = "Parrot_AIF";
            if (charsUuid == RollingSpiderCharacteristicUuids.Parrot_B01) result = "Parrot_B01";
            if (charsUuid == RollingSpiderCharacteristicUuids.Parrot_B0E_BC_BD) result = "Parrot_B0E_BC_BD";
            if (charsUuid == RollingSpiderCharacteristicUuids.Parrot_B1B_E3_E4) result = "Parrot_B1B_E3_E4";
            if (charsUuid == RollingSpiderCharacteristicUuids.Parrot_B1C_E6_E7) result = "Parrot_B1C_E6_E7";
            if (charsUuid == RollingSpiderCharacteristicUuids.Parrot_B1F) result = "Parrot_B1F";
            if (charsUuid == RollingSpiderCharacteristicUuids.Parrot_Battery_B0F_BF_C0) result = "Parrot_Battery_B0F_BF_C0";
            if (charsUuid == RollingSpiderCharacteristicUuids.Parrot_D22) result = "Parrot_D22";
            if (charsUuid == RollingSpiderCharacteristicUuids.Parrot_D23) result = "Parrot_D23";
            if (charsUuid == RollingSpiderCharacteristicUuids.Parrot_D24) result = "Parrot_D24";
            if (charsUuid == RollingSpiderCharacteristicUuids.Parrot_D52) result = "Parrot_D52";
            if (charsUuid == RollingSpiderCharacteristicUuids.Parrot_D53) result = "Parrot_D53";
            if (charsUuid == RollingSpiderCharacteristicUuids.Parrot_D54) result = "Parrot_D54";
            if (charsUuid == RollingSpiderCharacteristicUuids.Parrot_DateTime) result = "Parrot_DateTime";
            if (charsUuid == RollingSpiderCharacteristicUuids.Parrot_EmergencyStop) result = "Parrot_EmergencyStop";
            if (charsUuid == RollingSpiderCharacteristicUuids.Parrot_FC1) result = "Parrot_FC1";
            if (charsUuid == RollingSpiderCharacteristicUuids.Parrot_InitCount1_20) result = "Parrot_InitCount1_20";
            if (charsUuid == RollingSpiderCharacteristicUuids.Parrot_PowerMotors) result = "Parrot_PowerMotors";
            if (charsUuid == RollingSpiderCharacteristicUuids.Parrot_Stop) result = "Parrot_Stop";
            if (charsUuid == RollingSpiderCharacteristicUuids.Parrot_TourTheStairsParrotA01) result = "Parrot_TourTheStairsParrotA01";



            if (charsUuid == GattCharacteristicUuids.AlertCategoryId) result = "AlertCategoryId";
            if (charsUuid == GattCharacteristicUuids.AlertCategoryIdBitMask) result = "AlertCategoryIdBitMask";
            if (charsUuid == GattCharacteristicUuids.AlertLevel) result = "";
            if (charsUuid == GattCharacteristicUuids.AlertNotificationControlPoint) result = "AlertNotificationControlPoint ";
            if (charsUuid == GattCharacteristicUuids.AlertStatus) result = "AlertStatus ";
            if (charsUuid == GattCharacteristicUuids.BatteryLevel) result = "BatteryLevel";
            if (charsUuid == GattCharacteristicUuids.BloodPressureFeature) result = "BloodPressureFeature";
            if (charsUuid == GattCharacteristicUuids.BloodPressureMeasurement) result = "BloodPressureMeasurement";
            if (charsUuid == GattCharacteristicUuids.BodySensorLocation) result = "BodySensorLocation";
            if (charsUuid == GattCharacteristicUuids.BootKeyboardInputReport) result = "BootKeyboardInputReport";
            if (charsUuid == GattCharacteristicUuids.BootKeyboardOutputReport) result = "BootKeyboardOutputReport";
            if (charsUuid == GattCharacteristicUuids.BootMouseInputReport) result = "BootMouseInputReport";
            if (charsUuid == GattCharacteristicUuids.CscFeature) result = "CscFeature";
            if (charsUuid == GattCharacteristicUuids.CscMeasurement) result = "CscMeasurement";
            if (charsUuid == GattCharacteristicUuids.CurrentTime) result = "CurrentTime";
            if (charsUuid == GattCharacteristicUuids.CyclingPowerControlPoint) result = "CyclingPowerControlPoint";
            if (charsUuid == GattCharacteristicUuids.CyclingPowerFeature) result = "CyclingPowerFeature";
            if (charsUuid == GattCharacteristicUuids.CyclingPowerMeasurement) result = "CyclingPowerMeasurement";
            if (charsUuid == GattCharacteristicUuids.CyclingPowerVector) result = "CyclingPowerVector";
            if (charsUuid == GattCharacteristicUuids.DateTime) result = "DateTime";
            if (charsUuid == GattCharacteristicUuids.DayDateTime) result = "DayDateTime";
            if (charsUuid == GattCharacteristicUuids.DayOfWeek) result = "DayOfWeek";
            if (charsUuid == GattCharacteristicUuids.DstOffset) result = "DstOffset";
            if (charsUuid == GattCharacteristicUuids.ExactTime256) result = "ExactTime256";
            if (charsUuid == GattCharacteristicUuids.FirmwareRevisionString) result = "FirmwareRevisionString";
            if (charsUuid == GattCharacteristicUuids.GapAppearance) result = "GapAppearance";
            if (charsUuid == GattCharacteristicUuids.GapDeviceName) result = "GapDeviceName";
            if (charsUuid == GattCharacteristicUuids.GapPeripheralPreferredConnectionParameters) result = "GapPeripheralPreferredConnectionParameters";
            if (charsUuid == GattCharacteristicUuids.GapPeripheralPrivacyFlag) result = "GapPeripheralPrivacyFlag";
            if (charsUuid == GattCharacteristicUuids.GapReconnectionAddress) result = "GapReconnectionAddress";
            if (charsUuid == GattCharacteristicUuids.GattServiceChanged) result = "GattServiceChanged";
            if (charsUuid == GattCharacteristicUuids.GlucoseFeature) result = "GlucoseFeature";
            if (charsUuid == GattCharacteristicUuids.GlucoseMeasurement) result = "GlucoseMeasurement";
            if (charsUuid == GattCharacteristicUuids.GlucoseMeasurementContext) result = "GlucoseMeasurementContext";
            if (charsUuid == GattCharacteristicUuids.HardwareRevisionString) result = "HardwareRevisionString";
            if (charsUuid == GattCharacteristicUuids.HeartRateControlPoint) result = "HeartRateControlPoint";
            if (charsUuid == GattCharacteristicUuids.HeartRateMeasurement) result = "HeartRateMeasurement";
            if (charsUuid == GattCharacteristicUuids.HidControlPoint) result = "HidControlPoint";
            if (charsUuid == GattCharacteristicUuids.HidInformation) result = "HidInformation";
            if (charsUuid == GattCharacteristicUuids.Ieee1107320601RegulatoryCertificationDataList) result = "Ieee1107320601RegulatoryCertificationDataList";
            if (charsUuid == GattCharacteristicUuids.IntermediateCuffPressure) result = "IntermediateCuffPressure";
            if (charsUuid == GattCharacteristicUuids.IntermediateTemperature) result = "IntermediateTemperature";
            if (charsUuid == GattCharacteristicUuids.LnControlPoint) result = "LnControlPoint";
            if (charsUuid == GattCharacteristicUuids.LnFeature) result = "LnFeature";
            if (charsUuid == GattCharacteristicUuids.LocalTimeInformation) result = "LocalTimeInformation";
            if (charsUuid == GattCharacteristicUuids.LocationAndSpeed) result = "LocationAndSpeed";
            if (charsUuid == GattCharacteristicUuids.ManufacturerNameString) result = "ManufacturerNameString";
            if (charsUuid == GattCharacteristicUuids.MeasurementInterval) result = "MeasurementInterval";
            if (charsUuid == GattCharacteristicUuids.ModelNumberString) result = "ModelNumberString";
            if (charsUuid == GattCharacteristicUuids.Navigation) result = "Navigation";
            if (charsUuid == GattCharacteristicUuids.NewAlert) result = "NewAlert";
            if (charsUuid == GattCharacteristicUuids.PnpId) result = "PnpId";
            if (charsUuid == GattCharacteristicUuids.PositionQuality) result = "PositionQuality";
            if (charsUuid == GattCharacteristicUuids.ProtocolMode) result = "ProtocolMode";
            if (charsUuid == GattCharacteristicUuids.RecordAccessControlPoint) result = "RecordAccessControlPoint";
            if (charsUuid == GattCharacteristicUuids.ReferenceTimeInformation) result = "ReferenceTimeInformation";
            if (charsUuid == GattCharacteristicUuids.Report) result = "Report";
            if (charsUuid == GattCharacteristicUuids.ReportMap) result = "ReportMap";
            if (charsUuid == GattCharacteristicUuids.RingerControlPoint) result = "RingerControlPoint";
            if (charsUuid == GattCharacteristicUuids.RingerSetting) result = "RingerSetting";
            if (charsUuid == GattCharacteristicUuids.RscFeature) result = "RscFeature";
            if (charsUuid == GattCharacteristicUuids.RscMeasurement) result = "RscMeasurement";
            if (charsUuid == GattCharacteristicUuids.SCControlPoint) result = "SCControlPoint";
            if (charsUuid == GattCharacteristicUuids.ScanIntervalWindow) result = "ScanIntervalWindow";
            if (charsUuid == GattCharacteristicUuids.ScanRefresh) result = "ScanRefresh";
            if (charsUuid == GattCharacteristicUuids.SensorLocation) result = "SensorLocation";
            if (charsUuid == GattCharacteristicUuids.SerialNumberString) result = "SerialNumberString";
            if (charsUuid == GattCharacteristicUuids.SoftwareRevisionString) result = "SoftwareRevisionString";
            if (charsUuid == GattCharacteristicUuids.SupportUnreadAlertCategory) result = "SupportUnreadAlertCategory";
            if (charsUuid == GattCharacteristicUuids.SupportedNewAlertCategory) result = "SupportedNewAlertCategory";
            if (charsUuid == GattCharacteristicUuids.SystemId) result = "SystemId";
            if (charsUuid == GattCharacteristicUuids.TemperatureMeasurement) result = "TemperatureMeasurement";
            if (charsUuid == GattCharacteristicUuids.TemperatureType) result = "TemperatureType";
            if (charsUuid == GattCharacteristicUuids.TimeAccuracy) result = "TimeAccuracy";
            if (charsUuid == GattCharacteristicUuids.TimeSource) result = "TimeSource";
            if (charsUuid == GattCharacteristicUuids.TimeUpdateControlPoint) result = "TimeUpdateControlPoint";
            if (charsUuid == GattCharacteristicUuids.TimeUpdateState) result = "TimeUpdateState";
            if (charsUuid == GattCharacteristicUuids.TimeWithDst) result = "TimeWithDst";
            if (charsUuid == GattCharacteristicUuids.TimeZone) result = "TimeZone";
            if (charsUuid == GattCharacteristicUuids.TxPowerLevel) result = "TxPowerLevel";
            if (charsUuid == GattCharacteristicUuids.UnreadAlertStatus) result = "UnreadAlertStatus";

            return result;
        }
    }

}
