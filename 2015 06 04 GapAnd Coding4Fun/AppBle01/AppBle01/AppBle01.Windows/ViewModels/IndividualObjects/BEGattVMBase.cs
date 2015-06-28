using System.ComponentModel;
using AppBle01.Models;

namespace AppBle01.ViewModels.IndividualObjects
{
    // A class that implements INotifyPropertyChanged so that XAML elements can be updated. 
    public abstract class BeGattVmBase<TGattObjectType> : INotifyPropertyChanged
    {
        public BeGattModelBase<TGattObjectType> Model { get; protected set; }

        public void UnregisterVmFromModel()
        {
            Model.UnregisterVmFromModel(this);
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public void SignalChanged(string name)
        {
            if (PropertyChanged != null)
            {
                // Make sure that the property change runs in the UI thread
                // If this is called from an MTA, you will get an RPC_E_WRONG_THREAD exception
                Utilities.RunActionOnUiThread(
                    () =>
                    {
                        var handler = PropertyChanged;
                        if (handler != null)
                        {
                            handler(this, new PropertyChangedEventArgs(name));
                        }
                    });
            }
        }
    }
}
