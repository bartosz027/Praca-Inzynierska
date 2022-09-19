using Network.Shared.Core;
using Network.Shared.Model;

using Network.Shared.DataTransfer.Base;
using Network.Shared.DataTransfer.Security.ExchangeAESKeys;
using Network.Shared.DataTransfer.Security.ExchangeRSAKeys;

namespace Network.Server.DataProcessing.Managers {

    internal static class SecurityRequestManager {
        public static void Dispatch(RequestDispatcher dispatcher, ClientInfo client) {
            dispatcher.Dispatch<ExchangeRSAKeysRequest>(OnExchangeRSAKeysRequest, client);
            dispatcher.Dispatch<ExchangeAESKeysRequest>(OnExchangeAESKeysRequest, client);
        }

        private static RequestResult OnExchangeRSAKeysRequest(ExchangeRSAKeysRequest request, ClientInfo client) {
            var response = new ExchangeRSAKeysResponse() {
                Key = EncryptionRSA.PublicKey,
                Result = ResponseResult.Success
            };

            client.IsConnectedViaRSA = true;
            client.RSA = request.Key;

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }

        private static RequestResult OnExchangeAESKeysRequest(ExchangeAESKeysRequest request, ClientInfo client) {
            var response = new ExchangeAESKeysResponse();
            response.Result = ResponseResult.Success;

            client.IsConnectedViaAES = true;
            client.AES = new EncryptionAES(request.Key, request.IV);

            return new RequestResult() {
                ResponseReceiver = client,
                ResponseData = response
            };
        }
    }

}