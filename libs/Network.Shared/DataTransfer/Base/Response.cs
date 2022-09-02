using System;
using Network.Shared.Core;

namespace Network.Shared.DataTransfer.Base {

    public enum Result {
        None = 0,
        Success, Failure
    }

    [Serializable]
    public abstract class Response {
        public Result Result { get; set; }
        public ErrorCode ErrorCode { get; set; }
    }

}