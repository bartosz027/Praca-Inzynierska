using System;
using System.Linq;

using Network.Server.Core;
using Network.Server.Database;

using Network.Shared.DataTransfer.Base;

using Network.Shared.DataTransfer.Model.Account.Login;
using Network.Shared.DataTransfer.Model.Account.Register;

namespace Network.Server.DataProcessing.Account {

    internal static class AccountRequestManager {
        public static void Dispatch(RequestDispatcher dispatcher, ClientInfo client) {
            // Login
            dispatcher.Dispatch<LoginRequest>(OnLoginRequest, client);

            // Register
            dispatcher.Dispatch<RegisterRequest>(OnRegisterRequest, client);
            dispatcher.Dispatch<VerifyEmailRequest>(OnVerifyEmailRequest, client);
        }

        // Login
        private static RequestResult OnLoginRequest(LoginRequest request, ClientInfo client) {
            using var db = new PiDbContext();
            
            var user_account = db.Accounts.Where(p => p.Email == request.Email).SingleOrDefault();
            var response = new LoginResponse();

            if (user_account != null && user_account.Password == request.Password) {
                var client_info = Server.Data.Clients.Find(p => p.UserID == user_account.ID);

                if (client_info == null) {
                    var token = TokenGenerator.Next();
                    Console.WriteLine("Client [id={0}, username={1}] connected!", user_account.ID, user_account.Username);

                    user_account.AccessToken = token;
                    db.SaveChanges();

                    response.AccessToken = token;
                    response.Status = STATUS.SUCCESS;

                    client.UserID = user_account.ID;
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
                response.Status = STATUS.FAILURE;
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        // Register
        private static RequestResult OnRegisterRequest(RegisterRequest request, ClientInfo client) {
            using var db = new PiDbContext();

            var user_account = db.Accounts.Where(p => p.Email == request.Email).SingleOrDefault();
            var response = new RegisterResponse();

            if (user_account == null || user_account.Verified == false) {
                response.Status = STATUS.SUCCESS;

                if(user_account != null) {
                    db.Accounts.Remove(user_account);
                }

                var code = TokenGenerator.Next().Substring(0, 6);
                var time = DateTime.Now.AddMinutes(15);

                var verification = new Verification() {
                    Email = request.Email,
                    Code = code,

                    ExpireDate = time
                };

                var account = new Database.Account() {
                    Username = request.Email.Substring(0, request.Email.IndexOf('@')),

                    Email = request.Email,
                    Password = request.Password,

                    Verified = false
                };

                db.Verifications.Add(verification);
                db.Accounts.Add(account);

                SMTP.SendMail(request.Email, "PI account verification", "Verification code: " + code);
                db.SaveChanges();
            }
            else {
                response.Status = STATUS.FAILURE;
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        private static RequestResult OnVerifyEmailRequest(VerifyEmailRequest request, ClientInfo client) {
            using var db = new PiDbContext();

            var user_account = db.Accounts.Where(p => p.Email == request.Email).SingleOrDefault();
            var verification = db.Verifications.Where(p => p.Email == request.Email).OrderByDescending(t => t.ExpireDate).First();

            var response = new VerifyEmailResponse();

            if(request.Code == verification.Code) {
                var time = (verification.ExpireDate - DateTime.Now).TotalSeconds;

                if (time > 0) {
                    response.Status = STATUS.SUCCESS;

                    user_account.Verified = true;
                    db.SaveChanges();
                }
                else {
                    // TODO: ErrorCode -> kod wygasł
                    response.Status = STATUS.FAILURE;
                }
            }
            else {
                // TODO: ErrorCode -> niepoprawny kod
                response.Status = STATUS.FAILURE;
            }

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }
    }

}