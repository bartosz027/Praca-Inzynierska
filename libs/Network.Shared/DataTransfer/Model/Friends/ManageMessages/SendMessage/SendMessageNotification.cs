using System;
using System.Collections.Generic;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.ManageMessages.SendMessage {

    [Serializable]
    public class SendMessageNotification : Notification {
        public int FriendID { get; set; }
        public int MessageID { get; set; }

        public string Content { get; set; }
        public DateTime SendDate { get; set; }

        public List<string> Images { get; set; }
    }

}