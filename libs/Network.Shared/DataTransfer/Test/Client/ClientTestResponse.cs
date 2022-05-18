using System;
using System.Collections.Generic;
using Network.Shared.DataTransfer.Interface;

namespace Network.Shared.DataTransfer.Test.Client {

    [Serializable]
    public class ClientTestResponse : IResponse {
        public int ID { get; set; }
        public bool LastResponse { get; set; }

        // For random packet size
        public List<long> UselessData { get; set; }
    }

}