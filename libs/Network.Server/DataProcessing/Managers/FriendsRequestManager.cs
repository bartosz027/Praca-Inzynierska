using System.Collections.Generic;
using System.Linq;

using Network.Server.Database;

using Network.Shared.DataTransfer.Model.Database.Friends;
using Network.Shared.DataTransfer.Model.Friends.SendMessage;

namespace Network.Server.DataProcessing.Managers {

    internal static class FriendsRequestManager {
        public static void Dispatch(RequestDispatcher dispatcher, ClientInfo client) {
            if (dispatcher.Request.AccessToken == client.AccessToken) {
                dispatcher.Dispatch<SendMessageRequest>(OnSendMessageRequest, client);
            }
        }

        private static RequestResult OnSendMessageRequest(SendMessageRequest request, ClientInfo client) {
            var receiver = Server.Data.Clients.Find(p => p.UserID == request.ReceiverID);

            using (var db = new PiDbContext()) {
                db.Messages.Add(new Message() {
                    SenderID = client.UserID,
                    ReceiverID = request.ReceiverID,
                    Content = request.Content
                });

                db.SaveChanges();
            }

            if(receiver != null) {
                var notification = new SendMessageNotification() {
                    SenderID = client.UserID,
                    Content = request.Content
                };

                return new RequestResult() {
                    NotificationReceivers = new List<ClientInfo>() { receiver },
                    NotificationData = notification
                };
            }

            return null;
        }
    }

}