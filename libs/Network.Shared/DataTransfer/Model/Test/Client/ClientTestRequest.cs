using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Test.Client {

    [Serializable]
    public class ClientTestRequest : Request {
        public int ResponseCount { get; set; }
        public int NotificationCount { get; set; }
    }

}