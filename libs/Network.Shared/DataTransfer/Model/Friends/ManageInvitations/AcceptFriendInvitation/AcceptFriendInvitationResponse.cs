using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.ManageInvitations.AcceptFriendInvitation {

    [Serializable]
    public class AcceptFriendInvitationResponse : Response {
        public int UserID { get; set; }
        public string Username { get; set; }
    }

}