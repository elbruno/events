using System.Collections.Generic;
using System.Collections.ObjectModel;
using AppBle01.Models;
using AppBle01.ViewModels.IndividualObjects;

namespace AppBle01.ViewModels.Lists
{
    /// <summary>
    /// A list containing Bluetooth Devices with wrappers to the XAML UI. 
    /// </summary>
    public class BEDeviceListVM : ObservableCollection<BeDeviceVm> 
    {
        public void Initialize(ICollection<BeDeviceModel> deviceModels) 
        {
            Clear();
            foreach (var deviceM in deviceModels) 
            {
                var deviceVM = new BeDeviceVm();
                deviceVM.Initialize(deviceM); 
                Add(deviceVM);
            }
        }

        /// <summary>
        /// Unregisters view-models in this from their models.
        /// </summary>
        public void Unregister()
        {
            foreach (var VMinstance in this)
            {
                VMinstance.UnregisterVmFromModel();
            }
        }
    }
}