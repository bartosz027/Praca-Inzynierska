using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Account.Register {

    [Serializable]
    public class VerifyEmailRequest : Request {
        public string Email { get; set; }
        public string Code { get; set; }
    }

}