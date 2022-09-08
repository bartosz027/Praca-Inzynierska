using System;

using Network.Client;
using Network.Client.DataProcessing;

using Network.Shared.DataTransfer.Base;

namespace ClientApp.Core {

    public abstract class BaseVM : ObservableObject {
        public void EnableResponseListener() {
            Client.Instance.ResponseReceived += OnResponseReceivedEvent;
        }

        public void EnableNotificationListener() {
            Client.Instance.NotificationReceived += OnNotificationReceivedEvent;
        }

        // Response events
        protected void OnResponseReceivedEvent(object sender, Response response) {
            App.Current.Dispatcher.Invoke(delegate {
                var dispatcher = new ResponseDispatcher(response);

                if(response.Result == ResponseResult.None) {
                    throw new NotImplementedException();
                }

                OnResponseReceived(dispatcher);
            });
        }

        // Notification events
        protected void OnNotificationReceivedEvent(object sender, Notification notification) {
            App.Current.Dispatcher.Invoke(delegate {
                var dispatcher = new NotificationDispatcher(notification);
                OnNotificationReceived(dispatcher);
            });
        }

        // Template method pattern
        protected virtual void OnResponseReceived(ResponseDispatcher dispatcher) { }
        protected virtual void OnNotificationReceived(NotificationDispatcher dispatcher) { }
    }

}