using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.VoiceChat.AcceptVoiceChat {

    [Serializable]
    public class AcceptVoiceChatRequest : Request {
        public int FriendID { get; set; }
        public byte[] Key { get; set; }
        public byte[] IV { get; set; }
    }

}