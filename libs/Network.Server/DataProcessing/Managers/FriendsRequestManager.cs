using System;
using System.IO;

using System.Linq;
using System.Collections.Generic;

using Network.Server.Core;
using Network.Server.Database;
using Network.Server.Model;

using Network.Shared.Core;
using Network.Shared.DataTransfer.Base;

using Network.Shared.DataTransfer.Model.Friends.ManageInvitations.AcceptFriendInvitation;
using Network.Shared.DataTransfer.Model.Friends.ManageInvitations.SendFriendInvitation;

using Network.Shared.DataTransfer.Model.Friends.ManageMessages.DownloadImage;
using Network.Shared.DataTransfer.Model.Friends.ManageMessages.DeleteMessage;
using Network.Shared.DataTransfer.Model.Friends.ManageMessages.SendMessage;

using Network.Shared.DataTransfer.Model.Friends.VoiceChat.AcceptVoiceChat;
using Network.Shared.DataTransfer.Model.Friends.VoiceChat.DisconnectVoiceChat;
using Network.Shared.DataTransfer.Model.Friends.VoiceChat.StartVoiceChat;

namespace Network.Server.DataProcessing.Managers {

    internal static class FriendsRequestManager {
        public static void Dispatch(RequestDispatcher dispatcher, ClientInfo client) {
            if (dispatcher.Request.AccessToken == client.AccessToken) {
                // Manage invitations
                dispatcher.Dispatch<SendFriendInvitationRequest>(OnSendFriendInvitationRequest, client);
                dispatcher.Dispatch<AcceptFriendInvitationRequest>(OnAcceptFriendInvitationRequest, client);

                // Manage messages
                dispatcher.Dispatch<DownloadImageRequest>(OnDownloadImageRequest, client);
                dispatcher.Dispatch<DeleteMessageRequest>(OnDeleteMessageRequest, client);
                dispatcher.Dispatch<SendMessageRequest>(OnSendMessageRequest, client);

                // Voice chat
                dispatcher.Dispatch<StartVoiceChatRequest>(OnStartVoiceChatRequest, client);
                dispatcher.Dispatch<AcceptVoiceChatRequest>(OnAcceptVoiceChatRequest, client);
                dispatcher.Dispatch<DisconnectVoiceChatRequest>(OnDisconnectVoiceChatRequest, client);
            }
        }

        // Manage invitations
        private static RequestResult OnSendFriendInvitationRequest(SendFriendInvitationRequest request, ClientInfo client) {
            var response = new SendFriendInvitationResponse();
            response.Result = ResponseResult.Success;

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
            var response = new AcceptFriendInvitationResponse();
            response.Result = ResponseResult.Success;

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
                response.Status = (receiver != null) && (receiver.Status == true);

                response.Username = user_account.Username;
                response.ImageBytes = File.ReadAllBytes(invitation.Sender.UserImage);

                db.Friendships.Add(new Friendship() { UserID = client.ID, FriendID = user_account.ID });
                db.Friendships.Add(new Friendship() { UserID = user_account.ID, FriendID = client.ID });

                db.Messages.Add(new Message() { SenderID = client.ID, ReceiverID = user_account.ID, Content = "", SendDate = DateTime.UtcNow, IsRead = false });
                db.FriendInvitations.Remove(invitation);

                db.SaveChanges();

                if (receiver != null) {
                    user_account = db.Accounts.SingleOrDefault(p => p.ID == client.ID);

                    var notification = new AcceptFriendInvitationNotification() {
                        UserID = client.ID,
                        Status = client.Status,
                        Username = client.Username,
                        ImageBytes = File.ReadAllBytes(user_account.UserImage)
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
        private static RequestResult OnDownloadImageRequest(DownloadImageRequest request, ClientInfo client) {
            var response = new DownloadImageResponse() {
                Result = ResponseResult.Success,

                FriendID = request.FriendID,
                MessageID = request.MessageID,

                Filename = request.Filename,
                ImageBytes = File.ReadAllBytes("Resources/Images/" + request.Filename)
            };

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        private static RequestResult OnDeleteMessageRequest(DeleteMessageRequest request, ClientInfo client) {
            var response = new DeleteMessageResponse();
            response.Result = ResponseResult.Success;

            using (var db = new PiDbContext()) {
                var message = db.Messages.Find(request.MessageID);
                var receiver = Server.Data.Clients.Find(p => p.ID == request.FriendID);

                if (message == null || message.SenderID != client.ID || message.ReceiverID != request.FriendID) {
                    throw new ArgumentException();
                }

                response.FriendID = request.FriendID;
                response.MessageID = request.MessageID;

                message.Content = "";
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
                Result = ResponseResult.Success,
                Images = new List<string>()
            };

            if (request.Content.Length == 0 || request.Content.Length > Values.MaxMessageLength) {
                throw new ArgumentException();
            }

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

                foreach (var image_data in request.Images) {
                    var filename = TokenGenerator.Next() + ".jpg";
                    var image_path = "Resources/Images/" + filename;

                    using (var img = System.Drawing.Image.FromStream(new MemoryStream(image_data))) {
                        img.Save(image_path, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }

                    var image = new Image() {
                        MessageID = message.ID,
                        Filename = filename
                    };

                    response.Images.Add(filename);
                    db.Images.Add(image);
                }

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
                        SendDate = current_time,

                        Images = response.Images
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

        // Voice chat
        private static RequestResult OnStartVoiceChatRequest(StartVoiceChatRequest request, ClientInfo client) {
            var receiver = Server.Data.Clients.Find(p => p.ID == request.FriendID);

            var response = new StartVoiceChatResponse() {
                FriendID = request.FriendID,
                Result = ResponseResult.Success
            };

            if (receiver != null) {
                var notification = new StartVoiceChatNotification() {
                    FriendID = client.ID
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

        private static RequestResult OnAcceptVoiceChatRequest(AcceptVoiceChatRequest request, ClientInfo client) {
            var receiver = Server.Data.Clients.Find(p => p.ID == request.FriendID);

            if (receiver != null) {
                var client_endpoint = (client.ExternalEndPoint.Address.ToString() != receiver.ExternalEndPoint.Address.ToString()) ? client.ExternalEndPoint : client.InternalEndPoint;
                var receiver_endpoint = (client.ExternalEndPoint.Address.ToString() != receiver.ExternalEndPoint.Address.ToString()) ? receiver.ExternalEndPoint : receiver.InternalEndPoint;

                var response = new AcceptVoiceChatResponse() {
                    FriendID = receiver.ID,
                    EndPoint = receiver_endpoint,
                    Result = ResponseResult.Success
                };

                var notification = new AcceptVoiceChatNotification() {
                    FriendID = client.ID,
                    EndPoint = client_endpoint,

                    Key = request.Key,
                    IV = request.IV
                };

                return new RequestResult() {
                    ResponseReceiver = client,
                    ResponseData = response,

                    NotificationReceivers = new List<ClientInfo>() { receiver },
                    NotificationData = notification
                };
            }

            return null;
        }

        private static RequestResult OnDisconnectVoiceChatRequest(DisconnectVoiceChatRequest request, ClientInfo client) {
            var receiver = Server.Data.Clients.Find(p => p.ID == request.FriendID);

            if (receiver != null) {
                var notification = new DisconnectVoiceChatNotification() {
                    FriendID = client.ID
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