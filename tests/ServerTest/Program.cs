using System;
using System.Collections.Generic;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Network.Client;
using Network.Shared.DataTransfer.Test.Server;

namespace ServerTest {

    class Program {
        static void Main(string[] args) {
            // Settings
            var thread_count = 2;
            var request_count = 32768;

            // Connect to server
            Client.Instance.Connect("127.0.0.1", 65535);
            Thread.Sleep(250);

            // Delete previous results
            var filepath = args[0];
            File.Delete(filepath);

            // Start testing
            var tasks = new List<Task>(thread_count);

            for (int i = 0; i < thread_count; i++) {
                int start = (request_count / thread_count) * i;
                int end = (request_count / thread_count) * (i + 1);

                var task = Task.Factory.StartNew(() => Test(start, end, filepath), TaskCreationOptions.LongRunning);
                tasks.Add(task);
            }

            foreach (var task in tasks) {
                task.Wait();
            }

            Client.Instance.SendRequest(new ServerTestSaveLogsRequest() { 
                Filepath = filepath
            });

            // Wait for results
            while(!File.Exists(filepath)) {
                Thread.Sleep(500);
            }
        }

        static void Test(int start, int end, string filepath) {
            var random = new Random();

            for (int i = start; i < end; i++) {
                var data = new List<long>();
                data.Capacity = random.Next(1, 64);

                Client.Instance.SendRequest(new ServerTestPacketRequest() {
                    ID = i,

                    Filepath = filepath,
                    UselessData = data
                });
            }
        }
    }

}