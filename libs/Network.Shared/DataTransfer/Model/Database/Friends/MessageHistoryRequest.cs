﻿using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Database.Friends {

    [Serializable]
    public class MessageHistoryRequest : Request {
        public int FriendID { get; set; }
    }

}