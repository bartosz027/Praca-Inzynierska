using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Test.Server {

    [Serializable]
    public class ServerTestSaveLogsRequest : Request {
        public string Filepath { get; set; }    // Specifies log file location
    }

}