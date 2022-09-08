using System;
using System.Collections.ObjectModel;

using System.Linq;
using System.Windows;

using ClientApp.Core;

using Network.Client;
using Network.Client.DataProcessing;

using Network.Shared.DataTransfer.Base;
using Network.Shared.DataTransfer.Model.Database.Friends.GetMessageHistory;

using Network.Shared.DataTransfer.Model.Friends.ManageMessages.DeleteMessage;
using Network.Shared.DataTransfer.Model.Friends.ManageMessages.SendMessage;

namespace ClientApp.MVVM.ViewModel.Contacts.Chat {

    internal class MessageInfo : ObservableObject {
        public int ID { get; set; }
        public bool IsMyMessage { get; set; }

        public string Content {
            get { 
                return _Content; 
            }
            set {
                _Content = value;
                OnPropertyChanged();
            }
        }
        private string _Content;

        public string Date {
            get { 
                return _Date; 
            }
            set {
                _Date = value;
                OnPropertyChanged();
            }
        }
        private string _Date;

        public string Sender {
            get { 
                return _Sender; 
            }
            set {
                _Sender = value;
                OnPropertyChanged();
            }
        }
        private string _Sender;
    }

    internal class ChatViewModel : BaseVM {
        public ChatViewModel(FriendInfo friend_info) {
            EnableResponseListener();
            EnableNotificationListener();

            FriendInfo = friend_info;
            Messages = new ObservableCollection<MessageInfo>();

            SendMessageCommand = new RelayCommand(o => {
                if (RichBoxContent.Length > 1) {
                    Client.Instance.SendRequest(new SendMessageRequest() {
                        FriendID = FriendInfo.ID,
                        Content = RichBoxContent,
                    });

                    RichBoxContent = String.Empty;
                }
            });

            RemoveMessageCommand = new RelayCommand(o => {
                var message = o as MessageInfo;

                Client.Instance.SendRequest(new DeleteMessageRequest() {
                    FriendID = FriendInfo.ID,
                    MessageID = message.ID
                });
            });

            CopyMessageCommand = new RelayCommand(o => {
                var message = o as MessageInfo;
                Clipboard.SetText(message.Content);
            });
        }

        // Methods
        public void Init() {
            Client.Instance.SendRequest(new GetMessageHistoryRequest() {
                FriendID = FriendInfo.ID
            });
        }
        public bool Initialized { get; set; }

        // Commands
        public RelayCommand SendMessageCommand { get; private set; }
        public RelayCommand RemoveMessageCommand { get; private set; }
        public RelayCommand CopyMessageCommand { get; private set; }

        // Properties
        public FriendInfo FriendInfo {
            get { 
                return _Friend; 
            }
            set {
                _Friend = value;
                OnPropertyChanged();
            }
        }
        private FriendInfo _Friend;

        public string RichBoxContent {
            get { 
                return _RichBoxContent; 
            }
            set {
                _RichBoxContent = value;
                OnPropertyChanged();
            }
        }
        private string _RichBoxContent;

        public ObservableCollection<MessageInfo> Messages {
            get { 
                return _Messages; 
            }
            set {
                _Messages = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<MessageInfo> _Messages;

        // Response events
        protected override void OnResponseReceived(ResponseDispatcher dispatcher) {
            dispatcher.Dispatch<GetMessageHistoryResponse>(OnGetMessageHistoryResponse);
            dispatcher.Dispatch<DeleteMessageResponse>(OnDeleteMessageResponse);
            dispatcher.Dispatch<SendMessageResponse>(OnSendMessageResponse);
        }

        private void OnGetMessageHistoryResponse(GetMessageHistoryResponse response) {
            if (response.FriendID == FriendInfo.ID) {
                var messages = response.Messages;

                foreach (var message_info in messages) {
                    var message = new MessageInfo() {
                        ID = message_info.ID,
                        Date = message_info.SendDate.ToLocalTime().ToString("HH:mm"),
                        Content = message_info.Content
                    };

                    if (message_info.SenderID != FriendInfo.ID) {
                        message.Sender = Client.Data.Username;
                        message.IsMyMessage = true;
                    }
                    else {
                        message.Sender = FriendInfo.Username;
                        message.IsMyMessage = false;
                    }

                    Messages.Add(message);
                }

                Initialized = true;
            }
        }

        private void OnDeleteMessageResponse(DeleteMessageResponse response) {
            if (response.FriendID == FriendInfo.ID) {
                Messages.Remove(Messages.Single(p => p.ID == response.MessageID));
            }
        }

        private void OnSendMessageResponse(SendMessageResponse response) {
            if (response.FriendID == FriendInfo.ID) {
                Messages.Add(new MessageInfo() {
                    ID = response.MessageID,
                    Date = response.SendDate.ToLocalTime().ToString("HH:mm"),
                    Content = response.Content,

                    Sender = Client.Data.Username,
                    IsMyMessage = true
                });
            }
        }

        // Notification events
        protected override void OnNotificationReceived(NotificationDispatcher dispatcher) {
            dispatcher.Dispatch<DeleteMessageNotification>(OnDeleteMessageNotification);
            dispatcher.Dispatch<SendMessageNotification>(OnSendMessageNotification);
        }

        private void OnDeleteMessageNotification(DeleteMessageNotification notification) {
            if (Initialized && notification.FriendID == FriendInfo.ID) {
                var message = Messages.Single(p => p.ID == notification.MessageID);
                Messages.Remove(message);
            }
        }

        private void OnSendMessageNotification(SendMessageNotification notification) {
            if (Initialized && notification.FriendID == FriendInfo.ID) {
                Messages.Add(new MessageInfo() {
                    ID = notification.MessageID,
                    Date = notification.SendDate.ToLocalTime().ToString("HH:mm"),
                    Content = notification.Content,

                    Sender = FriendInfo.Username,
                    IsMyMessage = false
                });
            }
        }
    }

}