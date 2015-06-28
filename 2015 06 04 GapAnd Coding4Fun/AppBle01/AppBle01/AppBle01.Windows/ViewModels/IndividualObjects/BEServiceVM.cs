using System;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.UI.Xaml;

namespace AppBle01.ViewModels.IndividualObjects
{
    /// <summary>
    /// Glue between the serivce view and model.
    /// </summary>
    public class BEServiceVM : BEGattVMBase<GattDeviceService>
    {

        #region ---------------------- Properties -------------------
        public BEServiceModel ServiceM { get; private set; }

        public string Name
        {
            get
            {
                return ServiceM.Name;
            }
        }

        public Guid Uuid
        {
            get
            {
                return ServiceM.Uuid;
            }
        }

        public string ParentString
        {
            get
            {
                return "device: " + ServiceM.DeviceM.Name + " - ";
            }
        }

        public Visibility NameChangable
        {
            get 
            {
                if (!ServiceM.Default)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed; 
                }
            }
        }

        public Visibility ToastableVisibility
        {
            get
            {
                if (ServiceM.Toastable)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public Visibility WritableVisibility
        {
            get
            {
                if (ServiceM.Writable)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public Visibility IsUnknownVisibility
        {
            get
            {
                if (!ServiceM.Default)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }
        #endregion // properties

        public override string ToString()
        {
            return Name;
        }

        public void Initialize(BEServiceModel serviceM)
        {
            if (serviceM == null)
            {
                throw new ArgumentNullException("In BEServiceVM, GattDeviceService cannot be null");
            }
            Model = serviceM;
            ServiceM = serviceM;
            ServiceM.Register(this);
        }
    }

}