using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Network.Shared.Core {

    internal static class Serializer {
        public static byte[] Serialize(object data) {
            var binaryFormatter = new BinaryFormatter();
            var memoryStream = new MemoryStream(); 

            binaryFormatter.Serialize(memoryStream, data);
            return memoryStream.ToArray();
        }

        public static object Deserialize(byte[] data) {
            var binaryFormatter = new BinaryFormatter();
            return binaryFormatter.Deserialize(new MemoryStream(data));
        }

        // Adds length of serialized object
        public static byte[] AddArrayLength(byte[] data) {
            var arr = new byte[data.Length + sizeof(int)];
            var length = BitConverter.GetBytes(data.Length);

            Array.Copy(length, 0, arr, 0, sizeof(int));
            Array.Copy(data, 0, arr, sizeof(int), data.Length);

            return arr;
        }
    }

}