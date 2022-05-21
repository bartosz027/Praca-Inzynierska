using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Account.Login {

    [Serializable]
    public class LoginResponse : Response {
        public string AccessToken { get; set; }
    }

}