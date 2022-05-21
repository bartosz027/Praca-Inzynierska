using System;
using System.Collections.Generic;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Test.Client {

    [Serializable]
    public class ClientTestResponse : Response {
        public int ID { get; set; }
        public bool IsLastResponse { get; set; }

        // For random packet size
        public List<long> UselessData { get; set; }
    }

}