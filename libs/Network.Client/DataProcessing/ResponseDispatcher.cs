using System;
using Network.Shared.DataTransfer.Interface;

namespace Network.Client.DataProcessing {

    public class ResponseDispatcher {
        public ResponseDispatcher(IResponse response) {
            _Response = response;
        }

        public void Dispatch<T>(Action<T> function) where T : IResponse {
            if (_Response.GetType() == typeof(T)) {
                function((T)_Response);
            }
        }

        private readonly IResponse _Response;
    }

}