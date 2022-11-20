using System;
using System.Collections.Concurrent;
using System.Text;

using System.Net;
using System.Net.Sockets;

using System.Threading;
using System.Threading.Tasks;

using Network.Client.DataProcessing;
using Network.Client.Model;

using Network.Shared.Core;
using Network.Shared.Core.Audio;

using Network.Shared.DataTransfer.Base;
using Network.Shared.DataTransfer.Security.ExchangeAESKeys;
using Network.Shared.DataTransfer.Security.ExchangeRSAKeys;

namespace Network.Client {

    public class Client {
        private Client() {
            EncryptionRSA.Init();

            Client.Data = new ClientInfo();
            Client.Data.TCP = new TcpClient();

            _Audio = new Audio();
            _NoiseGate = new NoiseGate();

            Client.Data.UDP = new UdpClient(0);
            UdpListenerThread();

            _Audio.StartVoiceRecording((sender, args) => {
                if(Client.Data.ClientEndPoint != null) {
                    try {
                        byte[] data = _NoiseGate.ApplyNoiseGate(args.Buffer);

                        data = _Audio.Encode(data);
                        data = AES.Encrypt(data);

                        if (data != null) {
                            Client.Data.UDP.Send(data, data.Length, Client.Data.ClientEndPoint);
                        }
                    }
                    catch { }
                }
            });
        }

        private void UdpListenerThread() {
            Task.Factory.StartNew(() => {
                while (true) {
                    try {
                        IPEndPoint ep = null;
                        byte[] data = Client.Data.UDP.Receive(ref ep);

                        if (Encoding.UTF8.GetString(data) != "NULL") {
                            data = AES.Decrypt(data);
                            data = _Audio.Decode(data);

                            if (ep != null) {
                                _Audio.Play(data);
                            }
                        }
                    }
                    catch { }
                }
            }, TaskCreationOptions.LongRunning);
        }

        // Singleton
        public static Client Instance {
            get { return _Instance ??= new Client(); }
            private set { _Instance = value; }
        }
        private static Client _Instance;

        // Connect to server
        public void Connect(string ip, int port) {
            Client.Data.ServerEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            Client.Data.TCP.Connect(ip, port);
            Client.Data.Stream = Client.Data.TCP.GetStream();

            if (Client.Data.TCP.Connected) {
                StartListenerThread();
            }
        }

        private void StartListenerThread() {
            var received_data_queue = new BlockingCollection<object>();
            var cancellation = new CancellationTokenSource();

            // Receive data
            Task.Factory.StartNew(() => {
                byte[] receive_buffer = new byte[256 * 1024 * 1024]; // 256 MB cache
                int buffer_length = 0;

                while (true) {
                    try {
                        byte[] received_bytes = new byte[Client.Data.TCP.Available];
                        Client.Data.Stream.Read(received_bytes, 0, received_bytes.Length);

                        Array.Copy(received_bytes, 0, receive_buffer, buffer_length, received_bytes.Length);
                        buffer_length += received_bytes.Length;

                        for (int index = 0, length = 0; index <= buffer_length; index += length + 4) {
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
                            var token = Client.Data.AccessToken;

                            Client.Data = new ClientInfo();
                            Client.Data.TCP = new TcpClient();

                            Client.Data.AccessToken = token;
                            ConnectionLost?.Invoke(this, new EventArgs());

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
            catch {}
        }

        // Remove event handler references
        public void UnsubscribeAllEvents() {
            ConnectionLost = null;
            ResponseReceived = null;
            NotificationReceived = null;
        }

        // Properties
        public static ClientInfo Data { get; internal set; }
        public static EncryptionAES AES { get; set; }

        // Voice chat
        private Audio _Audio;
        private NoiseGate _NoiseGate;

        // Events
        public event EventHandler<Response> ResponseReceived;
        public event EventHandler<Notification> NotificationReceived;
        public event EventHandler<EventArgs> ConnectionLost;
    }

}