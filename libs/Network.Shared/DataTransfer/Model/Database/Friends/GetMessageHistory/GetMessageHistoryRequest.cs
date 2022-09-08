using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Database.Friends.GetMessageHistory {

    [Serializable]
    public class GetMessageHistoryRequest : Request {
        public int FriendID { get; set; }
    }

}