using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.SendMessage {

    [Serializable]
    public class SendMessageRequest : Request {
        public int ReceiverID { get; set; }
        public string Content { get; set; }
        public DateTime SendDate { get; set; }
    }

}