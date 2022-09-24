using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.VoiceChat.StartVoiceChat {

    [Serializable]
    public class StartVoiceChatRequest : Request {
        public int FriendID { get; set; }
    }

}