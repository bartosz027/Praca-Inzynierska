using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Test.Server {

    [Serializable]
    public class ServerTestSaveLogsRequest : Request {
        // Specifies log file location
        public string Filepath { get; set; }
    }

}