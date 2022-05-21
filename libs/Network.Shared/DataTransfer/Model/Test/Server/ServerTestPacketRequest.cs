using System;
using System.Collections.Generic;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Test.Server {

    [Serializable]
    public class ServerTestPacketRequest : Request {
        public int ID { get; set; }

        // For logging
        public string Filepath { get; set; }

        // For random packet size
        public List<long> UselessData { get; set; }
    }

}