using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.SendMessage {

    [Serializable]
    public class SendMessageNotification : Notification {
        public int SenderID { get; set; }
        public int MessageID { get; set; }
        public string Content { get; set; }

    }

}