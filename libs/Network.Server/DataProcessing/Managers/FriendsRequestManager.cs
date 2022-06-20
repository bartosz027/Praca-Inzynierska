using System;
using System.Collections.Generic;

using Network.Server.Database;

using Network.Shared.DataTransfer.Model.Friends.DeleteMessage;
using Network.Shared.DataTransfer.Model.Friends.SendMessage;

namespace Network.Server.DataProcessing.Managers {

    internal static class FriendsRequestManager {
        public static void Dispatch(RequestDispatcher dispatcher, ClientInfo client) {
            if (dispatcher.Request.AccessToken == client.AccessToken) {
                dispatcher.Dispatch<DeleteMessageRequest>(OnDeleteMessageRequest, client);
                dispatcher.Dispatch<SendMessageRequest>(OnSendMessageRequest, client);
            }
        }

        private static RequestResult OnDeleteMessageRequest(DeleteMessageRequest request, ClientInfo client) {
            var receiver = Server.Data.Clients.Find(p => p.UserID == request.FriendID);

            using (var db = new PiDbContext()) {
                var message = db.Messages.Find(request.MessageID);
                db.Messages.Remove(message);
                db.SaveChanges();
            }

            if (receiver != null) {
                var notification = new DeleteMessageNotification() {
                    FriendID = client.UserID,
                    MessageID = request.MessageID
                };

                return new RequestResult() {
                    NotificationReceivers = new List<ClientInfo>() { receiver },
                    NotificationData = notification
                };
            }

            return null;
        }

        private static RequestResult OnSendMessageRequest(SendMessageRequest request, ClientInfo client) {
            var receiver = Server.Data.Clients.Find(p => p.UserID == request.ReceiverID);
            var message = new Message();

            using (var db = new PiDbContext()) {
                message = db.Messages.Add(new Message() {
                    SenderID = client.UserID,
                    ReceiverID = request.ReceiverID,

                    Content = request.Content,
                    SendDate = request.SendDate
                });

                db.SaveChanges();
            }

            if(receiver != null) {
                var notification = new SendMessageNotification() {
                    MessageID = message.ID,
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