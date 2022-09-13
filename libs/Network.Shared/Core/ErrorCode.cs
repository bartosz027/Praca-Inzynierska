namespace Network.Shared.Core {

    public enum ErrorCode {
        AccountNotFound,
        AccountNotVerified,

        EmailAddressTaken,
        InvalidEmailOrPassword,

        InvalidVerificationCode,
        ExpiredVerificationCode,

        InvitationAlreadyFriends,
        InvitationSelfInvite,
        InvitationDuplicate
    }

}