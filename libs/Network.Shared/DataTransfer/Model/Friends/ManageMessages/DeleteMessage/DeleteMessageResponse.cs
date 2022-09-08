using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.ManageMessages.DeleteMessage {

    [Serializable]
    public class DeleteMessageResponse : Response {
        public int FriendID { get; set; }
        public int MessageID { get; set; }
    }

}