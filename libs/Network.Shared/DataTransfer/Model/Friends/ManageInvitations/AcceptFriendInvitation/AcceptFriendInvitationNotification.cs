using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.ManageInvitations.AcceptFriendInvitation {

    [Serializable]
    public class AcceptFriendInvitationNotification : Notification {
        public int UserID { get; set; }
        public bool Status { get; set; }
        public string Username { get; set; }
    }

}