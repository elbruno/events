using System.Collections.Generic;
using System.Collections.ObjectModel;
using AppBle01.ViewModels.IndividualObjects;

namespace AppBle01.ViewModels.Lists
{
    /// <summary>
    /// A list containing Bluetooth Characteristics with wrappers to the XAML UI.
    /// </summary>
    public class BECharacteristicListVM : ObservableCollection<BECharacteristicVM>
    {
        public void Initialize(ICollection<BECharacteristicModel> characteristicModels)
        {
            this.Clear();
            foreach (BECharacteristicModel characteristicM in characteristicModels)
            {
                BECharacteristicVM characteristicVM = new BECharacteristicVM();
                characteristicVM.Initialize(characteristicM);
                this.Add(characteristicVM);
            }
        }

        /// <summary>
        /// Unregister all notifiable characteristics in this list
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