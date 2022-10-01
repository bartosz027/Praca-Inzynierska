using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Settings.ChangeAvatar {

    [Serializable]
    public class ChangeAvatarNotification : Notification {
        public int FriendID { get; set; }
        public byte[] UserImage { get; set; }
    }

}