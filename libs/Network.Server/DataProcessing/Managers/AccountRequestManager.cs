using System;
using System.ComponentModel.DataAnnotations;

using System.Data.SqlTypes;
using System.Linq;

using Network.Server.Core;
using Network.Server.Database;

using Network.Shared.Core;
using Network.Shared.DataTransfer.Base;

using Network.Shared.DataTransfer.Model.Account.Login;
using Network.Shared.DataTransfer.Model.Account.Logout;
using Network.Shared.DataTransfer.Model.Account.Register;

namespace Network.Server.DataProcessing.Managers {

    internal static class AccountRequestManager {
        public static void Dispatch(RequestDispatcher dispatcher, ClientInfo client) {
            if (String.IsNullOrEmpty(dispatcher.Request.AccessToken) || (dispatcher.Request.AccessToken != client.AccessToken)) {
                // Login
                dispatcher.Dispatch<LoginRequest>(OnLoginRequest, client);

                // Register
                dispatcher.Dispatch<RegisterRequest>(OnRegisterRequest, client);
                dispatcher.Dispatch<VerifyEmailRequest>(OnVerifyEmailRequest, client);
            }
            else {
                // Logout
                dispatcher.Dispatch<LogoutRequest>(OnLogoutRequest, client);
            }
        }

        // Login
        private static RequestResult OnLoginRequest(LoginRequest request, ClientInfo client) {
            var response = new LoginResponse() {
                Result = ResponseResult.Success
            };

            using var db = new PiDbContext();
            var user_account = db.Accounts.SingleOrDefault(p => p.Email == request.Email);

            if (user_account != null && user_account.Verified && PasswordHasher.Verify(request.Password, user_account.Password)) {
                if (Server.Data.Clients.Find(p => p.ID == user_account.ID) == null) {
                    Console.WriteLine("Client [id={0}, username={1}] connected!", user_account.ID, user_account.Username);

                    var token = TokenGenerator.Next();
                    user_account.AccessToken = token;

                    response.AccessToken = token;
                    response.Username = user_account.Username;

                    client.ID = user_account.ID;
                    client.Username = user_account.Username;

                    client.AccessToken = token;
                    Server.Data.Clients.Add(client);

                    db.SaveChanges();
                    db.Dispose();
                }
                else {
                    // TODO: Disconnect previous instance (duplicate login)
                    throw new NotImplementedException();
                }
            }
            else {
                response.Result = ResponseResult.Failure;
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        // Register
        private static RequestResult OnRegisterRequest(RegisterRequest request, ClientInfo client) {
            var response = new RegisterResponse() { 
                Result = ResponseResult.Success
            };

            if (new EmailAddressAttribute().IsValid(request.Email) == false) {
                response.Result = ResponseResult.Failure;
                response.Errors.Add(ErrorCode.InvalidEmailAddress);
            }

            if (request.Username.Length < Values.MinUsernameLength || request.Username.Length > Values.MaxUsernameLength) {
                response.Result = ResponseResult.Failure;
                response.Errors.Add(ErrorCode.InvalidUsername);
            }

            if (request.Password.Length < Values.MinPasswordLength || request.Password.Length > Values.MaxPasswordLength) {
                response.Result = ResponseResult.Failure;
                response.Errors.Add(ErrorCode.InvalidPassword);
            }

            if (response.Result == ResponseResult.Success) {
                using var db = new PiDbContext();
                var user_account = db.Accounts.SingleOrDefault(p => p.Email == request.Email);

                if(user_account != null && user_account.Verified) {
                    response.Result = ResponseResult.Failure;
                    response.Errors.Add(ErrorCode.EmailAddressTaken);
                }
                else {
                    var code = TokenGenerator.Next().Substring(0, 6);
                    var time = DateTime.Now.AddMinutes(15);

                    var account = new Account() {
                        Email = request.Email,
                        Username = request.Username,
                        Password = PasswordHasher.Hash(request.Password),
                        Verified = false
                    };

                    var verification = new Verification() {
                        Code = code,
                        ExpireDate = time,
                        Email = request.Email
                    };

                    if (user_account != null) {
                        db.Accounts.Remove(user_account);
                    }

                    db.Accounts.Add(account);
                    db.Verifications.Add(verification);

                    db.SaveChanges();
                    db.Dispose();

                    try {
                        SMTP.SendMail(request.Email, Values.EmailSubject, Values.EmailBody + ' ' + code);
                    }
                    catch {
                        // TODO: Account locked exception
                    }
                }
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        private static RequestResult OnVerifyEmailRequest(VerifyEmailRequest request, ClientInfo client) {
            var response = new VerifyEmailResponse() {
                Result = ResponseResult.Success
            };

            if (new EmailAddressAttribute().IsValid(request.Email) == false) {
                response.Result = ResponseResult.Failure;
                response.Errors.Add(ErrorCode.InvalidEmailAddress);
            }

            if (response.Result == ResponseResult.Success) {
                using var db = new PiDbContext();

                var user_account = db.Accounts.SingleOrDefault(p => p.Email == request.Email);
                var verification = db.Verifications.Where(p => p.Email == request.Email).OrderByDescending(t => t.ExpireDate).First();

                if (user_account == null || verification == null) {
                    return null;
                }

                if (request.Code != verification.Code) {
                    response.Result = ResponseResult.Failure;
                    response.Errors.Add(ErrorCode.InvalidVerificationCode);
                }
                else {
                    var time = verification.ExpireDate - DateTime.Now;

                    if (time.TotalSeconds > 0) {
                        user_account.Verified = true;
                        verification.ExpireDate = SqlDateTime.MinValue.Value;

                        db.SaveChanges();
                        db.Dispose();
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
            var response = new LogoutResponse() {
                Result = ResponseResult.Success
            };

            using (var db = new PiDbContext()) {
                var user_account = db.Accounts.Find(client.ID);
                user_account.AccessToken = null;
                db.SaveChanges();
            };

            client.ID = 0;
            client.Username = null;
            client.AccessToken = null;
            Server.Data.Clients.Remove(client);

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }
    }

}