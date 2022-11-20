using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.ManageMessages.DownloadImage {

    [Serializable]
    public class DownloadImageRequest : Request {
        public int FriendID { get; set; }
        public int MessageID { get; set; }
        public string Filename { get; set; }
    }

}