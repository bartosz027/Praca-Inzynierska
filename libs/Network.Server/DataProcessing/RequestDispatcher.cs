using System;
using Network.Shared.Model;
using Network.Shared.DataTransfer.Base;

namespace Network.Server.DataProcessing {

    internal class RequestDispatcher {
        public RequestDispatcher(Request request) {
            Request = request;
            Result = null;
        }

        public void Dispatch<T>(Func<T, ClientInfo, RequestResult> function, ClientInfo client) where T : Request {
            if (Request.GetType() == typeof(T)) {
                Result = function((T)Request, client);
            }
        }

        public Request Request { get; private set; }
        public RequestResult Result { get; private set; }
    }

}