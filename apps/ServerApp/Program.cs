using Network.Server;
using Network.Shared.Core;

namespace ServerApp {

    class Program {
        static void Main(string[] args) {
            string ip = ConfigManager.GetValue("Server_IP"), port = ConfigManager.GetValue("Server_PORT");
            Server.Instance.Start(ip, int.Parse(port));
        }
    }

}