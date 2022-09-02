using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.ManageMessages.SendMessage {

    [Serializable]
    public class SendMessageRequest : Request {
        public int FriendID { get; set; }
        public string Content { get; set; }
    }

}