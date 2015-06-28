using System;
using System.Collections.Generic;
using AppBle01.ViewModels.IndividualObjects;

namespace AppBle01.Models
{
    /// <summary>
    /// A base class containing functions common to all Gatt-object wrappers acting as models.
    /// 
    /// For a child model class of BEGattModelBase, BEGattVMBase<BEGattModelBase<GattObjectType>> 
    /// is the view-model class associated with the model.
    /// 
    /// An example:
    /// BEDeviceModel inherits from BEGattModelBase<BluetoothGattDevice>
    /// BEDeviceVM would be in the ViewModelInstances list. 
    /// </summary>
    /// <typeparam name="TGattObjectType"></typeparam>
    public class BeGattModelBase<TGattObjectType>
    {
        #region --------------------- Manipulate view-models dependent on this model ---------------------
        protected List<BeGattVmBase<TGattObjectType>> ViewModelInstances;

        /// <summary>
        /// Registers the view model with the model, so that the model can fire change notifications
        /// </summary>
        /// <param name="vm"></param>
        public void Register(BeGattVmBase<TGattObjectType> vm)
        {
            lock (ViewModelInstances)
            {
                if (vm == null)
                {
                    throw new ArgumentNullException("Tried to register a null-valued view-model to a model.");
                }
                ViewModelInstances.Add(vm);
            }
        }

        /// <summary>
        /// Unregisters the view model from the model
        /// </summary>
        /// <param name="vm"></param>
        public void UnregisterVmFromModel(BeGattVmBase<TGattObjectType> vm)
        {
            lock (ViewModelInstances)
            {
                if (vm == null)
                {
                    throw new ArgumentNullException("Tried to remove a null-valued view-model from a model");
                }
                if (ViewModelInstances.Contains(vm))
                {
                    ViewModelInstances.Remove(vm);
                }
            }
        }
        
        /// <summary>
        /// Iterates through all view models tracked by this model and fires the
        /// PropertyChanged event handler on them.
        /// </summary>
        /// <param name="property"></param>
        protected void SignalChanged(string property)
        {
            BeGattVmBase<TGattObjectType>[] viewModels;
            lock (ViewModelInstances)
            {
                viewModels = ViewModelInstances.ToArray();
            }

            // Call each VM outside the lock, as they can signal a change to the UI, resulting
            // in a deadlock if the UI thread callback is trying to Register or Unregister a
            // VM above
            foreach (var vm in viewModels)
            {
                vm.SignalChanged(property);
            }
        }
        #endregion
    }
}
