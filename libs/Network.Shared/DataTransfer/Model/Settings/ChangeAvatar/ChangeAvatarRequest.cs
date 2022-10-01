using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Settings.ChangeAvatar {

    [Serializable]
    public class ChangeAvatarRequest : Request {
        public byte[] UserImage { get; set; }
    }

}