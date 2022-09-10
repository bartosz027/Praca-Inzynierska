namespace Network.Shared.Core {

    public enum ErrorCode {
        AccountNotFound,

        AccountNotVerified,
        InvalidEmailOrPassword,

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