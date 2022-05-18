using System;
using System.Collections.Generic;

using Network.Server.DataProcessing.Test;
using Network.Shared.DataTransfer.Interface;

namespace Network.Server.DataProcessing {

    internal class RequestResult {
        // Response
        public ClientInfo ResponseReceiver { get; set; }
        public IResponse ResponseData { get; set; }

        // Notification
        public List<ClientInfo> NotificationReceivers { get; set; }
        public INotification NotificationData { get; set; }
    }

    internal static class RequestManager {
        public static RequestResult Dispatch(IRequest request, ClientInfo client) {
            var dispatcher = new RequestDispatcher(request);
            TestRequestManager.Dispatch(dispatcher, client);

            return dispatcher.Result;
        }
    }

}