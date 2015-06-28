using System;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using AppBle01.Extras;

namespace AppBle01.Dictionary
{
    // A dictionary which holds additional information about Characteristics in the system.
    public class CharacteristicDictionary : DictionaryBase<CharacteristicDictionaryEntry>
    {
        private const string CHARACTERISTIC_DICTIONARY_WRITTEN_FILE_NAME = "CharacteristicDictionary";
        private void AddAndCreateNewEntry(Guid uuid, String name, bool isDefault = false)
        {
            // Error handle duplicates
            if (ContainsKey(uuid))
            {
                Utilities.OnException(new ArgumentException("In ServiceDictionary.AddAndCreateNewEntry: Attempted to add Uuid which exists."));
                return; 
            } 
            
            // add in new entry
            var entry = new CharacteristicDictionaryEntry(); 
            entry.Initialize(uuid, name, isDefault);
            Add(uuid, entry); 
        }

        // Initializes a dictionary as the constant (ie, unchangable; only contains final, default value)
        // dictionary. 
        public void InitAsConstant()
        {
            // Automatically add all UIUDs defined in GattCharacteristicUuids to the system. 
            // Uses System.Reflection to get the name. Assumes that corresponding characteristic data
            // class exists in the CharacteristicList namespace. 
            // (Ex. HeartRateMeasurementData exists for prop.Name of HeartRateMeasurement.)

            // MSDN defined characteristics of the Bluetooth specification
            var properties = typeof(GattCharacteristicUuids).GetRuntimeProperties();
            foreach (var prop in properties)
            {
                AddAndCreateNewEntry((Guid)prop.GetValue(null), prop.Name, true);
            }

            // TI Sensor tag characteristics
            properties = typeof(TI_BLESensorTagGattUuids.TISensorTagCharacteristicUUIDs).GetRuntimeProperties();
            foreach (var prop in properties)
            {
                AddAndCreateNewEntry((Guid)prop.GetValue(null), prop.Name, true);
            }
        }


        public override async Task LoadDictionaryAsync()
        {
            await ReadFileAndDeserializeIfExistsAsync(CHARACTERISTIC_DICTIONARY_WRITTEN_FILE_NAME);
        }

        public override async Task SaveDictionaryAsync()
        {
            await SerializeAndWriteFileAsync(CHARACTERISTIC_DICTIONARY_WRITTEN_FILE_NAME); 
        }

        public override void AddLoadedEntry(CharacteristicDictionaryEntry input)
        {
            Add(input.Uuid, input); 
        }
    }
}
