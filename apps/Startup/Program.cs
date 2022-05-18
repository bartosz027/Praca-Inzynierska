using System;
using System.Diagnostics;

namespace Startup {

    enum STARTUP {
        MAIN_APPLICATION = 0,
        CLIENT_TEST, SERVER_TEST, ALL_TESTS
    }

    class Program {
        static void Main(string[] args) {
            // Settings
            var app_type = STARTUP.MAIN_APPLICATION;
            var client_count = 2;

            // Init processes
            var server_process = Process.Start("ServerApp.exe");
            var client_processes = new Process[client_count * 2];

            // Start processes
            for(int i = 0; i < client_count; i++) {
                switch (app_type) {
                    case STARTUP.MAIN_APPLICATION: {
                        client_processes[i] = RunClientApplication();
                        break;
                    }
                    case STARTUP.CLIENT_TEST: {
                        client_processes[i] = RunClientTest(i + 1);
                        break;
                    }
                    case STARTUP.SERVER_TEST: {
                        client_processes[i] = RunServerTest(i + 1);
                        break;
                    }
                    case STARTUP.ALL_TESTS: {
                        client_processes[(i * 2) + 0] = RunClientTest(i + 1);
                        client_processes[(i * 2) + 1] = RunServerTest(i + 1);
                        break;
                    }
                }
            }

            // Wait for server process
            server_process.WaitForExit();

            // Kill all client processes
            foreach (var process in client_processes) {
                if (process != null && process.HasExited == false) {
                    process.Kill();
                }
            }
        }

        // Apps
        static Process RunClientApplication() {
            return Process.Start("ClientApp.exe");
        }

        static Process RunClientTest(int id) {
            return Process.Start("ClientTest.exe", "ClientLog" + id + ".txt");
        }

        static Process RunServerTest(int id) {
            return Process.Start("ServerTest.exe", "ServerLog" + id + ".txt");
        }
    }

}