using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Network.Shared.Core {

    public static class Serializer {
        public static byte[] Serialize(object data) {
            var binaryFormatter = new BinaryFormatter();
            var memoryStream = new MemoryStream(); 

            binaryFormatter.Serialize(memoryStream, data);
            var length = BitConverter.GetBytes((int)memoryStream.Length);

            return length.Concat(memoryStream.ToArray()).ToArray();
        }

        public static object Deserialize(byte[] data) {
            var binaryFormatter = new BinaryFormatter();
            return binaryFormatter.Deserialize(new MemoryStream(data));
        }
    }

}