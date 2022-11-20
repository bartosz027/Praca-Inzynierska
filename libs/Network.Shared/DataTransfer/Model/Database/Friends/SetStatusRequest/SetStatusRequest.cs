using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Database.Friends.SetStatusRequest {

    [Serializable]
    public class SetStatusRequest : Request {
        public int ID { get; set; }
        public bool Status { get; set; }
    }

}