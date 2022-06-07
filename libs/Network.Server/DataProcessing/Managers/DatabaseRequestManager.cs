using System;
using System.Collections.Generic;
using System.Linq;
using Network.Server.Database;
using Network.Shared.DataTransfer.Model.Database.Friends;

namespace Network.Server.DataProcessing.Managers {

    internal static class DatabaseRequestManager {
        public static void Dispatch(RequestDispatcher dispatcher, ClientInfo client) {
            if (dispatcher.Request.AccessToken == client.AccessToken) {
                dispatcher.Dispatch<FriendsListRequest>(OnFriendsListRequest, client);
                dispatcher.Dispatch<MessageHistoryRequest>(OnMessageHistoryRequest, client);
            }
        }

        private static RequestResult OnFriendsListRequest(FriendsListRequest request, ClientInfo client)
        {
            var response = new FriendsListResponse();
            response.FriendsList = new List<FriendInfo>();

            using (var db = new PiDbContext())
            {
                var user_account = db.Accounts.Where(p => p.ID == client.UserID).SingleOrDefault();
                var user_friend_list = user_account.Friends.ToList();

                foreach (var acc in user_friend_list)
                {
                    var friend = new FriendInfo()
                    {
                        UserID = acc.User.ID,
                        Username = acc.User.Username
                    };

                    response.FriendsList.Add(friend);
                }
            }

            return new RequestResult()
            {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        private static RequestResult OnMessageHistoryRequest(MessageHistoryRequest request, ClientInfo client) {
            throw new NotImplementedException();
        }
    }

}