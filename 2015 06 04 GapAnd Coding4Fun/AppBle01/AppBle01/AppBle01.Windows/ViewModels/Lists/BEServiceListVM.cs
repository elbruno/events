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
            Clear();
            foreach (BEServiceModel serviceM in serviceModels)
            {
                var serviceVM = new BEServiceVM();
                serviceVM.Initialize(serviceM);
                Add(serviceVM);
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