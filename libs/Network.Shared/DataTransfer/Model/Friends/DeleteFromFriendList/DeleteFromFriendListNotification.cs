using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.DeleteFromFriendList {

    [Serializable]
    public class DeleteFromFriendListNotification : Notification {
        public int FriendID { get; set; }
    }

}