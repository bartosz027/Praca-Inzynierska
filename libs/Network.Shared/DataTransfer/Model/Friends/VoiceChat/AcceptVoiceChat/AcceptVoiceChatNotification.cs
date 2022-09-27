using System;
using System.Net;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.VoiceChat.AcceptVoiceChat {

    [Serializable]
    public class AcceptVoiceChatNotification : Notification {
        public int FriendID { get; set; }
        public IPEndPoint EndPoint { get; set; }

        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
    }

}