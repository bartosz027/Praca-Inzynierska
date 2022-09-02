using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.ManageInvitations.AcceptFriendInvitation {

    [Serializable]
    public class AcceptFriendInvitationRequest : Request {
        public int UserID { get; set; }
    }

}