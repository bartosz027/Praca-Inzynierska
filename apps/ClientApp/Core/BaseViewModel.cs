using System;

using Network.Client;
using Network.Client.DataProcessing;

using Network.Shared.DataTransfer.Base;

namespace ClientApp.Core {

    public abstract class BaseViewModel : ObservableObject {
        public BaseViewModel() {
            Client.Instance.ResponseReceived += OnResponseReceived;
            Client.Instance.NotificationReceived += OnNotificationReceived;
        }

        public void Dispose() {
            Client.Instance.ResponseReceived -= OnResponseReceived;
            Client.Instance.NotificationReceived -= OnNotificationReceived;
        }

        // Response events
        protected void OnResponseReceived(object sender, Response response) {
            App.Current.Dispatcher.Invoke(delegate {
                var dispatcher = new ResponseDispatcher(response);

                if(response.Result == Result.None) {
                    throw new NotImplementedException();
                }

                ResponseReceived(dispatcher);
            });
        }

        protected virtual void ResponseReceived(ResponseDispatcher dispatcher) {

        }

        // Notification events
        protected void OnNotificationReceived(object sender, Notification notification) {
            App.Current.Dispatcher.Invoke(delegate {
                var dispatcher = new NotificationDispatcher(notification);
                NotificationReceived(dispatcher);
            });
        }

        protected virtual void NotificationReceived(NotificationDispatcher dispatcher) {

        }
    }

}