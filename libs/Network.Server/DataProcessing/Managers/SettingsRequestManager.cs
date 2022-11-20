using System.IO;
using System.Collections.Generic;

using System.Drawing;
using System.Drawing.Imaging;

using Network.Server.Core;
using Network.Server.Database;
using Network.Server.Model;

using Network.Shared.DataTransfer.Base;
using Network.Shared.DataTransfer.Model.Settings.ChangeAvatar;
using Network.Shared.DataTransfer.Model.Settings.ChangePassword;
using Network.Shared.DataTransfer.Model.Settings.ChangeUsername;

namespace Network.Server.DataProcessing.Managers {

    internal static class SettingsRequestManager {
        public static void Dispatch(RequestDispatcher dispatcher, ClientInfo client) {
            dispatcher.Dispatch<ChangeAvatarRequest>(OnChangeAvatarRequest, client);
            dispatcher.Dispatch<ChangePasswordRequest>(OnChangePasswordRequest, client);
            dispatcher.Dispatch<ChangeUsernameRequest>(OnChangeUsernameRequest, client);
        }

        private static RequestResult OnChangeAvatarRequest(ChangeAvatarRequest request, ClientInfo client) {
            using var db = new PiDbContext();
            var image_path = "Resources/Avatars/" + TokenGenerator.Next() + ".jpg";

            using (var img = System.Drawing.Image.FromStream(new MemoryStream(request.UserImage))) {
                img.Save(image_path, ImageFormat.Jpeg);
            }

            var user_account = db.Accounts.Find(client.ID);
            user_account.UserImage = image_path;
            db.SaveChanges();

            var receivers = new List<ClientInfo>();
            var notification = new ChangeAvatarNotification() {
                FriendID = client.ID,
                UserImage = request.UserImage
            };

            foreach (var friendship in user_account.Friends) {
                var friend_info = Server.Data.Clients.Find(p => p.ID == friendship.FriendID);

                if (friend_info != null) {
                    receivers.Add(friend_info);
                }
            }

            return new RequestResult() {
                NotificationReceivers = receivers,
                NotificationData = notification
            };
        }

        private static RequestResult OnChangePasswordRequest(ChangePasswordRequest request, ClientInfo client) {
            using (var db = new PiDbContext()) {
                var user_account = db.Accounts.Find(client.ID);
                user_account.Password = PasswordHasher.Hash(request.Password);
                db.SaveChanges();
            }

            return null;
        }

        private static RequestResult OnChangeUsernameRequest(ChangeUsernameRequest request, ClientInfo client) {
            using var db = new PiDbContext();

            var user_account = db.Accounts.Find(client.ID);
            user_account.Username = request.Username;
            db.SaveChanges();

            var receivers = new List<ClientInfo>();
            var notification = new ChangeUsernameNotification() {
                FriendID = client.ID,
                Username = request.Username
            };

            foreach (var friendship in user_account.Friends) {
                var friend_info = Server.Data.Clients.Find(p => p.ID == friendship.FriendID);

                if (friend_info != null) {
                    receivers.Add(friend_info);
                }
            }

            var response = new ChangeUsernameResponse() {
                Username = request.Username,
                Result = ResponseResult.Success
            };

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response,

                NotificationReceivers = receivers,
                NotificationData = notification
            };
        }
    }

}