using System;
using System.Collections.Concurrent;

using System.Net.Sockets;
using System.Security.Cryptography;

using System.Threading;
using System.Threading.Tasks;

using Network.Client.DataProcessing;

using Network.Shared.Core;
using Network.Shared.DataTransfer.Base;

using Network.Shared.DataTransfer.Security.ExchangeAESKeys;
using Network.Shared.DataTransfer.Security.ExchangeRSAKeys;

namespace Network.Client {

    public class Client {
        private Client() {
            EncryptionRSA.Init();
        }

        public static Client Instance {
            get { return _Instance ??= new Client(); }
            private set { _Instance = value; }
        }
        private static Client _Instance;

        // Connect to server
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

                            if (index + length + 4 > buffer_length) {
                                var new_length = buffer_length - index;

                                Array.Copy(receive_buffer, index, receive_buffer, 0, new_length);
                                buffer_length = new_length;

                                break;
                            }

                            var data = new byte[length];
                            Array.Copy(receive_buffer, index + 4, data, 0, length);

                            if (Client.Data.IsConnectedViaAES) {
                                data = Client.Data.AES.Decrypt(data);
                            }
                            else if (Client.Data.IsConnectedViaRSA) {
                                data = EncryptionRSA.Decrypt(data);
                            }

                            data = CompressionLZ4.Decode(data);
                            var obj = Serializer.Deserialize(data);

                            received_data_queue.Add(obj);
                        }
                    }
                    catch {
                        if(Client.Data.TCP.Connected == false) {
                            // TODO: Write to log file
                            cancellation.Cancel();
                            break;
                        }
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
                        // TODO: Write to log file

                        if(e is OperationCanceledException) {
                            break;
                        }
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        // Exchange RSA + AES keys
        public void EnableSecureConnection() {
            Client.Instance.ResponseReceived += OnExchangeKeysResponse;
            Client.Instance.SendRequest(new ExchangeRSAKeysRequest() { Key = EncryptionRSA.PublicKey });
        }

        private void OnExchangeKeysResponse(object sender, Response response) {
            var dispatcher = new ResponseDispatcher(response);

            dispatcher.Dispatch<ExchangeRSAKeysResponse>((response) => {
                Client.Data.AES = new EncryptionAES();

                Client.Data.IsConnectedViaRSA = true;
                Client.Data.RSA = response.Key;

                Client.Instance.SendRequest(new ExchangeAESKeysRequest() { 
                    Key = Client.Data.AES.GetKey(), 
                    IV = Client.Data.AES.GetIV() 
                });
            });

            dispatcher.Dispatch<ExchangeAESKeysResponse>((response) => {
                Client.Data.IsConnectedViaAES = true;
            });

            if (Client.Data.IsConnectedViaRSA && Client.Data.IsConnectedViaAES) {
                Client.Instance.ResponseReceived -= OnExchangeKeysResponse;
            }
        }

        // Send request to server
        public void SendRequest(Request request) {
            request.AccessToken = Client.Data.AccessToken;

            try {
                byte[] request_bytes = Serializer.Serialize(request);
                request_bytes = CompressionLZ4.Encode(request_bytes);

                if (Client.Data.IsConnectedViaAES) {
                    request_bytes = Client.Data.AES.Encrypt(request_bytes);
                }
                else if (Client.Data.IsConnectedViaRSA) {
                    request_bytes = EncryptionRSA.Encrypt(request_bytes, Client.Data.RSA);
                }

                request_bytes = Serializer.AddArrayLength(request_bytes);
                Client.Data.Stream.Write(request_bytes, 0, request_bytes.Length);
            }
            catch {
                // TODO: Write to log file
            }
        }

        // Remove event handler references
        public void UnsubscribeAllEvents() {
            ResponseReceived = null;
            NotificationReceived = null;
        }

        // Properties
        public static class Data {
            // User data
            public static string Username { get; set; }
            public static string AccessToken { get; set; }

            // Connection data
            internal static TcpClient TCP { get; set; }
            internal static NetworkStream Stream { get; set; }

            // Security
            public static bool IsConnectedViaRSA { get; internal set; }
            internal static RSAParameters RSA { get; set; }

            public static bool IsConnectedViaAES { get; internal set; }
            internal static EncryptionAES AES { get; set; }
        }

        // Events
        public event EventHandler<Response> ResponseReceived = null;
        public event EventHandler<Notification> NotificationReceived = null;
    }

}