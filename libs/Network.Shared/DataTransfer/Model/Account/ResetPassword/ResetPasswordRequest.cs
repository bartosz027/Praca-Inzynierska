using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Account.ResetPassword {
    
    [Serializable]
    public class ResetPasswordRequest : Request {
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string VerificationCode { get; set; }
    }

}