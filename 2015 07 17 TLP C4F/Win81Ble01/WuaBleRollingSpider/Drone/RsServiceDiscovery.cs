using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using WuaBleRollingSpider.Model;

namespace WuaBleRollingSpider.Drone
{
    public class RsServiceDiscovery : ParrotBase
    {
        public const string DeviceName = "RS_R056712";

        public async Task<List<string>> GetDeviceIdsfromServiceIds()
        {
            var devicesId = new List<string>();
            
            var bts = await DeviceInformation.FindAllAsync();
            foreach (var serviceId in ParrotConstants.AppManifiestServiceIds)
            {
                foreach (var device in bts)
                {
                    try
                    {
                        if (!device.Id.ToUpper().Contains(serviceId)) continue;
                        var service = await GattDeviceService.FromIdAsync(device.Id);
                        if (null == service) continue;
                        var characteristicsFa00 = service.GetAllCharacteristics();
                        devicesId.Add(device.Id);
                    }
                    catch(Exception exception)
                    {
                        AddException(exception);
                    }
                }
            }
            return devicesId;
        }

        public async Task<Dictionary<string, RsCharacteristic>> InitServices()
        {
            var dicCharacteristics = new Dictionary<string, RsCharacteristic>();
            var devicesIds = await GetDeviceIdsfromServiceIds();
            foreach (var deviceId in devicesIds)
            {
                dicCharacteristics = await GetCharsFromDeviceId(deviceId, dicCharacteristics);
            }
            return dicCharacteristics;
        }

        public async Task InitChararacteristicsConfiguration()
        {
            ClearExceptions();
            const GattClientCharacteristicConfigurationDescriptorValue charValue =
                GattClientCharacteristicConfigurationDescriptorValue.Indicate;
            try
            {
                await InitCount1To20.WriteClientCharacteristicConfigurationDescriptorAsync(charValue);
            }
            catch (Exception exception)
            {
                 AddException(exception);
            }
            try
            {
                await PowerMotors.WriteClientCharacteristicConfigurationDescriptorAsync(charValue);
            }
            catch (Exception exception)
            {
                AddException(exception);
            }
            try
            {
                await DateTime.WriteClientCharacteristicConfigurationDescriptorAsync(charValue);
            }
            catch (Exception exception)
            {
                AddException(exception);
            }
            try
            {
                await EmergencyStop.WriteClientCharacteristicConfigurationDescriptorAsync(charValue);
            }
            catch (Exception exception)
            {
                AddException(exception);
            }
        }


        private async Task<Dictionary<string, RsCharacteristic>> GetCharsFromDeviceId(string deviceId, Dictionary<string, RsCharacteristic> dicCharacteristics)
        {
            try
            {
                var service = await GattDeviceService.FromIdAsync(deviceId);
                if (null != service)
                {
                    dicCharacteristics = CompletePropsFromServiceChars(service, deviceId, dicCharacteristics);
                }
            }
            catch
            {
                // ignored
            }
            return dicCharacteristics;
        }

        private Dictionary<string, RsCharacteristic> CompletePropsFromServiceChars(GattDeviceService service, string deviceId, Dictionary<string, RsCharacteristic> dicCharacteristics)
        {
            var characteristics = service.GetAllCharacteristics();
            foreach (var characteristic in characteristics)
            {
                var charName = CharacteristicUuidsResolver.GetNameFromUuid(characteristic.Uuid);
                Debug.WriteLine(characteristic.Uuid + " - " + charName);
                if (charName == "undefined") continue;

                var rsCharacteristic = new RsCharacteristic
                {
                    Characteristic = characteristic,
                    CharName = charName,
                    DeviceId = deviceId
                };
                dicCharacteristics.Add(charName, rsCharacteristic);

                switch (charName)
                {
                    case "undefined":
                        Undefined = characteristic;
                        break;
                    case "GapDeviceName":
                        GapDeviceName = characteristic;
                        break;
                    case "GapAppearance":
                        GapAppearance = characteristic;
                        break;
                    case "GapPeripheralPreferredConnectionParameters":
                        GapPeripheralPreferredConnectionParameters = characteristic;
                        break;
                    case "GattServiceChanged":
                        GattServiceChanged = characteristic;
                        break;
                    case "Parrot_TourTheStairsParrotA01":
                        TourTheStairsParrotA01 = characteristic;
                        break;
                    case "Parrot_Stop":
                        Stop = characteristic;
                        break;
                    case "Parrot_PowerMotors":
                        PowerMotors = characteristic;
                        break;
                    case "Parrot_DateTime":
                        DateTime = characteristic;
                        break;
                    case "Parrot_EmergencyStop":
                        EmergencyStop = characteristic;
                        break;
                    case "Parrot_InitCount1_20":
                        InitCount1To20 = characteristic;
                        break;
                    case "Parrot_AIF":
                        Aif = characteristic;
                        break;
                    case "Parrot_B01":
                        B01 = characteristic;
                        break;
                    case "Parrot_B0E_BC_BD":
                        B0E_BC_BD = characteristic;
                        break;
                    case "Parrot_Battery_B0F_BF_C0":
                        B0F_BF_C0 = characteristic;
                        break;
                    case "Parrot_B1B_E3_E4":
                        B1B_E3_E4 = characteristic;
                        break;
                    case "Parrot_B1C_E6_E7":
                        B1C_E6_E7 = characteristic;
                        break;
                    case "Parrot_B1F":
                        B1F = characteristic;
                        break;
                    case "Parrot_FC1":
                        Fc1 = characteristic;
                        break;
                    case "Parrot_D22":
                        D22 = characteristic;
                        break;
                    case "Parrot_D23":
                        D23 = characteristic;
                        break;
                    case "Parrot_D24":
                        D24 = characteristic;
                        break;
                    case "Parrot_D52":
                        D52 = characteristic;
                        break;
                    case "Parrot_D53":
                        D53 = characteristic;
                        break;
                    case "Parrot_D54":
                        Fc1 = characteristic;
                        break;
                    default:
                        Debug.WriteLine("not found for " + charName);
                        break;
                }
            }

            return dicCharacteristics;
        }

        public GattCharacteristic D53 { get; set; }

        public GattCharacteristic D52 { get; set; }

        public GattCharacteristic D24 { get; set; }

        public GattCharacteristic D23 { get; set; }

        public GattCharacteristic D22 { get; set; }

        public GattCharacteristic Fc1 { get; set; }

        public GattCharacteristic B1F { get; set; }

        public GattCharacteristic B1C_E6_E7 { get; set; }

        public GattCharacteristic B1B_E3_E4 { get; set; }

        public GattCharacteristic B0F_BF_C0 { get; set; }

        public GattCharacteristic B0E_BC_BD { get; set; }

        public GattCharacteristic B01 { get; set; }

        public GattCharacteristic Undefined { get; set; }

        public GattCharacteristic GattServiceChanged { get; set; }

        public GattCharacteristic GapAppearance { get; set; }

        public GattCharacteristic GapPeripheralPreferredConnectionParameters { get; set; }

        public GattCharacteristic GapDeviceName { get; set; }

        public GattCharacteristic Aif { get; set; }

        public GattCharacteristic InitCount1To20 { get; set; }

        public GattCharacteristic EmergencyStop { get; set; }

        public GattCharacteristic DateTime { get; set; }

        public GattCharacteristic PowerMotors { get; set; }

        public GattCharacteristic Stop { get; set; }

        public GattCharacteristic TourTheStairsParrotA01 { get; set; }
        public GattCharacteristic ParrotFc1 { get; set; }
    }
}
