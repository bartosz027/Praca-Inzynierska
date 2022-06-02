using Network;
using Network.Server;

namespace ServerApp {

    class Program {
        static void Main(string[] args) {
            // TODO: Load data from "config.ini" file
            Server.Instance.Start("0.0.0.0", 65535);
        }
    }

}