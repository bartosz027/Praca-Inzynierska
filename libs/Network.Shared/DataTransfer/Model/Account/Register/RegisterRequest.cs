using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Account.Register {

    [Serializable]
    public class RegisterRequest : Request {
        // Login data
        public string Email { get; set; }
        public string Password { get; set; }

        // User data
        public string Username { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

}