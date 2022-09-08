namespace Network.Shared.Core {

    public enum ErrorCode {
        AccountNotFound,

        InvalidUsername,
        InvalidPassword,

        EmailAddressTaken,
        InvalidEmailAddress,

        InvalidVerificationCode,
        ExpiredVerificationCode,

        InvitationAlreadyFriends,
        InvitationSelfInvite,
        InvitationDuplicate
    }

}