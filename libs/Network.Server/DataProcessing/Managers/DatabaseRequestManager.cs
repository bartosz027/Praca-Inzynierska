using System;
using System.Collections.Generic;
using System.Linq;

using Network.Server.Database;
using Network.Shared.DataTransfer.Base;

using Network.Shared.DataTransfer.Model.Database.Friends.GetFriendList;
using Network.Shared.DataTransfer.Model.Database.Friends.GetInvitations;
using Network.Shared.DataTransfer.Model.Database.Friends.GetMessageHistory;

using Network.Shared.DataTransfer.Model.Database.Friends.SetMessageRead;

namespace Network.Server.DataProcessing.Managers {

    internal static class DatabaseRequestManager {
        public static void Dispatch(RequestDispatcher dispatcher, ClientInfo client) {
            if (dispatcher.Request.AccessToken == client.AccessToken) {
                // Get data
                dispatcher.Dispatch<GetFriendListRequest>(OnGetFriendListRequest, client);
                dispatcher.Dispatch<GetInvitationsRequest>(OnGetInvitationsRequest, client);
                dispatcher.Dispatch<GetMessageHistoryRequest>(OnGetMessageHistoryRequest, client);

                // Set data
                dispatcher.Dispatch<SetMessageReadRequest>(OnSetMessageReadRequest, client);
            }
        }

        // Get data
        private static RequestResult OnGetFriendListRequest(GetFriendListRequest request, ClientInfo client) {
            var response = new GetFriendListResponse() {
                FriendList = new List<FriendInfo>(),
                Result = ResponseResult.Success
            };
            
            using (var db = new PiDbContext()) {
                var user_account = db.Accounts.Find(client.ID);
                var user_friend_list = user_account.Friends.ToList();

                foreach (var account in user_friend_list) {
                    var friend_info = new FriendInfo() {
                        UserID = account.Friend.ID,
                        Username = account.Friend.Username
                    };

                    var messages = db.Messages.Where(p => (p.SenderID == client.ID && p.ReceiverID == friend_info.UserID) || (p.SenderID == friend_info.UserID && p.ReceiverID == client.ID));
                    var message = messages.OrderByDescending(t => t.SendDate).FirstOrDefault();

                    if(message != null) {
                        friend_info.IsLastMessageRead = (message.SenderID == client.ID) || (message.IsRead == true);
                        friend_info.LastMessageSendDate = message.SendDate;
                    }

                    var client_info = Server.Data.Clients.Find(p => p.ID == account.Friend.ID);
                    friend_info.Status = (client_info != null) ? client_info.Status : false;

                    response.FriendList.Add(friend_info);
                }
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        private static RequestResult OnGetInvitationsRequest(GetInvitationsRequest request, ClientInfo client) {
            var response = new GetInvitationsResponse() {
                PendingInvitations = new List<FriendInvitationInfo>(),
                ReceivedInvitations = new List<FriendInvitationInfo>()
            };

            using (var db = new PiDbContext()) {
                var invitations = db.FriendInvitations.Where(p => p.SenderID == client.ID || p.ReceiverID == client.ID);
                response.Result = ResponseResult.Success;

                foreach (var invitation in invitations) {
                    if (invitation.SenderID == client.ID) {
                        var invitation_info = new FriendInvitationInfo() {
                            UserID = invitation.Receiver.ID,
                            Username = invitation.Receiver.Username
                        };

                        response.PendingInvitations.Add(invitation_info);
                    }

                    if (invitation.ReceiverID == client.ID) {
                        var invitation_info = new FriendInvitationInfo() {
                            UserID = invitation.Sender.ID,
                            Username = invitation.Sender.Username
                        };

                        response.ReceivedInvitations.Add(invitation_info);
                    }
                }
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        private static RequestResult OnGetMessageHistoryRequest(GetMessageHistoryRequest request, ClientInfo client) {
            var response = new GetMessageHistoryResponse() {
                FriendID = request.FriendID,
                Messages = new List<MessageInfo>()
            };

            using (var db = new PiDbContext()) {
                var messages = db.Messages.Where(p => (p.SenderID == client.ID && p.ReceiverID == request.FriendID) || (p.SenderID == request.FriendID && p.ReceiverID == client.ID));
                response.Result = ResponseResult.Success;

                foreach (var message in messages) {
                    if(String.IsNullOrEmpty(message.Content)) {
                        continue;
                    }

                    var message_info = new MessageInfo() {
                        ID = message.ID,
                        SenderID = message.SenderID,

                        Content = message.Content,
                        SendDate = message.SendDate
                    };

                    response.Messages.Add(message_info);
                }
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        // Set data
        private static RequestResult OnSetMessageReadRequest(SetMessageReadRequest request, ClientInfo client) {
            using (var db = new PiDbContext()) {
                var messages = db.Messages.Where(p => p.SenderID == request.FriendID && p.ReceiverID == client.ID);
                var message = messages.OrderByDescending(p => p.SendDate).FirstOrDefault();

                if(message != null) {
                    message.IsRead = true;
                    db.SaveChanges();
                }
            }

            return null;
        }
    }

}