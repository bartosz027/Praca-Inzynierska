using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.ManageInvitations.SendFriendInvitation {

    [Serializable]
    public class SendFriendInvitationResponse : Response {
        public int UserID { get; set; }
        public string Username { get; set; }
    }

}