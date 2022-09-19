using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Database.Friends.GetAvatar {

    [Serializable]
    public class GetAvatarResponse : Response {
        public byte[] ImageBytes { get; set; }
    }

}