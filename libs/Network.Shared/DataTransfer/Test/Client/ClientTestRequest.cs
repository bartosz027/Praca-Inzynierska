using System;
using Network.Shared.DataTransfer.Interface;

namespace Network.Shared.DataTransfer.Test.Client {

    [Serializable]
    public class ClientTestRequest : IRequest {
        public int ResponseCount { get; set; }
        public int NotificationCount { get; set; }
    }

}