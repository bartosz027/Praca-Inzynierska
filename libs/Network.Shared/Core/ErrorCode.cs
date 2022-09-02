namespace Network.Shared.Core {

    public enum ErrorCode {
        None = 0,

        InvalidVerificationCode,
        ExpiredVerificationCode,

        FriendInvitationAlreadyFriends,
        FriendInvitationUserNotExist,
        FriendInvitationSelfInvite,
        FriendInvitationDuplicate
    }

}