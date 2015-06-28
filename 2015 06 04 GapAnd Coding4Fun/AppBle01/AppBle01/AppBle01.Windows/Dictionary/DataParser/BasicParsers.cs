using System;
using Windows.Storage.Streams;

namespace AppBle01.Dictionary.DataParser
{
    // A class which includes methods for parsing a buffer in a few generic ways.
    // (String, uint8, etc)
    public static class BasicParsers
    {
        #region --------------- Parse as some type of integer ---------------
        #region --------------- Parse as UInt8
        // Single
        public static string ParseUInt8(IBuffer buffer)
        {
            var reader = DataReader.FromBuffer(buffer);
            var result = reader.ReadByte();
            return Convert.ToString(result);
        }

        // Multiple
        public static string ParseUInt8Multi(IBuffer buffer)
        {
            var bytes = ReadBufferToBytes(buffer);
            var result = "";
            foreach (var b in bytes) {
                result += Convert.ToString(b); 
            }
            return result; 
        }
        #endregion // Parse as UInt8

        #region --------------- Parse as UInt16
        public static string ParseUInt16Multi(IBuffer buffer)
        {
            var bytes = ReadBufferToBytes(buffer);
            var hex = BitConverter.ToString(bytes);
            return hex.Replace("-", "");
        }
        #endregion

        #endregion // Parse as some type of integer

        #region --------------- Parse as string ---------------
        public static string ParseString(IBuffer buffer)
        {
            var reader = DataReader.FromBuffer(buffer);
            return reader.ReadString(buffer.Length);
        }
        #endregion // Parse as string

        #region --------------- Helper Functions ---------------

        // Gets a byte array from a buffer
        public static byte[] ReadBufferToBytes(IBuffer buffer)
        {
            var dataLength = buffer.Length;
            var data = new byte[dataLength];
            var reader = DataReader.FromBuffer(buffer);
            reader.ReadBytes(data);
            return data;
        }

        // Given an enum that represents a bit field and a byte, prints out a 
        // string which includes the name of each enum value and whether or not
        // that bit is set.
        // This function assumes that the first set of values are contiguous. 
        public static string FlagsSetInByte(Type type, byte b)
        {
            var result = ""; 
            
            foreach (var categoryName in Enum.GetNames(type))
            {
                var categoryValue = (byte)Enum.Parse(type, categoryName);
                var present = ((b & categoryValue) > 0);
                result += String.Format("\n{0} [{1}]", categoryName, present.ToString());
            }
            return result; 
        }

        #endregion // Helper Functions
    }
}
