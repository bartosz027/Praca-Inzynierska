using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.VoiceChat.DisconnectVoiceChat {

    [Serializable]
    public class DisconnectVoiceChatRequest : Request {
        public int FriendID { get; set; }
    }

}