using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Account.VerifyAccessToken {

    [Serializable]
    public class VerifyAccessTokenResponse : Response {
        public string Username { get; set; }
    }

}