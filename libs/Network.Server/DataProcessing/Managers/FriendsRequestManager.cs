using System;
using System.Collections.Generic;
using System.Linq;

using Network.Server.Database;
using Network.Shared.DataTransfer.Base;

using Network.Shared.DataTransfer.Model.Friends.AddFriend;
using Network.Shared.DataTransfer.Model.Friends.AcceptFriendInvitation;

using Network.Shared.DataTransfer.Model.Friends.DeleteMessage;
using Network.Shared.DataTransfer.Model.Friends.SendMessage;

namespace Network.Server.DataProcessing.Managers {

    internal static class FriendsRequestManager {
        public static void Dispatch(RequestDispatcher dispatcher, ClientInfo client) {
            if (dispatcher.Request.AccessToken == client.AccessToken) {
                dispatcher.Dispatch<AddFriendRequest>(OnAddFriendRequest, client);
                dispatcher.Dispatch<AcceptFriendInvitationRequest>(OnAcceptFriendInvitationRequest, client);

                dispatcher.Dispatch<DeleteMessageRequest>(OnDeleteMessageRequest, client);
                dispatcher.Dispatch<SendMessageRequest>(OnSendMessageRequest, client);
            }
        }

        private static RequestResult OnAddFriendRequest(AddFriendRequest request, ClientInfo client) {
            using var db = new PiDbContext();
           
            var user = db.Accounts.Where(p => p.Username == request.Username).SingleOrDefault();
            var response = new AddFriendResponse();

            if (user != null) {
                response.Status = STATUS.SUCCESS;
            }
            else {
                response.Status = STATUS.FAILURE;
            }

            var receiver = Server.Data.Clients.Find(p => p.UserID == user.ID);
            var notification = new AddFriendNotification();

            if(receiver != null) {
                notification.Username = client.Username;

                return new RequestResult() {
                    ResponseReceiver = client,
                    ResponseData = response,

                    NotificationReceivers = new List<ClientInfo>() { receiver },
                    NotificationData = notification
                };
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        private static RequestResult OnAcceptFriendInvitationRequest(AcceptFriendInvitationRequest request, ClientInfo client) {
            using var db = new PiDbContext();

            var user = db.Accounts.Where(p => p.Username == request.Username).SingleOrDefault();
            var receiver = Server.Data.Clients.Find(p => p.UserID == user.ID);

            var response = new AcceptFriendInvitationResponse();
            var notification = new AcceptFriendInvitationNotification();

            db.Friendships.Add(new Friendship() { UserID = client.UserID, FriendID = user.ID });
            db.Friendships.Add(new Friendship() { UserID = user.ID, FriendID = client.UserID });
            db.SaveChanges();

            response.UserID = user.ID;
            response.Username = user.Username;

            if (receiver != null) {
                notification.UserID = client.UserID;
                notification.Username = client.Username;

                return new RequestResult() {
                    ResponseReceiver = client,
                    ResponseData = response,

                    NotificationReceivers = new List<ClientInfo>() { receiver },
                    NotificationData = notification
                };
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
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