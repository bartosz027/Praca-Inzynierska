using System;
using System.Collections.Generic;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Database.Friends {

    [Serializable]
    public class FriendInfo {
        public int UserID { get; set; }
        public string Username { get; set; }
    }

    [Serializable]
    public class FriendsListResponse : Response {
        public List<FriendInfo> FriendsList { get; set; }
    }

}