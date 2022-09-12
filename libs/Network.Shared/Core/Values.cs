namespace Network.Shared.Core {

    public static class Values {
        // Email
        public const string EmailSubject = "PI account verification";
        public const string EmailBody = "Verification code:";

        // Username
        public const int MinUsernameLength = 2;
        public const int MaxUsernameLength = 32;

        // Password
        public const int MinPasswordLength = 8;
        public const int MaxPasswordLength = 128;
    }

}