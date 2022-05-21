namespace Network.Shared.DataTransfer.Base {

    public enum STATUS {
        NONE = 0, 
        SUCCESS, FAILURE
    }

    [System.Serializable]
    public abstract class Response {
        public STATUS Status { get; set; } = STATUS.NONE;
    }

}