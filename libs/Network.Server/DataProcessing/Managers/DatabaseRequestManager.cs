using System.Collections.Generic;

using System.Linq;
using Network.Server.Database;

using Network.Shared.DataTransfer.Base;
using Network.Shared.DataTransfer.Model.Database.Friends;

namespace Network.Server.DataProcessing.Managers {

    internal static class DatabaseRequestManager {
        public static void Dispatch(RequestDispatcher dispatcher, ClientInfo client) {
            if (dispatcher.Request.AccessToken == client.AccessToken) {
                dispatcher.Dispatch<GetFriendListRequest>(OnGetFriendListRequest, client);
                dispatcher.Dispatch<GetInvitationsRequest>(OnGetInvitationsRequest, client);
                dispatcher.Dispatch<GetMessageHistoryRequest>(OnGetMessageHistoryRequest, client);
            }

            // TODO: Access denied
        }

        private static RequestResult OnGetFriendListRequest(GetFriendListRequest request, ClientInfo client) {
            var response = new GetFriendListResponse() {
                FriendList = new List<FriendInfo>()
            };
            
            using (var db = new PiDbContext()) {
                var user_account = db.Accounts.Single(p => p.ID == client.ID);
                var user_friend_list = user_account.Friends.ToList();

                foreach (var account in user_friend_list) {
                    var friend_info = new FriendInfo() {
                        UserID = account.Friend.ID,
                        Username = account.Friend.Username
                    };

                    response.FriendList.Add(friend_info);
                }

                response.Result = Result.Success;
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

                response.Result = Result.Success;
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

                foreach (var message in messages) {
                    var message_info = new MessageInfo() {
                        ID = message.ID,
                        SenderID = message.SenderID,

                        Content = message.Content,
                        SendDate = message.SendDate
                    };

                    response.Messages.Add(message_info);
                }

                response.Result = Result.Success;
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }
    }

}