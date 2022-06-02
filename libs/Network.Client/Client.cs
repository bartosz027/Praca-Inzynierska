using System;

using System.Collections.Concurrent;
using System.Net.Sockets;

using System.Threading;
using System.Threading.Tasks;

using Network.Shared.Core;
using Network.Shared.DataTransfer.Base;

namespace Network.Client {

    public class Client {
        private Client() {
            
        }

        public static Client Instance {
            get { return _Instance ??= new Client(); }
            private set { _Instance = value; }
        }
        private static Client _Instance;


        public void Connect(string ip, int port) {
            Client.Data.TCP = new TcpClient();

            Client.Data.TCP.Connect(ip, port);
            Client.Data.Stream = Client.Data.TCP.GetStream();

            if (Client.Data.TCP.Connected) {
                // TODO: Write to log file
                StartListenerThread();
            }
        }

        private void StartListenerThread() {
            var received_data_queue = new BlockingCollection<object>();
            var cancellation = new CancellationTokenSource();

            // Receive data
            Task.Factory.StartNew(() => {
                byte[] receive_buffer = new byte[256 * 1024]; // 256 KB cache
                int buffer_length = 0;

                while (true) {
                    try {
                        byte[] received_bytes = new byte[Client.Data.TCP.Available];
                        Client.Data.Stream.Read(received_bytes, 0, received_bytes.Length);

                        Array.Copy(received_bytes, 0, receive_buffer, buffer_length, received_bytes.Length);
                        buffer_length += received_bytes.Length;

                        for (int index = 0, length = 0; index <= buffer_length; index += (length + 4)) {
                            length = BitConverter.ToInt32(receive_buffer, index);

                            if (index == buffer_length) {
                                buffer_length = 0;
                                break;
                            }

                            if (index + (length + 4) > buffer_length) {
                                var new_length = buffer_length - index;

                                Array.Copy(receive_buffer, index, receive_buffer, 0, new_length);
                                buffer_length = new_length;

                                break;
                            }

                            var data = new byte[length];
                            Array.Copy(receive_buffer, index + 4, data, 0, length);

                            var obj = Serializer.Deserialize(data);
                            received_data_queue.Add(obj);
                        }
                    }
                    catch (Exception e) {
                        if(Client.Data.TCP.Connected == false) {
                            // TODO: Write to log file

                            cancellation.Cancel();
                            break;
                        }

                        // TODO: Write to log file
                    }
                }
            }, TaskCreationOptions.LongRunning);

            // Process data
            Task.Factory.StartNew(() => {
                while (true) {
                    try {
                        var received_data = received_data_queue.Take(cancellation.Token);

                        switch (received_data) {
                            case Response: {
                                ResponseReceived?.Invoke(this, (Response)received_data);
                                break;
                            }
                            case Notification: {
                                NotificationReceived?.Invoke(this, (Notification)received_data);
                                break;
                            }
                        }
                    }
                    catch (Exception e) {
                        if(e is OperationCanceledException) {
                            break;
                        }

                        // TODO: Write to log file
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }


        public void SendRequest(Request request) {
            request.AccessToken = Client.Data.AccessToken;

            try {
                byte[] request_bytes = Serializer.Serialize(request);
                Client.Data.Stream.Write(request_bytes, 0, request_bytes.Length);
            }
            catch (Exception e) {
                // TODO: Write to log file
            }
        }


        public static class Data {
            // User data
            public static string Username { get; set; }
            public static string AccessToken { get; set; }

            // Connection data
            internal static TcpClient TCP { get; set; }
            internal static NetworkStream Stream { get; set; }
        }

        
        // Events
        public event EventHandler<Response> ResponseReceived;
        public event EventHandler<Notification> NotificationReceived;
    }

}