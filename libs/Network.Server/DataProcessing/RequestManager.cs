using System.Collections.Generic;
using Network.Shared.DataTransfer.Base;

using Network.Server.Model;
using Network.Server.DataProcessing.Managers;

namespace Network.Server.DataProcessing {

    internal class RequestResult {
        // Response
        public ClientInfo ResponseReceiver { get; set; }
        public Response ResponseData { get; set; }

        // Notification
        public List<ClientInfo> NotificationReceivers { get; set; }
        public Notification NotificationData { get; set; }
    }

    internal static class RequestManager {
        public static RequestResult Dispatch(Request request, ClientInfo client) {
            var dispatcher = new RequestDispatcher(request);

            // Request managers
            AccountRequestManager.Dispatch(dispatcher, client);
            DatabaseRequestManager.Dispatch(dispatcher, client);
            FriendsRequestManager.Dispatch(dispatcher, client);
            SecurityRequestManager.Dispatch(dispatcher, client);

            return dispatcher.Result;
        }
    }

}