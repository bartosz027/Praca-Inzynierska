using System;
using Network.Shared.DataTransfer.Model.Database.Friends;

namespace Network.Server.DataProcessing.Managers {

    internal static class DatabaseRequestManager {
        public static void Dispatch(RequestDispatcher dispatcher, ClientInfo client) {
            if (dispatcher.Request.AccessToken == client.AccessToken) {
                dispatcher.Dispatch<FriendsListRequest>(OnFriendsListRequest, client);
                dispatcher.Dispatch<MessageHistoryRequest>(OnMessageHistoryRequest, client);
            }
        }

        private static RequestResult OnFriendsListRequest(FriendsListRequest request, ClientInfo client) {
            throw new NotImplementedException();
        }

        private static RequestResult OnMessageHistoryRequest(MessageHistoryRequest request, ClientInfo client) {
            throw new NotImplementedException();
        }
    }

}