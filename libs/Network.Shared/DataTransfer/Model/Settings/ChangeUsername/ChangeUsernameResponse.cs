using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Settings.ChangeUsername {

    [Serializable]
    public class ChangeUsernameResponse : Response {
        public string Username { get; set; }
    }

}