using Network.Shared.Core;

using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace Network.Client.Model {

    public class ClientInfo {
        // User data
        public int ID { get; set; }
        public bool Status { get; set; }

        public string Email { get; set; }
        public string Username { get; set; }
        public string AccessToken { get; set; }

        public IPEndPoint ClientEndPoint { get; set; }

        // TCP
        public TcpClient TCP { get; internal set; }
        internal NetworkStream Stream { get; set; }

        // UDP
        public UdpClient UDP { get; internal set; }
        public IPEndPoint ServerEndPoint { get; internal set; }

        // Security -> RSA
        public bool IsConnectedViaRSA { get; internal set; }
        internal RSAParameters RSA { get; set; }

        // Security -> AES
        public bool IsConnectedViaAES { get; internal set; }
        internal EncryptionAES AES { get; set; }

        // Methods
        public void ClearUserData() {
            ID = 0;
            Email = null;
            Status = false;
            Username = null;
            AccessToken = null;
            ClientEndPoint = null;
        }
    }

}