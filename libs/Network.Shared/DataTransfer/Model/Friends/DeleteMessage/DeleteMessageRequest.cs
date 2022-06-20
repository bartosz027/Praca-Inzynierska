﻿using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.DeleteMessage {

    [Serializable]
    public class DeleteMessageRequest : Request {
        public int FriendID { get; set; }
        public int MessageID { get; set; }
    }

}