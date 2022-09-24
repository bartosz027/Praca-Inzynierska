using System;
using System.Net;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.VoiceChat.AcceptVoiceChat {

    [Serializable]
    public class AcceptVoiceChatResponse : Response {
        public int FriendID { get; set; }
        public IPEndPoint EndPoint { get; set; }
    }

}