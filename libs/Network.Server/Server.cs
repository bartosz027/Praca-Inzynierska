using System;
using System.Text;

using System.Collections.Concurrent;
using System.Collections.Generic;

using System.Net;
using System.Net.Sockets;

using System.Threading;
using System.Threading.Tasks;

using Network.Server.Core;
using Network.Server.Database;
using Network.Server.DataProcessing;
using Network.Server.Model;

using Network.Shared.Core;
using Network.Shared.DataTransfer.Base;
using Network.Shared.DataTransfer.Model.Account.Logout;

using Network.Shared.DataTransfer.Security.ExchangeAESKeys;
using Network.Shared.DataTransfer.Security.ExchangeRSAKeys;

namespace Network.Server {

    public class Server {
        private Server() {
            Console.SetOut(new TimestampTextWriter());

            var email = ConfigManager.GetValue("SMTP_Email");
            var password = ConfigManager.GetValue("SMTP_Password");

            SMTP.Init(email, password);
            EncryptionRSA.Init();
        }

        // Singleton
        public static Server Instance {
            get { return _Instance ??= new Server(); }
            private set { _Instance = value; }
        }
        private static Server _Instance;

        // Methods
        public void Start(string ip_address, int port) {
            // Init server
            var ip = IPAddress.Parse(ip_address);
            Server.Data.Clients = new ThreadSafeList<ClientInfo>();

            // Start server
            Server.Data.Listener = new TcpListener(ip, port);
            Server.Data.Listener.Start();

            Server.Data.UDP = new UdpClient(new IPEndPoint(ip, port));
            StartUdpThread(ip, port);

            Console.WriteLine("Server has started on {0}!", Server.Data.Listener.LocalEndpoint);
            Console.WriteLine("Waiting for connection...");

            while (true) {
                var client = Server.Data.Listener.AcceptTcpClient();
                var client_info = new ClientInfo();

                client_info.TCP = client;
                client_info.Stream = client.GetStream();
                client_info.InternalEndPoint = client_info.TCP.Client.LocalEndPoint as IPEndPoint;

                Console.WriteLine("Connection established with {0}!", client_info.TCP.Client.RemoteEndPoint);
                StartTcpThread(client_info);
            }
        }

        private void StartUdpThread(IPAddress ip, int port) {
            Task.Factory.StartNew(() => {
                while (true) {
                    try {
                        byte[] data = null; 
                        IPEndPoint ep = null;
                        data = Server.Data.UDP.Receive(ref ep);

                        if (data != null) {
                            var id = 0;
                            var result = int.TryParse(Encoding.UTF8.GetString(data), out id);

                            if (result) {
                                var client = Server.Data.Clients.Find(p => p.ID == id);
                                var ep_address = ep.Address.ToString();

                                var external_ip_string = new WebClient().DownloadString("http://icanhazip.com").Replace("\\r\\n", "").Replace("\\n", "").Trim();
                                var external_ip = IPAddress.Parse(external_ip_string);

                                client.ExternalEndPoint = ep_address.StartsWith("192.168") ? new IPEndPoint(external_ip, ep.Port) : ep;
                                client.InternalEndPoint.Port = ep.Port;

                                data = Encoding.UTF8.GetBytes("NULL");
                                Server.Data.UDP.Send(data, data.Length, ep);
                            }
                        }
                    }
                    catch { }
                }
            }, TaskCreationOptions.LongRunning);
        }

        private void StartTcpThread(ClientInfo client) {
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

                        for (int index = 0, length = 0; index <= buffer_length; index += length + 4) {
                            length = BitConverter.ToInt32(request_buffer, index);

                            if(index == buffer_length) {
                                buffer_length = 0;
                                break;
                            }

                            if (index + length + 4 > buffer_length) {
                                var new_length = buffer_length - index;

                                Array.Copy(request_buffer, index, request_buffer, 0, new_length);
                                buffer_length = new_length;

                                break;
                            }

                            var data = new byte[length];
                            Array.Copy(request_buffer, index + 4, data, 0, length);

                            if(client.IsConnectedViaAES) {
                                data = client.AES.Decrypt(data);
                            } 
                            else if (client.IsConnectedViaRSA) {
                                data = EncryptionRSA.Decrypt(data);
                            }

                            data = CompressionLZ4.Decode(data);
                            var request = Serializer.Deserialize(data) as Request;

                            request_queue.Add(request);
                        }
                    }
                    catch (Exception e) {
                        if(client.TCP.Connected == false) {
                            var client_info = Server.Data.Clients.Find(p => p.TCP == client.TCP);

                            if (client_info != null) {
                                Console.WriteLine("Client [id={0}, username={1}] disconnected!", client_info.ID, client_info.Username);

                                var notification = new LogoutNotification() {
                                    ID = client.ID,
                                    EndPoint = client.ExternalEndPoint
                                };

                                using (var db = new PiDbContext()) {
                                    var receivers = new List<ClientInfo>();
                                    var user_account = db.Accounts.Find(client.ID);

                                    foreach (var friendship in user_account.Friends) {
                                        var friend_info = Server.Data.Clients.Find(p => p.ID == friendship.FriendID);

                                        if (friend_info != null) {
                                            receivers.Add(friend_info);
                                        }
                                    }

                                    BroadcastNotification(receivers, notification);
                                }

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
                response_bytes = CompressionLZ4.Encode(response_bytes);

                if ((response is ExchangeAESKeysResponse) == false && client.IsConnectedViaAES) {
                    response_bytes = client.AES.Encrypt(response_bytes);
                }
                else if ((response is ExchangeRSAKeysResponse) == false && client.IsConnectedViaRSA) {
                    response_bytes = EncryptionRSA.Encrypt(response_bytes, client.RSA);
                }

                response_bytes = Serializer.AddArrayLength(response_bytes);
                client.Stream.Write(response_bytes, 0, response_bytes.Length);
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
        }

        internal static void SendNotification(ClientInfo client, Notification notification) {
            try {
                byte[] notification_bytes = Serializer.Serialize(notification);
                notification_bytes = CompressionLZ4.Encode(notification_bytes);

                if (client.IsConnectedViaAES) {
                    notification_bytes = client.AES.Encrypt(notification_bytes);
                }

                notification_bytes = Serializer.AddArrayLength(notification_bytes);
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

        // Properties
        internal static class Data {
            public static UdpClient UDP { get; set; }
            public static TcpListener Listener { get; set; }
            public static ThreadSafeList<ClientInfo> Clients { get; set; }
        }
    }

}