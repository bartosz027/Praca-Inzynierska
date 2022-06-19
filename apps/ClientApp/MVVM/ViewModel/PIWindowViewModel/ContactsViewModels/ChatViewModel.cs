using System;
using System.Collections.ObjectModel;
using System.Windows;

using ClientApp.Core;
using ClientApp.MVVM.Model;

using Network.Client;
using Network.Client.DataProcessing;

using Network.Shared.DataTransfer.Base;

using Network.Shared.DataTransfer.Model.Database.Friends;
using Network.Shared.DataTransfer.Model.Friends.SendMessage;

namespace ClientApp.MVVM.ViewModel.PIWindowViewModel.ContactsViewModels
{
    internal class ChatViewModel : ObservableObject
    {
        public ChatViewModel(FriendModel friend)
        {
            Client.Instance.ResponseReceived += OnResponseReceived;
            Client.Instance.NotificationReceived += OnNotificationReceived;

            Friend = friend;
            Messages = new ObservableCollection<MessageModel>();

            SendMessageCommand = new RelayCommand(o => 
            {
                if (RichBoxContent.Length > 1)
                {
                    var current_time = DateTime.Now;
                    Messages.Add(new MessageModel { Date = current_time.ToString("HH:mm"), Content = RichBoxContent, Sender = Client.Data.Username, IsMyMessage = true});
                    
                    Client.Instance.SendRequest(new SendMessageRequest()
                    {
                        ReceiverID = Friend.UserID,

                        Content = RichBoxContent,
                        SendDate = current_time
                    });

                    RichBoxContent = String.Empty;
                }
            });

            RemoveMessageCommand = new RelayCommand(o => 
            {
                var message = o as MessageModel;
                Messages.Remove(message);
            });
            CopyMessageCommand = new RelayCommand(o =>
            {
                var message = o as MessageModel;
                Clipboard.SetText(message.Content);
            });
        }

        // Methods
        public void Init() 
        {
            Client.Instance.SendRequest(new MessageHistoryRequest() 
            {
                FriendID = Friend.UserID
            });

            // TODO: Move this to "OnMessageHistoryResponse" method
            Initialized = true;
        }

        // Commands
        public RelayCommand SendMessageCommand { get; set; }
        public RelayCommand RemoveMessageCommand { get; set; }
        public RelayCommand CopyMessageCommand { get; set; }

        // Common properties
        public bool Initialized { get; private set; }

        // Observable properties
        public FriendModel Friend
        {
            get { return _Friend; }
            set
            {
                _Friend = value;
                OnPropertyChanged();
            }
        }
        private FriendModel _Friend;

        public string RichBoxContent 
        {
            get { return _RichBoxContent; }
            set 
            {
                _RichBoxContent = value;
                OnPropertyChanged();
            }
        }
        private string _RichBoxContent;

        public ObservableCollection<MessageModel> Messages
        {
            get { return _Messages; }
            set
            {
                _Messages = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<MessageModel> _Messages;

        // Response event handling
        private void OnResponseReceived(object sender, Response response) 
        {
            var dispatcher = new ResponseDispatcher(response);

            App.Current.Dispatcher.Invoke(delegate {
                dispatcher.Dispatch<MessageHistoryResponse>(OnMessageHistoryResponse);
            });
        }

        private void OnMessageHistoryResponse(MessageHistoryResponse response) 
        {
            if(response.FriendID == Friend.UserID) 
            {
                foreach(var message_info in response.Messages) 
                {
                    var message = new MessageModel();
                    message.Content = message_info.Content;

                    if (message_info.SenderID == Friend.UserID)
                    {
                        message.Sender = Friend.Username;
                    }
                    else
                    {
                        message.Sender = Client.Data.Username;
                        message.IsMyMessage = true;
                    }
                    
                    message.Date = message_info.SendDate.ToString("HH:mm");
                    Messages.Add(message);
                }
            }
        }

        // Notification event handling
        private void OnNotificationReceived(object sender, Notification notification)
        {
            var dispatcher = new NotificationDispatcher(notification);

            App.Current.Dispatcher.Invoke(delegate {
                dispatcher.Dispatch<SendMessageNotification>(OnSendMessageNotification);
            });
        }

        private void OnSendMessageNotification(SendMessageNotification notification)
        {
            if(notification.SenderID == Friend.UserID)
            {
                Messages.Add(new MessageModel { Date = DateTime.Now.ToString("HH:mm"), Content = notification.Content, Sender = Friend.Username });
            }
        }
    }
}