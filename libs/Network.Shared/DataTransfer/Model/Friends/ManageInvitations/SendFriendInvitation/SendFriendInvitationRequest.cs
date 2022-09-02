using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.ManageInvitations.SendFriendInvitation {

    [Serializable]
    public class SendFriendInvitationRequest : Request {
        public int UserID { get; set; }
    }

}