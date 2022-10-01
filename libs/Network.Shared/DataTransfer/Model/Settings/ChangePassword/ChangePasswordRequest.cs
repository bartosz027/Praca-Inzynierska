using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Settings.ChangePassword {

    [Serializable]
    public class ChangePasswordRequest : Request {
        public string Password { get; set; }
    }

}