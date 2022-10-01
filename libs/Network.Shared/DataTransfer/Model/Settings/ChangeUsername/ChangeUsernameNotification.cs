using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Settings.ChangeUsername {

    [Serializable]
    public class ChangeUsernameNotification : Notification {
        public int FriendID { get; set; }
        public string Username { get; set; }
    }

}