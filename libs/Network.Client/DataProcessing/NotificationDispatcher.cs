using System;
using Network.Shared.DataTransfer.Interface;

namespace Network.Client.DataProcessing {

    public class NotificationDispatcher {
        public NotificationDispatcher(INotification notification) {
            _Notification = notification;
        }

        public void Dispatch<T>(Action<T> function) where T : INotification {
            if (_Notification.GetType() == typeof(T)) {
                function((T)_Notification);
            }
        }

        private readonly INotification _Notification;
    }

}