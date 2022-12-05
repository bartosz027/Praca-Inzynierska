using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.ManageInvitations.DeclineFriendInvitation {

    [Serializable]
    public class DeclineFriendInvitationRequest : Request {
        public int UserID { get; set; }
    }

}