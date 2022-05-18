using System;
using System.Collections.Generic;

using System.IO;
using System.Threading.Tasks;

using Network.Shared.DataTransfer.Test.Client;
using Network.Shared.DataTransfer.Test.Server;

namespace Network.Server.DataProcessing.Test {

    internal class TestResult { 
        public string Filepath { get; set; }
        public string Data { get; set; }
    }

    internal static class TestRequestManager {
        public static void Dispatch(RequestDispatcher dispatcher, ClientInfo client) {
            // Server
            dispatcher.Dispatch<ServerTestPacketRequest>(OnServerTestPacketRequest, client);
            dispatcher.Dispatch<ServerTestSaveLogsRequest>(OnServerTestResultRequest, client);

            // Client
            dispatcher.Dispatch<ClientTestRequest>(OnClientTestRequest, client);
        }


        // Server
        private static RequestResult OnServerTestPacketRequest(ServerTestPacketRequest request, ClientInfo client) {
            var test = _Logger.Find(p => p.Filepath == request.Filepath);

            if (test == null) {
                test = new TestResult() {
                    Filepath = request.Filepath,
                    Data = ""
                };

                _Logger.Add(test);
            }

            test.Data += "[" + DateTime.Now + "] = " + request.ID + "\n";
            return null;
        }

        private static RequestResult OnServerTestResultRequest(ServerTestSaveLogsRequest request, ClientInfo client) {
            var test = _Logger.Find(p => p.Filepath == request.Filepath);
            File.WriteAllText(test.Filepath, test.Data);

            return null;
        }


        // Client
        private static RequestResult OnClientTestRequest(ClientTestRequest request, ClientInfo client) {
            // Send responses
            Task.Factory.StartNew(() => {
                var random = new Random();

                for (int i = 0; i < request.ResponseCount; i++) {
                    var data = new List<long>();
                    data.Capacity = random.Next(1, 64);

                    Server.SendResponse(client, new ClientTestResponse() {
                        ID = i,

                        LastResponse = ((i + 1) == request.ResponseCount),
                        UselessData = data
                    });
                }
            }, TaskCreationOptions.LongRunning);

            // Send notifications
            Task.Factory.StartNew(() => {
                var random = new Random();

                for (int i = 0; i < request.NotificationCount; i++) {
                    var data = new List<long>();
                    data.Capacity = random.Next(1, 64);

                    Server.SendNotification(client, new ClientTestNotification() {
                        ID = i,

                        LastNotification = ((i + 1) == request.NotificationCount),
                        UselessData = data
                    });
                }
            }, TaskCreationOptions.LongRunning);

            return null;
        }


        private static List<TestResult> _Logger = new List<TestResult>();
    }

}