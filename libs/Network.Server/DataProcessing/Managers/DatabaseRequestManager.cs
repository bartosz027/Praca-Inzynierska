using System;
using System.Collections.Generic;
using System.Linq;

using Network.Server.Database;
using Network.Shared.DataTransfer.Model.Database.Friends;

namespace Network.Server.DataProcessing.Managers {

    internal static class DatabaseRequestManager {
        public static void Dispatch(RequestDispatcher dispatcher, ClientInfo client) {
            if (dispatcher.Request.AccessToken == client.AccessToken) {
                dispatcher.Dispatch<FriendListRequest>(OnFriendListRequest, client);
                dispatcher.Dispatch<MessageHistoryRequest>(OnMessageHistoryRequest, client);
            }
        }

        private static RequestResult OnFriendListRequest(FriendListRequest request, ClientInfo client) {
            var response = new FriendListResponse();
            response.FriendList = new List<FriendInfo>();

            using (var db = new PiDbContext()) {
                var user_account = db.Accounts.Where(p => p.ID == client.UserID).SingleOrDefault();
                var user_friend_list = user_account.Friends.ToList();

                foreach (var account in user_friend_list) {
                    var friend_info = new FriendInfo() {
                        UserID = account.Friend.ID,
                        Username = account.Friend.Username
                    };

                    response.FriendList.Add(friend_info);
                }
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        private static RequestResult OnMessageHistoryRequest(MessageHistoryRequest request, ClientInfo client) {
            throw new NotImplementedException();
        }
    }

}