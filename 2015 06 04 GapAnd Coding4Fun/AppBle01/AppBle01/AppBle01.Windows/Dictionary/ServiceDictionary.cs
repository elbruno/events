using System;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using AppBle01.Extras;

namespace AppBle01.Dictionary
{
    public class ServiceDictionary : DictionaryBase<ServiceDictionaryEntry>
    {
        private const string SERVICE_DICTIONARY_WRITTEN_FILE_NAME = "ServiceDictionary";

        private void AddAndCreateNewEntry(Guid uuid, String name, bool isDefault = false)
        {
            // Error handle duplicates
            if (ContainsKey(uuid))
            {
                Utilities.OnException(new ArgumentException("In ServiceDictionary.AddAndCreateNewEntry: Attempted to add Uuid which exists."));
                return; 
            }

            // Add in new value. 
            var entry = new ServiceDictionaryEntry();
            entry.Initialize(uuid, name, isDefault);
            Add(uuid, entry);
        }

        // Initialize the dictionary with values for which names will not be changing. 
        public void InitAsConstant()
        {
            // Automatically add all UIUDs defined in GattServiceUuids to the system. (Uses System.Reflection)

            // MSDN defined services of the Bluetooth specification
            var properties = typeof(GattServiceUuids).GetRuntimeProperties();
            foreach (var prop in properties)
            {
                AddAndCreateNewEntry((Guid)prop.GetValue(null), prop.Name, true);
            }

            // TI Sensor tag services
            properties = typeof(TI_BLESensorTagGattUuids.TISensorTagServiceUUIDs).GetRuntimeProperties();
            foreach (var prop in properties)
            {
                AddAndCreateNewEntry((Guid)prop.GetValue(null), prop.Name, true);
            }
        }

        public override async Task LoadDictionaryAsync()
        {
            await ReadFileAndDeserializeIfExistsAsync(SERVICE_DICTIONARY_WRITTEN_FILE_NAME); 
        }

        public override async Task SaveDictionaryAsync()
        {
            await SerializeAndWriteFileAsync(SERVICE_DICTIONARY_WRITTEN_FILE_NAME); 
        }

        public override void AddLoadedEntry(ServiceDictionaryEntry input)
        {
            Add(input.Uuid, input);
        }
    }
}
