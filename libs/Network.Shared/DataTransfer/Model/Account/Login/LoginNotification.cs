using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Account.Login {

    [Serializable]
    public class LoginNotification : Notification {
        public int ID { get; set; }
        public bool Status { get; set; }
    }

}