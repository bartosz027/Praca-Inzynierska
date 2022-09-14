using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Database.Friends.SetMessageRead {

    [Serializable]
    public class SetMessageReadRequest : Request {
        public int FriendID { get; set; }
    }

}