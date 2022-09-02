using System;
using System.Data.SqlTypes;
using System.Linq;

using Network.Server.Core;
using Network.Server.Database;

using Network.Shared.Core;
using Network.Shared.DataTransfer.Base;

using Network.Shared.DataTransfer.Model.Account.Login;
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
        }

        // Login
        private static RequestResult OnLoginRequest(LoginRequest request, ClientInfo client) {
            using var db = new PiDbContext();
            var user_account = db.Accounts.SingleOrDefault(p => p.Email == request.Email);

            var response = new LoginResponse();
            response.Result = Result.Success;

            if (user_account != null && user_account.Password == request.Password) {
                var client_info = Server.Data.Clients.Find(p => p.ID == user_account.ID);

                if (client_info == null) {
                    var token = TokenGenerator.Next();
                    Console.WriteLine("Client [id={0}, username={1}] connected!", user_account.ID, user_account.Username);

                    user_account.AccessToken = token;
                    db.SaveChanges();

                    response.Username = user_account.Username;
                    response.AccessToken = token;

                    client.ID = user_account.ID;
                    client.Username = user_account.Username;

                    client.AccessToken = token;
                    Server.Data.Clients.Add(client);
                }
                else {
                    // TODO: Disconnect previous instance
                    throw new NotImplementedException();
                }
            }
            else {
                response.Result = Result.Failure;
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        // Register
        private static RequestResult OnRegisterRequest(RegisterRequest request, ClientInfo client) {
            using var db = new PiDbContext();
            var user_account = db.Accounts.SingleOrDefault(p => p.Email == request.Email);

            var response = new RegisterResponse();
            response.Result = Result.Success;

            if (user_account == null || user_account.Verified == false) {
                var code = TokenGenerator.Next().Substring(0, 6);
                var time = DateTime.Now.AddMinutes(15);

                var verification = new Verification() {
                    Email = request.Email,

                    Code = code,
                    ExpireDate = time
                };

                var account = new Account() {
                    Email = request.Email,
                    Username = request.Username,
                    Password = request.Password,
                    Verified = false
                };

                if (user_account != null) {
                    db.Accounts.Remove(user_account);
                }

                db.Verifications.Add(verification);
                db.Accounts.Add(account);

                SMTP.SendMail(request.Email, "PI account verification", "Verification code: " + code);
                db.SaveChanges();
            }
            else {
                response.Result = Result.Failure;
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        private static RequestResult OnVerifyEmailRequest(VerifyEmailRequest request, ClientInfo client) {
            using var db = new PiDbContext();

            var user_account = db.Accounts.Single(p => p.Email == request.Email);
            var verification = db.Verifications.Where(p => p.Email == request.Email).OrderByDescending(t => t.ExpireDate).First();

            var response = new VerifyEmailResponse();
            response.Result = Result.Success;

            if (request.Code == verification.Code) {
                var time = (verification.ExpireDate - DateTime.Now).TotalSeconds;

                if (time > 0) {
                    user_account.Verified = true;
                    verification.ExpireDate = SqlDateTime.MinValue.Value;

                    db.SaveChanges();
                }
                else {
                    response.Result = Result.Failure;
                    response.ErrorCode = ErrorCode.ExpiredVerificationCode;
                }
            }
            else {
                response.Result = Result.Failure;
                response.ErrorCode = ErrorCode.InvalidVerificationCode;
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }
    }

}