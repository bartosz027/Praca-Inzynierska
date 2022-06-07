using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Account.Login {

    [Serializable]
    public class LoginResponse : Response {
        public string Username { get; set; }
        public string AccessToken { get; set; }
    }

}