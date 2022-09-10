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
using Network.Shared.DataTransfer.Model.Account.ResetPassword;
using Network.Shared.DataTransfer.Model.Account.SendVerificationCode;

using Network.Shared.DataTransfer.Model.Account.VerifyEmail;

namespace Network.Server.DataProcessing.Managers {

    internal static class AccountRequestManager {
        public static void Dispatch(RequestDispatcher dispatcher, ClientInfo client) {
            if (String.IsNullOrEmpty(dispatcher.Request.AccessToken) || (dispatcher.Request.AccessToken != client.AccessToken)) {
                // Login
                dispatcher.Dispatch<LoginRequest>(OnLoginRequest, client);

                // Register
                dispatcher.Dispatch<RegisterRequest>(OnRegisterRequest, client);

                // Send verification code
                dispatcher.Dispatch<SendVerificationCodeRequest>(OnSendVerificationCodeRequest, client);

                // Reset password
                dispatcher.Dispatch<ResetPasswordRequest>(OnResetPasswordRequest, client);

                // Verify email
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

            // TODO: Handle duplicate login
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

                response.AccessToken = token;
                response.Username = user_account.Username;

                client.ID = user_account.ID;
                client.Username = user_account.Username;

                client.AccessToken = token;
                Server.Data.Clients.Add(client);

                db.SaveChanges();
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
                    var account = new Account() {
                        Email = request.Email,
                        Username = request.Username,
                        Password = PasswordHasher.Hash(request.Password)
                    };

                    if (user_account != null) {
                        db.Accounts.Remove(user_account);
                    }

                    db.Accounts.Add(account);
                    db.SaveChanges();
                }
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        // Send verification code
        private static RequestResult OnSendVerificationCodeRequest(SendVerificationCodeRequest request, ClientInfo client) {
            var response = new SendVerificationCodeResponse() {
                Result = ResponseResult.Success
            };

            if (new EmailAddressAttribute().IsValid(request.Email) == false) {
                response.Result = ResponseResult.Failure;
                response.Errors.Add(ErrorCode.InvalidEmailAddress);
            }

            using var db = new PiDbContext();
            var user_account = db.Accounts.SingleOrDefault(p => p.Email == request.Email);

            if(user_account == null) {
                response.Result = ResponseResult.Failure;
                response.Errors.Add(ErrorCode.AccountNotFound);
            }

            if (response.Result == ResponseResult.Success) {
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
                catch {
                    // TODO: Account locked exception
                }
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        // Reset password
        private static RequestResult OnResetPasswordRequest(ResetPasswordRequest request, ClientInfo client) {
            var response = new ResetPasswordResponse() {
                Result = ResponseResult.Success
            };

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
                        if (user_account.Verified == false) {
                            response.Result = ResponseResult.Failure;
                            response.Errors.Add(ErrorCode.AccountNotVerified);
                        }

                        if (request.NewPassword.Length < Values.MinPasswordLength || request.NewPassword.Length > Values.MaxPasswordLength) {
                            response.Result = ResponseResult.Failure;
                            response.Errors.Add(ErrorCode.InvalidPassword);
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

        // Verify email
        private static RequestResult OnVerifyEmailRequest(VerifyEmailRequest request, ClientInfo client) {
            var response = new VerifyEmailResponse() {
                Result = ResponseResult.Success
            };

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