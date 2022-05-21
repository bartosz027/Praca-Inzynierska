using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Account.Login {

    [Serializable]
    public class LoginRequest : Request {
        public string Email { get; set; }
        public string Password { get; set; }
    }

}