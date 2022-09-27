using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Account.VerifyAccessToken {

    [Serializable]
    public class VerifyAccessTokenResponse : Response {
        public int ID { get; set; }
        public bool Status { get; set; }

        public string Email { get; set; }
        public string Username { get; set; }
    }

}