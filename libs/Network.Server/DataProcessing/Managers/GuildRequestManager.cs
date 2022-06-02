namespace Network.Server.DataProcessing.Managers {

    internal static class GuildRequestManager {
        public static void Dispatch(RequestDispatcher dispatcher, ClientInfo client) {
            if (dispatcher.Request.AccessToken == client.AccessToken) {
                // TODO: Implement this
            }
        }
    }

}