using Network.Shared.Core;

using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace Network.Server.Model {

    internal class ClientInfo {
        // User data
        public int ID { get; set; }
        public bool Status { get; set; }
        public string Username { get; set; }
        public string AccessToken { get; set; }

        // TCP
        public TcpClient TCP { get; set; }
        public NetworkStream Stream { get; set; }

        // UDP
        public IPEndPoint InternalEndPoint { get; set; }
        public IPEndPoint ExternalEndPoint { get; set; }

        // Security -> RSA
        public bool IsConnectedViaRSA { get; set; }
        public RSAParameters RSA { get; set; }

        // Security -> AES
        public bool IsConnectedViaAES { get; set; }
        public EncryptionAES AES { get; set; }
    }

}