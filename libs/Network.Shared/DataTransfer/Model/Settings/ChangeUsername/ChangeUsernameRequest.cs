using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Settings.ChangeUsername {

    [Serializable]
    public class ChangeUsernameRequest : Request {
        public string Username { get; set; }
    }

}