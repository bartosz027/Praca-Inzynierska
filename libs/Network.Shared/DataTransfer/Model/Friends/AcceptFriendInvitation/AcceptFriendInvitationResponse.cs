using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.AcceptFriendInvitation {

    [Serializable]
    public class AcceptFriendInvitationResponse : Response {
        public int UserID { get; set; }
        public string Username { get; set; }
    }

}