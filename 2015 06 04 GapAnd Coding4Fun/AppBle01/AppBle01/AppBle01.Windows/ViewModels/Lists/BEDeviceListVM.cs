using System.Collections.Generic;
using System.Collections.ObjectModel;
using AppBle01.ViewModels.IndividualObjects;

namespace AppBle01.ViewModels.Lists
{
    /// <summary>
    /// A list containing Bluetooth Devices with wrappers to the XAML UI. 
    /// </summary>
    public class BEDeviceListVM : ObservableCollection<BEDeviceVM> 
    {
        public void Initialize(ICollection<BEDeviceModel> deviceModels) 
        {
            this.Clear();
            foreach (BEDeviceModel deviceM in deviceModels) 
            {
                BEDeviceVM deviceVM = new BEDeviceVM();
                deviceVM.Initialize(deviceM); 
                this.Add(deviceVM);
            }
        }

        /// <summary>
        /// Unregisters view-models in this from their models.
        /// </summary>
        public void Unregister()
        {
            foreach (var VMinstance in this)
            {
                VMinstance.UnregisterVMFromModel();
            }
        }
    }
}