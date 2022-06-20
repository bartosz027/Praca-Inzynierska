using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.AcceptFriendInvitation {

    [Serializable]
    public class AcceptFriendInvitationRequest : Request {
        public string Username { get; set; }
    }

}