using System;
using System.Security.Cryptography;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Security.ExchangeRSAKeys {

    [Serializable]
    internal class ExchangeRSAKeysRequest : Request {
        public RSAParameters Key { get; set; }
    }

}