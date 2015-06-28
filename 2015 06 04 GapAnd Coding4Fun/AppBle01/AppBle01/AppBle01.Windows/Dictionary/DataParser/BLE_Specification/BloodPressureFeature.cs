using Windows.Storage.Streams;

namespace AppBle01.Dictionary.DataParser.BLE_Specification
{
    public static class BloodPressureFeature
    {
        // From
        // https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.blood_pressure_feature.xml
        // NOTE: This characteristic not tested. 
        public enum Status
        {
            BodyMovementDetectionSupported = 1 << 0,
            CuffFitDetectionSupported = 1 << 1,
            IrregularPulseDetectionSupported = 1 << 2,
            PulseRateRangeDetectionSupported = 1 << 3,
            MeasurementPositionDetectionSupported = 1 << 4,
            MultipleBondsSupported = 1 << 5,
        }

        public static string ParseBuffer(IBuffer buffer)
        {
            DataReader reader = DataReader.FromBuffer(buffer);
            byte result = reader.ReadByte();
            return BasicParsers.FlagsSetInByte(typeof(Status), result);
        }
    }
}
