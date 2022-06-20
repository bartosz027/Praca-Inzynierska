using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.AddFriend {

    [Serializable]
    public class AddFriendRequest : Request {
        public string Username { get; set; }
    }

}