using Network.Server;
using Network.Shared.Core;

namespace ServerApp {

    class Program {
        static void Main(string[] args) {
            var ip = ConfigManager.GetValue("Server_IP");
            var port = int.Parse(ConfigManager.GetValue("Server_PORT"));

            Server.Instance.Start(ip, port);
        }
    }

}