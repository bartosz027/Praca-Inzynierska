using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.AddFriend {

    [Serializable]
    public class AddFriendNotification : Notification {
        public string Username { get; set; }
    }

}