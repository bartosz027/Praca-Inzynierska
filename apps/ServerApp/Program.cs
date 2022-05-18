using Network;
using Network.Server;

namespace ServerApp {

    class Program {
        static void Main(string[] args) {
            Server.Instance.Start("0.0.0.0", 65535);
        }
    }

}