using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace WuaBleRollingSpider.Drone
{
    public class ParrotLoadDevices : ParrotBase
    {
        public async Task<List<DeviceInformation>> LoadParrotDevices()
        {
            var parrotDevices = new List<DeviceInformation>();
            var bts = await DeviceInformation.FindAllAsync();
            foreach (var di in bts.Where(di => di.Name == ParrotConstants.DeviceName && ParrotConstants.DevicesId.Contains(di.Id)))
            {
                parrotDevices.Add(di);
                AddLogAction(di.Name);
            }
            return parrotDevices;
        }

        public async Task<List<DeviceInformation>> LoadAllDevices()
        {
            var allDevices = await DeviceInformation.FindAllAsync();
            var devices = new List<DeviceInformation>();
            var filteredList = new Dictionary<string, DeviceInformation>();
            foreach (var device in allDevices)
            {
                if (filteredList.ContainsKey(device.Name)) continue;
                filteredList.Add(device.Name, device);
                devices.Add(device);
                AddLogAction(device.Name);
            }
            return devices;
        }
    }
}
