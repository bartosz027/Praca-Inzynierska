using System;
using System.Collections.Generic;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Database.Friends.GetInvitations {

    [Serializable]
    public class FriendInvitationInfo {
        public int UserID { get; set; }
        public string Username { get; set; }
    }

    [Serializable]
    public class GetInvitationsResponse : Response {
        public List<FriendInvitationInfo> PendingInvitations { get; set; }
        public List<FriendInvitationInfo> ReceivedInvitations { get; set; }
    }

}