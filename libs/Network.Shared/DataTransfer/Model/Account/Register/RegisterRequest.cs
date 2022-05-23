using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Account.Register {

    [Serializable]
    public class RegisterRequest : Request {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

}