using System;
using System.Security.Cryptography;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Security.ExchangeAESKeys {

    [Serializable]
    internal class ExchangeAESKeysRequest : Request {
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
    }

}