using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using System.Data.SqlTypes;
using System.Linq;

using Network.Server.Core;
using Network.Server.Database;
using Network.Server.Model;

using Network.Shared.Core;
using Network.Shared.DataTransfer.Base;

using Network.Shared.DataTransfer.Model.Account.Login;
using Network.Shared.DataTransfer.Model.Account.Logout;

using Network.Shared.DataTransfer.Model.Account.Register;
using Network.Shared.DataTransfer.Model.Account.ResetPassword;
using Network.Shared.DataTransfer.Model.Account.SendVerificationCode;

using Network.Shared.DataTransfer.Model.Account.VerifyAccessToken;
using Network.Shared.DataTransfer.Model.Account.VerifyEmail;

namespace Network.Server.DataProcessing.Managers {

    internal static class AccountRequestManager {
        public static void Dispatch(RequestDispatcher dispatcher, ClientInfo client) {
            if (String.IsNullOrEmpty(dispatcher.Request.AccessToken) || (dispatcher.Request.AccessToken != client.AccessToken)) {
                // Login
                dispatcher.Dispatch<LoginRequest>(OnLoginRequest, client);
                dispatcher.Dispatch<VerifyAccessTokenRequest>(OnVerifyAccessTokenRequest, client);

                // Register
                dispatcher.Dispatch<RegisterRequest>(OnRegisterRequest, client);
                dispatcher.Dispatch<VerifyEmailRequest>(OnVerifyEmailRequest, client);

                // Send verification code
                dispatcher.Dispatch<SendVerificationCodeRequest>(OnSendVerificationCodeRequest, client);

                // Reset password
                dispatcher.Dispatch<ResetPasswordRequest>(OnResetPasswordRequest, client);
            }
            else {
                // Logout
                dispatcher.Dispatch<LogoutRequest>(OnLogoutRequest, client);
            }
        }

        // Login
        private static RequestResult OnLoginRequest(LoginRequest request, ClientInfo client) {
            var response = new LoginResponse();
            response.Result = ResponseResult.Success;

            using var db = new PiDbContext();
            var user_account = db.Accounts.SingleOrDefault(p => p.Email == request.Email);

            if (user_account == null || PasswordHasher.Verify(request.Password, user_account.Password) == false) {
                response.Result = ResponseResult.Failure;
                response.Errors.Add(ErrorCode.InvalidEmailOrPassword);
            }
            else if (user_account.Verified == false) {
                response.Result = ResponseResult.Failure;
                response.Errors.Add(ErrorCode.AccountNotVerified);
            }
            else {
                Console.WriteLine("Client [id={0}, username={1}] connected!", user_account.ID, user_account.Username);

                var token = TokenGenerator.Next();
                user_account.AccessToken = token;

                client.ID = user_account.ID;
                client.Status = user_account.Status;
                client.Username = user_account.Username;
                client.AccessToken = token;

                Server.Data.Clients.Add(client);
                db.SaveChanges();

                response.ID = client.ID;
                response.Status = client.Status;

                response.Email = user_account.Email;
                response.Username = client.Username;
                response.AccessToken = client.AccessToken;

                var receivers = new List<ClientInfo>();
                var notification = new LoginNotification() {
                    ID = client.ID,
                    Status = client.Status
                };

                foreach (var friendship in user_account.Friends) {
                    var friend_info = Server.Data.Clients.Find(p => p.ID == friendship.FriendID);

                    if (friend_info != null) {
                        receivers.Add(friend_info);
                    }
                }

                return new RequestResult() {
                    ResponseReceiver = client,
                    ResponseData = response,

                    NotificationReceivers = receivers,
                    NotificationData = notification
                };
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        private static RequestResult OnVerifyAccessTokenRequest(VerifyAccessTokenRequest request, ClientInfo client) {
            var response = new VerifyAccessTokenResponse();
            response.Result = ResponseResult.Success;

            using var db = new PiDbContext();
            var user_account = db.Accounts.SingleOrDefault(p => p.AccessToken == request.AccessToken);

            if(user_account == null || String.IsNullOrEmpty(request.AccessToken)) {
                response.Result = ResponseResult.Failure;
            }
            else {
                Console.WriteLine("Client [id={0}, username={1}] connected!", user_account.ID, user_account.Username);

                client.ID = user_account.ID;
                client.Status = user_account.Status;
                client.Username = user_account.Username;
                client.AccessToken = user_account.AccessToken;

                response.ID = client.ID;
                response.Status = client.Status;

                response.Email = user_account.Email;
                response.Username = client.Username;

                Server.Data.Clients.Add(client);

                var receivers = new List<ClientInfo>();
                var notification = new LoginNotification() {
                    ID = client.ID,
                    Status = client.Status
                };

                foreach (var friendship in user_account.Friends) {
                    var friend_info = Server.Data.Clients.Find(p => p.ID == friendship.FriendID);

                    if (friend_info != null) {
                        receivers.Add(friend_info);
                    }
                }

                return new RequestResult() {
                    ResponseReceiver = client,
                    ResponseData = response,

                    NotificationReceivers = receivers,
                    NotificationData = notification
                };
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        // Register
        private static RequestResult OnRegisterRequest(RegisterRequest request, ClientInfo client) {
            var response = new RegisterResponse();
            response.Result = ResponseResult.Success;

            if (new EmailAddressAttribute().IsValid(request.Email) == false) {
                throw new ArgumentException();
            }

            if (request.Username.Length < Values.MinUsernameLength || request.Username.Length > Values.MaxUsernameLength) {
                throw new ArgumentException();
            }

            if (request.Password.Length < Values.MinPasswordLength || request.Password.Length > Values.MaxPasswordLength) {
                throw new ArgumentException();
            }

            using var db = new PiDbContext();
            var user_account = db.Accounts.SingleOrDefault(p => p.Email == request.Email);

            if (user_account != null && user_account.Verified) {
                response.Result = ResponseResult.Failure;
                response.Errors.Add(ErrorCode.EmailAddressTaken);
            }
            else {
                var account = new Account() {
                    Email = request.Email,
                    Username = request.Username,
                    Password = PasswordHasher.Hash(request.Password),

                    Status = true,
                    Verified = false,
                    
                    UserImage = "Resources/DefaultAvatar.jpg"
                };

                if (user_account != null) {
                    db.Accounts.Remove(user_account);
                }

                db.Accounts.Add(account);
                db.SaveChanges();
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        private static RequestResult OnVerifyEmailRequest(VerifyEmailRequest request, ClientInfo client) {
            var response = new VerifyEmailResponse();
            response.Result = ResponseResult.Success;

            using (var db = new PiDbContext()) {
                var user_account = db.Accounts.SingleOrDefault(p => p.Email == request.Email);
                var verification = db.Verifications.Where(p => p.Email == request.Email).OrderByDescending(t => t.ExpireDate).First();

                if (user_account == null || verification == null) {
                    throw new ArgumentException();
                }

                if (request.VerificationCode != verification.Code) {
                    response.Result = ResponseResult.Failure;
                    response.Errors.Add(ErrorCode.InvalidVerificationCode);
                }
                else {
                    var time = verification.ExpireDate - DateTime.Now;

                    if (time.TotalSeconds > 0) {
                        verification.ExpireDate = SqlDateTime.MinValue.Value;
                        user_account.Verified = true;
                        db.SaveChanges();
                    }
                    else {
                        response.Result = ResponseResult.Failure;
                        response.Errors.Add(ErrorCode.ExpiredVerificationCode);
                    }
                }
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        // Send verification code
        private static RequestResult OnSendVerificationCodeRequest(SendVerificationCodeRequest request, ClientInfo client) {
            var response = new SendVerificationCodeResponse();
            response.Result = ResponseResult.Success;

            using var db = new PiDbContext();
            var user_account = db.Accounts.SingleOrDefault(p => p.Email == request.Email);

            if (user_account == null) {
                response.Result = ResponseResult.Failure;
                response.Errors.Add(ErrorCode.AccountNotFound);
            }
            else {
                var code = TokenGenerator.Next().Substring(0, 6);
                var time = DateTime.Now.AddMinutes(15);

                var verification = new Verification() {
                    Code = code,
                    ExpireDate = time,
                    Email = request.Email
                };

                db.Verifications.Add(verification);
                db.SaveChanges();

                try {
                    SMTP.SendMail(request.Email, Values.EmailSubject, Values.EmailBody + ' ' + code);
                }
                catch (Exception e) {
                    Console.WriteLine(e);
                }
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        // Reset password
        private static RequestResult OnResetPasswordRequest(ResetPasswordRequest request, ClientInfo client) {
            var response = new ResetPasswordResponse();
            response.Result = ResponseResult.Success;

            using (var db = new PiDbContext()) {
                var user_account = db.Accounts.SingleOrDefault(p => p.Email == request.Email);
                var verification = db.Verifications.Where(p => p.Email == request.Email).OrderByDescending(t => t.ExpireDate).First();

                if (user_account == null || verification == null) {
                    throw new ArgumentException();
                }

                if (request.NewPassword.Length < Values.MinPasswordLength || request.NewPassword.Length > Values.MaxPasswordLength) {
                    throw new ArgumentException();
                }

                if (request.VerificationCode != verification.Code) {
                    response.Result = ResponseResult.Failure;
                    response.Errors.Add(ErrorCode.InvalidVerificationCode);
                }
                else {
                    var time = verification.ExpireDate - DateTime.Now;

                    if (time.TotalSeconds > 0) {
                        if (user_account.Verified == false) {
                            response.Result = ResponseResult.Failure;
                            response.Errors.Add(ErrorCode.AccountNotVerified);
                        }

                        if(response.Result == ResponseResult.Success) {
                            verification.ExpireDate = SqlDateTime.MinValue.Value;
                            user_account.Password = PasswordHasher.Hash(request.NewPassword);
                            db.SaveChanges();
                        }
                    }
                    else {
                        response.Result = ResponseResult.Failure;
                        response.Errors.Add(ErrorCode.ExpiredVerificationCode);
                    }
                }
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        // Logout
        private static RequestResult OnLogoutRequest(LogoutRequest request, ClientInfo client) {
            var response = new LogoutResponse();
            response.Result = ResponseResult.Success;

            using (var db = new PiDbContext()) {
                var user_account = db.Accounts.Find(client.ID);
                user_account.AccessToken = null;
                db.SaveChanges();

                var receivers = new List<ClientInfo>();
                var notification = new LogoutNotification() {
                    ID = client.ID,
                    EndPoint = client.ExternalEndPoint
                };

                foreach (var friendship in user_account.Friends) {
                    var friend_info = Server.Data.Clients.Find(p => p.ID == friendship.FriendID);

                    if (friend_info != null) {
                        receivers.Add(friend_info);
                    }
                }

                client.ID = 0;
                client.Status = false;
                client.Username = null;
                client.AccessToken = null;
                client.ExternalEndPoint = null;

                Server.Data.Clients.Remove(client);

                return new RequestResult() {
                    ResponseReceiver = client,
                    ResponseData = response,

                    NotificationReceivers = receivers,
                    NotificationData = notification
                };
            };
        }
    }

}