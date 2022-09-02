using System;
using System.Collections.Generic;

using System.Linq;
using Network.Server.Database;

using Network.Shared.Core;
using Network.Shared.DataTransfer.Base;

using Network.Shared.DataTransfer.Model.Friends.ManageInvitations.AcceptFriendInvitation;
using Network.Shared.DataTransfer.Model.Friends.ManageInvitations.SendFriendInvitation;

using Network.Shared.DataTransfer.Model.Friends.ManageMessages.DeleteMessage;
using Network.Shared.DataTransfer.Model.Friends.ManageMessages.SendMessage;

namespace Network.Server.DataProcessing.Managers {

    internal static class FriendsRequestManager {
        public static void Dispatch(RequestDispatcher dispatcher, ClientInfo client) {
            if (dispatcher.Request.AccessToken == client.AccessToken) {
                // Manage invitations
                dispatcher.Dispatch<SendFriendInvitationRequest>(OnSendFriendInvitationRequest, client);
                dispatcher.Dispatch<AcceptFriendInvitationRequest>(OnAcceptFriendInvitationRequest, client);

                // Manage messages
                dispatcher.Dispatch<DeleteMessageRequest>(OnDeleteMessageRequest, client);
                dispatcher.Dispatch<SendMessageRequest>(OnSendMessageRequest, client);
            }

            // TODO: Access denied
        }

        // Manage invitations
        private static RequestResult OnSendFriendInvitationRequest(SendFriendInvitationRequest request, ClientInfo client) {
            using var db = new PiDbContext();

            var user = db.Accounts.SingleOrDefault(p => p.ID == request.UserID);
            var response = new SendFriendInvitationResponse();

            if (user != null) {
                var receiver = Server.Data.Clients.Find(p => p.ID == user.ID);

                response.UserID = user.ID;
                response.Username = user.Username;

                // SELF INVITE
                if (user.ID == client.ID) {
                    response.Result = Result.Failure;
                    response.ErrorCode = ErrorCode.FriendInvitationSelfInvite;
                }

                // ALREADY ON FRIEND LIST
                else if (user.Friends.SingleOrDefault(p => p.FriendID == client.ID) != null) {
                    response.Result = Result.Failure;
                    response.ErrorCode = ErrorCode.FriendInvitationAlreadyFriends;
                }

                // INVITATION DUPLICATE
                else if (db.FriendInvitations.SingleOrDefault(p => (p.SenderID == user.ID && p.ReceiverID == client.ID) || (p.SenderID == client.ID && p.ReceiverID == user.ID)) != null) {
                    response.Result = Result.Failure;
                    response.ErrorCode = ErrorCode.FriendInvitationDuplicate;
                }

                // NO ERRORS
                else {
                    response.Result = Result.Success;

                    db.FriendInvitations.Add(new FriendInvitation() {
                        SenderID = client.ID,
                        ReceiverID = user.ID
                    });

                    db.SaveChanges();
                    db.Dispose();
                }

                if (receiver != null && response.Result != Result.Failure) {
                    var notification = new SendFriendInvitationNotification() {
                        UserID = client.ID,
                        Username = client.Username
                    };

                    return new RequestResult() {
                        ResponseReceiver = client,
                        ResponseData = response,

                        NotificationReceivers = new List<ClientInfo>() { receiver },
                        NotificationData = notification
                    };
                }
            }
            else {
                response.Result = Result.Failure;
                response.ErrorCode = ErrorCode.FriendInvitationUserNotExist;
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        private static RequestResult OnAcceptFriendInvitationRequest(AcceptFriendInvitationRequest request, ClientInfo client) {
            using var db = new PiDbContext();

            var user = db.Accounts.Single(p => p.ID == request.UserID);
            var receiver = Server.Data.Clients.Find(p => p.ID == user.ID);

            db.Friendships.Add(new Friendship() { UserID = client.ID, FriendID = user.ID });
            db.Friendships.Add(new Friendship() { UserID = user.ID, FriendID = client.ID });

            var invitation = db.FriendInvitations.Single(p => p.SenderID == user.ID && p.ReceiverID == client.ID);
            db.FriendInvitations.Remove(invitation);

            db.SaveChanges();
            db.Dispose();

            var response = new AcceptFriendInvitationResponse() {
                UserID = user.ID,
                Username = user.Username,

                Result = Result.Success,
                ErrorCode = ErrorCode.None
            };

            if (receiver != null) {
                var notification = new AcceptFriendInvitationNotification() {
                    UserID = client.ID,
                    Username = client.Username
                };

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

        // Manage messages
        private static RequestResult OnDeleteMessageRequest(DeleteMessageRequest request, ClientInfo client) {
            var receiver = Server.Data.Clients.Find(p => p.ID == request.FriendID);

            using (var db = new PiDbContext()) {
                var message = db.Messages.Find(request.MessageID);
                db.Messages.Remove(message);
                db.SaveChanges();
            }

            if (receiver != null) {
                var notification = new DeleteMessageNotification() {
                    FriendID = client.ID,
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
            var receiver = Server.Data.Clients.Find(p => p.ID == request.FriendID);

            var time = DateTime.UtcNow;
            var message = new Message();

            using (var db = new PiDbContext()) {
                message = db.Messages.Add(new Message() {
                    SenderID = client.ID,
                    ReceiverID = request.FriendID,

                    Content = request.Content,
                    SendDate = time
                });

                db.SaveChanges();
            }

            var response = new SendMessageResponse() {
                FriendID = request.FriendID,
                MessageID = message.ID,

                Content = request.Content,
                SendDate = time,

                Result = Result.Success,
                ErrorCode = ErrorCode.None
            };

            if (receiver != null) {
                var notification = new SendMessageNotification() {
                    FriendID = client.ID,
                    MessageID = message.ID,

                    Content = request.Content,
                    SendDate = time
                };

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
    }

}