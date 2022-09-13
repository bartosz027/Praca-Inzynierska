using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Account.Logout {

    [Serializable]
    public class LogoutNotification : Notification {
        public int ID { get; set; }
    }

}