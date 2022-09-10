using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Account.SendVerificationCode {

    [Serializable]
    public class SendVerificationCodeRequest : Request {
        public string Email { get; set; }
    }

}