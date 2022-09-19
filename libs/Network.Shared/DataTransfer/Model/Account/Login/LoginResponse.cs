using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Account.Login {

    [Serializable]
    public class LoginResponse : Response {
        public int ID { get; set; }
        public bool Status { get; set; }
        public string Username { get; set; }
        public string AccessToken { get; set; }
    }

}