using System;
using Network.Shared.DataTransfer.Base;

namespace Network.Shared.DataTransfer.Model.Friends.VoiceChat.StartVoiceChat {

    [Serializable]
    public class StartVoiceChatNotification : Notification {
        public int FriendID { get; set; }
    }

}