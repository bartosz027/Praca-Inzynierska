using System;
using System.Linq;

using Network.Server.Database;

using Network.Shared.DataTransfer.Base;
using Network.Shared.DataTransfer.Model.Account.Login;

namespace Network.Server.DataProcessing.Account {

    internal static class AccountRequestManager {
        public static void Dispatch(RequestDispatcher dispatcher, ClientInfo client) {
            dispatcher.Dispatch<LoginRequest>(OnLoginRequest, client);
        }

        private static RequestResult OnLoginRequest(LoginRequest request, ClientInfo client) {
            using var db = new PiDbContext(); 
            
            var user_account = db.Accounts.Where(p => p.Email == request.Email).First();
            var response = new LoginResponse();

            if (user_account != null && user_account.Password == request.Password) {
                var client_info = Server.Data.Clients.Find(p => p.UserID == user_account.ID);

                if (client_info == null) {
                    client.UserID = user_account.ID;
                    client.Username = user_account.Username;
                    client.AccessToken = user_account.AccessToken;

                    response.AccessToken = user_account.AccessToken;
                    response.Status = STATUS.SUCCESS;

                    Server.Data.Clients.Add(client);
                }
                else {
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
    }

}