namespace Network.Shared.DataTransfer.Base {

    [System.Serializable]
    public abstract class Request {
        internal string AccessToken { get; set; }
    }

}