using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace WuaBleRollingSpider.Model
{
    public class RsCharacteristic
    {
        public string DeviceId { get; set; }
        public string CharName { get; set; }
        public GattCharacteristic Characteristic { get; set; }
    }
}
