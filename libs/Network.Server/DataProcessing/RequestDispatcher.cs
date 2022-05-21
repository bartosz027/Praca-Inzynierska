using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Server.DataProcessing {

    internal class RequestDispatcher {
        public RequestDispatcher(Request request) {
            _Result = new RequestResult();
            _Request = request;
        }

        public void Dispatch<T>(Func<T, ClientInfo, RequestResult> function, ClientInfo client) where T : Request {
            if (_Request.GetType() == typeof(T)) {
                _Result = function((T)_Request, client);
            }
        }

        public RequestResult Result {
            get { 
                return _Result; 
            }
            set {
                _Result = value; 
            }
        }

        private RequestResult _Result;
        private readonly Request _Request;
    }

}