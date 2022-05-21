using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Client.DataProcessing {

    public class ResponseDispatcher {
        public ResponseDispatcher(Response response) {
            _Response = response;
        }

        public void Dispatch<T>(Action<T> function) where T : Response {
            if (_Response.GetType() == typeof(T)) {
                function((T)_Response);
            }
        }

        private readonly Response _Response;
    }

}