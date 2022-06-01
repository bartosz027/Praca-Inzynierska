using System;
using System.Diagnostics;

namespace Startup {

    class Program {
        static void Main(string[] args) {
            // Settings
            var client_count = 2;

            // Init processes
            var server_process = Process.Start("ServerApp.exe");
            var client_processes = new Process[client_count];

            // Start processes
            for(int i = 0; i < client_count; i++) {
                client_processes[i] = Process.Start("ClientApp.exe");
            }

            // Wait for server process to exit
            server_process.WaitForExit();

            // Exit all client processes
            foreach (var process in client_processes) {
                if (process != null && process.HasExited == false) {
                    process.Kill();
                }
            }
        }
    }

}