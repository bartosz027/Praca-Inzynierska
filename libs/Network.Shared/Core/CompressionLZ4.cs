using K4os.Compression.LZ4;

namespace Network.Shared.Core {

    public static class CompressionLZ4 {
        public static byte[] Encode(byte[] data) {
            return LZ4Pickler.Pickle(data, LZ4Level.L00_FAST);
        }

        public static byte[] Decode(byte[] data) {
            return LZ4Pickler.Unpickle(data);
        }
    }

}