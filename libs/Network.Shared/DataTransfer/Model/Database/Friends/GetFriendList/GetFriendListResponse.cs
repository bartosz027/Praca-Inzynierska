using System;
using System.Collections.Generic;

using Network.Shared.DataTransfer.Base;
using Network.Shared.DataTransfer.Model.Database.Friends.GetMessageHistory;

namespace Network.Shared.DataTransfer.Model.Database.Friends.GetFriendList {

    [Serializable]
    public class FriendInfo {
        public int UserID { get; set; }
        public bool Status { get; set; }
        public string Username { get; set; }

        public bool IsLastMessageRead { get; set; }
        public DateTime LastMessageSendDate { get; set; }
    }

    [Serializable]
    public class GetFriendListResponse : Response {
        public List<FriendInfo> FriendList { get; set; }
    }

}