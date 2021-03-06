using System;
using System.Collections.Generic;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Database.Friends {

    [Serializable]
    public class MessageInfo { 
        public int ID { get; set; }
        public int SenderID { get; set; }
        public string Content { get; set; }
        public DateTime SendDate { get; set; }
    }

    [Serializable]
    public class MessageHistoryResponse : Response {
        public int FriendID { get; set; }
        public List<MessageInfo> Messages { get; set; }
    }

}