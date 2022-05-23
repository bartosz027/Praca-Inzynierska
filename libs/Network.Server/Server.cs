using System;

using System.Collections.Concurrent;
using System.Collections.Generic;

using System.Net;
using System.Net.Sockets;

using System.Threading;
using System.Threading.Tasks;

using Network.Server.Core;
using Network.Server.DataProcessing;

using Network.Shared.Core;
using Network.Shared.DataTransfer.Base;

namespace Network.Server {

    internal class ClientInfo {
        // User data
        public int UserID { get; set; }
        public string Username { get; set; }

        // Connection data
        public TcpClient TCP { get; set; }
        public NetworkStream Stream { get; set; }

        // Authentication
        public string AccessToken { get; set; }
    }

    public class Server {
        private Server() {
            // TODO: Move this to config file
            SMTP.Email = "noreplyteampi@gmail.com";
            SMTP.Password = "Qwerty123.";

            Console.SetOut(new TimestampTextWriter());
        }

        public static Server Instance {
            get { return _Instance ??= new Server(); }
            private set { _Instance = value; }
        }
        private static Server _Instance;


        public void Start(string ip_address, int port) {
            // Init server
            var ip = IPAddress.Parse(ip_address);

            Server.Data.Listener = new TcpListener(ip, port);
            Server.Data.Clients = new ThreadSafeList<ClientInfo>();

            // Start server
            Server.Data.Listener.Start();

            Console.WriteLine("Server has started on {0}!", Server.Data.Listener.LocalEndpoint);
            Console.WriteLine("Waiting for connection...");

            while (true) {
                var client = Server.Data.Listener.AcceptTcpClient();
                var client_info = new ClientInfo();

                client_info.TCP = client;
                client_info.Stream = client.GetStream();

                Console.WriteLine("Connection established with {0}!", client_info.TCP.Client.RemoteEndPoint);
                StartClientThread(client_info);
            }
        }

        private void StartClientThread(ClientInfo client) {
            var request_queue = new BlockingCollection<Request>();
            var cancellation_token = new CancellationTokenSource();

            // Receive request
            Task.Factory.StartNew(() => {
                byte[] request_buffer = new byte[256 * 1024]; // 256 KB cache
                int buffer_length = 0;

                while (true) {
                    try {
                        byte[] request_bytes = new byte[client.TCP.Available];
                        client.Stream.Read(request_bytes, 0, request_bytes.Length);

                        Array.Copy(request_bytes, 0, request_buffer, buffer_length, request_bytes.Length);
                        buffer_length += request_bytes.Length;

                        for (int index = 0, length = 0; index <= buffer_length; index += (length + 4)) {
                            length = BitConverter.ToInt32(request_buffer, index);

                            if(index == buffer_length) {
                                buffer_length = 0;
                                break;
                            }

                            if (index + (length + 4) > buffer_length) {
                                var new_length = buffer_length - index;

                                Array.Copy(request_buffer, index, request_buffer, 0, new_length);
                                buffer_length = new_length;

                                break;
                            }

                            var data = new byte[length];
                            Array.Copy(request_buffer, index + 4, data, 0, length);

                            var request = Serializer.Deserialize(data) as Request;
                            request_queue.Add(request);
                        }
                    }
                    catch (Exception e) {
                        if(client.TCP.Connected == false) {
                            var client_info = Server.Data.Clients.Find(p => (p.TCP == client.TCP));

                            if (client_info != null) {
                                Console.WriteLine("Client [id={0}, username={1}] disconnected!", client_info.UserID, client_info.Username);
                                Server.Data.Clients.Remove(client_info);
                            }

                            Console.WriteLine("Connection lost with {0}!", client.TCP.Client.RemoteEndPoint);
                            cancellation_token.Cancel();

                            break;
                        }

                        Console.WriteLine(e);
                    }
                }
            }, TaskCreationOptions.LongRunning);

            // Process request
            Task.Factory.StartNew(() => {
                while (true) {
                    try {
                        // Dispatch request
                        var request = request_queue.Take(cancellation_token.Token);
                        var result = RequestManager.Dispatch(request, client);

                        if(result != null) {
                            // Send response
                            if (result.ResponseReceiver != null && result.ResponseData != null) {
                                Server.SendResponse(result.ResponseReceiver, result.ResponseData);
                            }

                            // Send notifications
                            if (result.NotificationReceivers != null && result.NotificationData != null) {
                                Server.BroadcastNotification(result.NotificationReceivers, result.NotificationData);
                            }
                        }
                    }
                    catch (Exception e) {
                        if (e is OperationCanceledException) {
                            break;
                        }

                        Console.WriteLine(e);
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }


        internal static void SendResponse(ClientInfo client, Response response) {
            try {
                byte[] response_bytes = Serializer.Serialize(response);
                client.Stream.Write(response_bytes, 0, response_bytes.Length);
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        internal static void SendNotification(ClientInfo client, Notification notification) {
            try {
                byte[] notification_bytes = Serializer.Serialize(notification);
                client.Stream.Write(notification_bytes, 0, notification_bytes.Length);
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }


        internal static void BroadcastNotification(List<ClientInfo> receivers, Notification notification) {
            foreach (var receiver in receivers) {
                Server.SendNotification(receiver, notification);
            }
        }


        internal static class Data {
            public static TcpListener Listener { get; set; }
            public static ThreadSafeList<ClientInfo> Clients { get; set; }
        }
    }

}