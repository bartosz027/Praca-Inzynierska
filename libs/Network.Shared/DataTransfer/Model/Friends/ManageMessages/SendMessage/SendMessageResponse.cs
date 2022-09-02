using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.ManageMessages.SendMessage {

    [Serializable]
    public class SendMessageResponse : Response {
        public int FriendID { get; set; }
        public int MessageID { get; set; }

        public string Content { get; set; }
        public DateTime SendDate { get; set; }
    }

}