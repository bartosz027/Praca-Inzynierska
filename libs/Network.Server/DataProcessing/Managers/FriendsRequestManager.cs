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
        }

        // Manage invitations
        private static RequestResult OnSendFriendInvitationRequest(SendFriendInvitationRequest request, ClientInfo client) {
            var response = new SendFriendInvitationResponse() {
                Result = ResponseResult.Success
            };

            using var db = new PiDbContext();
            var user_account = db.Accounts.SingleOrDefault(p => p.ID == request.UserID);

            if (user_account == null) {
                response.Result = ResponseResult.Failure;
                response.Errors.Add(ErrorCode.AccountNotFound);
            }
            else {
                var receiver = Server.Data.Clients.Find(p => p.ID == user_account.ID);

                // SELF INVITE
                if (user_account.ID == client.ID) {
                    response.Result = ResponseResult.Failure;
                    response.Errors.Add(ErrorCode.InvitationSelfInvite);
                }

                // ALREADY ON FRIEND LIST
                else if (user_account.Friends.SingleOrDefault(p => p.FriendID == client.ID) != null) {
                    response.Result = ResponseResult.Failure;
                    response.Errors.Add(ErrorCode.InvitationAlreadyFriends);
                }

                // INVITATION DUPLICATE
                else if (db.FriendInvitations.SingleOrDefault(p => (p.SenderID == user_account.ID && p.ReceiverID == client.ID) || (p.SenderID == client.ID && p.ReceiverID == user_account.ID)) != null) {
                    response.Result = ResponseResult.Failure;
                    response.Errors.Add(ErrorCode.InvitationDuplicate);
                }

                // NO ERRORS
                else {
                    response.UserID = user_account.ID;
                    response.Username = user_account.Username;

                    db.FriendInvitations.Add(new FriendInvitation() {
                        SenderID = client.ID,
                        ReceiverID = user_account.ID
                    });

                    db.SaveChanges();
                }

                if (receiver != null && response.Result == ResponseResult.Success) {
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

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        private static RequestResult OnAcceptFriendInvitationRequest(AcceptFriendInvitationRequest request, ClientInfo client) {
            var response = new AcceptFriendInvitationResponse() {
                Result = ResponseResult.Success
            };

            using (var db = new PiDbContext()) {
                var user_account = db.Accounts.SingleOrDefault(p => p.ID == request.UserID);

                if (user_account == null) {
                    throw new ArgumentException();
                }

                var invitation = db.FriendInvitations.SingleOrDefault(p => p.SenderID == user_account.ID && p.ReceiverID == client.ID);
                var receiver = Server.Data.Clients.Find(p => p.ID == user_account.ID);

                if (invitation == null) {
                    throw new ArgumentException();
                }

                response.UserID = user_account.ID;
                response.Username = user_account.Username;

                db.Friendships.Add(new Friendship() { UserID = client.ID, FriendID = user_account.ID });
                db.Friendships.Add(new Friendship() { UserID = user_account.ID, FriendID = client.ID });

                db.FriendInvitations.Remove(invitation);
                db.SaveChanges();

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
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        // Manage messages
        private static RequestResult OnDeleteMessageRequest(DeleteMessageRequest request, ClientInfo client) {
            var response = new DeleteMessageResponse() {
                Result = ResponseResult.Success
            };

            using (var db = new PiDbContext()) {
                var message = db.Messages.Find(request.MessageID);
                var receiver = Server.Data.Clients.Find(p => p.ID == request.FriendID);

                if (message == null || message.SenderID != client.ID || message.ReceiverID != request.FriendID) {
                    throw new ArgumentException();
                }

                response.FriendID = request.FriendID;
                response.MessageID = request.MessageID;

                db.Messages.Remove(message);
                db.SaveChanges();

                if (receiver != null) {
                    var notification = new DeleteMessageNotification() {
                        FriendID = client.ID,
                        MessageID = request.MessageID
                    };

                    return new RequestResult() {
                        ResponseReceiver = client,
                        ResponseData = response,

                        NotificationReceivers = new List<ClientInfo>() { receiver },
                        NotificationData = notification
                    };
                }
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        private static RequestResult OnSendMessageRequest(SendMessageRequest request, ClientInfo client) {
            var response = new SendMessageResponse() {
                Result = ResponseResult.Success
            };

            using (var db = new PiDbContext()) {
                var friendship = db.Friendships.SingleOrDefault(p => p.UserID == client.ID && p.FriendID == request.FriendID);

                if (friendship == null || String.IsNullOrEmpty(request.Content)) {
                    throw new ArgumentException();
                }

                var receiver = Server.Data.Clients.Find(p => p.ID == request.FriendID);
                var current_time = DateTime.UtcNow;

                var message = db.Messages.Add(new Message() {
                    SenderID = client.ID,
                    ReceiverID = request.FriendID,

                    Content = request.Content,
                    SendDate = current_time
                });

                db.SaveChanges();
                db.Dispose();

                response.FriendID = request.FriendID;
                response.MessageID = message.ID;

                response.Content = request.Content;
                response.SendDate = current_time;

                if (receiver != null) {
                    var notification = new SendMessageNotification() {
                        FriendID = client.ID,
                        MessageID = message.ID,

                        Content = request.Content,
                        SendDate = current_time
                    };

                    return new RequestResult() {
                        ResponseReceiver = client,
                        ResponseData = response,

                        NotificationReceivers = new List<ClientInfo>() { receiver },
                        NotificationData = notification
                    };
                }
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }
    }

}