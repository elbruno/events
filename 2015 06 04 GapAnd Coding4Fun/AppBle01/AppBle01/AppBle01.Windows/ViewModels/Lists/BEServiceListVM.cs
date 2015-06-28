using System.Collections.Generic;
using System.Collections.ObjectModel;
using AppBle01.ViewModels.IndividualObjects;

namespace AppBle01.ViewModels.Lists
{
    /// <summary>
    /// A list containing Bluetooth Services with wrappers to the XAML UI.
    /// </summary>
    public class BEServiceListVM : ObservableCollection<BEServiceVM>
    {
        public void Initialize(ICollection<BEServiceModel> serviceModels)
        {
            this.Clear();
            foreach (BEServiceModel serviceM in serviceModels)
            {
                BEServiceVM serviceVM = new BEServiceVM();
                serviceVM.Initialize(serviceM);
                this.Add(serviceVM);
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