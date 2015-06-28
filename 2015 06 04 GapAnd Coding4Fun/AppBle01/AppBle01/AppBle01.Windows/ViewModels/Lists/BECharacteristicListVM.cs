using System.Collections.Generic;
using System.Collections.ObjectModel;
using AppBle01.Models;
using AppBle01.ViewModels.IndividualObjects;

namespace AppBle01.ViewModels.Lists
{
    /// <summary>
    /// A list containing Bluetooth Characteristics with wrappers to the XAML UI.
    /// </summary>
    public class BECharacteristicListVM : ObservableCollection<BECharacteristicVM>
    {
        public void Initialize(ICollection<BeCharacteristicModel> characteristicModels)
        {
            Clear();
            foreach (var characteristicM in characteristicModels)
            {
                var characteristicVm = new BECharacteristicVM();
                characteristicVm.Initialize(characteristicM);
                Add(characteristicVm);
            }
        }

        /// <summary>
        /// Unregister all notifiable characteristics in this list
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