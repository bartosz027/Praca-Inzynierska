using System.Net.Sockets;
using System.Security.Cryptography;
using Network.Shared.Core;

namespace Network.Shared.Model {

    public class ClientInfo {
        // User data
        public int ID { get; set; }
        public bool Status { get; set; }
        public string Username { get; set; }
        public string AccessToken { get; set; }

        // Connection data
        public TcpClient TCP { get; internal set; }
        internal NetworkStream Stream { get; set; }

        // Security
        public bool IsConnectedViaRSA { get; internal set; }
        internal RSAParameters RSA { get; set; }

        public bool IsConnectedViaAES { get; internal set; }
        internal EncryptionAES AES { get; set; }
    }

}