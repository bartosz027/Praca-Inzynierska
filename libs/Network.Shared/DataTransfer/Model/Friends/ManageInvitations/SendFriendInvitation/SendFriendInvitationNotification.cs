using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.ManageInvitations.SendFriendInvitation {

    [Serializable]
    public class SendFriendInvitationNotification : Notification {
        public int UserID { get; set; }
        public string Username { get; set; }
    }

}