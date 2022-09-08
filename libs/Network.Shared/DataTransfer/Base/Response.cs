using System;
using System.Collections.Generic;
using Network.Shared.Core;

namespace Network.Shared.DataTransfer.Base {

    public enum ResponseResult {
        None = 0,
        Success, Failure
    }

    [Serializable]
    public abstract class Response {
        public ResponseResult Result { get; set; } = ResponseResult.None;
        public List<ErrorCode> Errors { get; set; } = new List<ErrorCode>();
    }

}