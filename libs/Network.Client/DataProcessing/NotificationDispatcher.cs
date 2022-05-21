using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Client.DataProcessing {

    public class NotificationDispatcher {
        public NotificationDispatcher(Notification notification) {
            _Notification = notification;
        }

        public void Dispatch<T>(Action<T> function) where T : Notification {
            if (_Notification.GetType() == typeof(T)) {
                function((T)_Notification);
            }
        }

        private readonly Notification _Notification;
    }

}