using System;
using Windows.Storage.Streams;

namespace AppBle01.Dictionary.DataParser.BLE_Specification
{
    public static class BodySensorLocation
    {
        // From
        // https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.body_sensor_location.xml
        public enum Status
        {
            Other = 0,
            Chest = 1,
            Wrist = 2,
            Finger = 3,
            Hand = 4,
            Ear_Lobe = 5,
            Foot = 6
        }

        public static string ParseBuffer(IBuffer buffer)
        {
            DataReader reader = DataReader.FromBuffer(buffer);
            byte result = reader.ReadByte();
            string categoryName;
            if (result < 7)
            {
                categoryName = ((Status)result).ToString();
            }
            else
            {
                categoryName = "Reserved for future use";
            }
            return String.Format("{0} ({1})", result, categoryName);
        }
    }
}
