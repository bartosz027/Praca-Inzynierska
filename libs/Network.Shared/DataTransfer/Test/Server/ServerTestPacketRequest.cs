using System;
using System.Collections.Generic;
using Network.Shared.DataTransfer.Interface;

namespace Network.Shared.DataTransfer.Test.Server {

    [Serializable]
    public class ServerTestPacketRequest : IRequest {
        public int ID { get; set; }

        // For logging
        public string Filepath { get; set; }

        // For random packet size
        public List<long> UselessData { get; set; }
    }

}