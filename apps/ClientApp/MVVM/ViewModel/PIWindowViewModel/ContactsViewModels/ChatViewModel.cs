using System;
using System.Collections.ObjectModel;
using System.Windows;

using ClientApp.Core;
using ClientApp.MVVM.Model;

using Network.Client;
using Network.Client.DataProcessing;

using Network.Shared.DataTransfer.Base;
using Network.Shared.DataTransfer.Model.Friends.SendMessage;

namespace ClientApp.MVVM.ViewModel.PIWindowViewModel.ContactsViewModels
{
    internal class ChatViewModel : ObservableObject
    {
        public ChatViewModel(FriendModel friend)
        {
            Client.Instance.NotificationReceived += OnNotificationReceived;

            Friend = friend;
            Messages = new ObservableCollection<MessageModel>();

            SendMessageCommand = new RelayCommand(o => 
            {
                if (RichBoxContent.Length > 1)
                {
                    Messages.Add(new MessageModel { Date = DateTime.Now.ToString("HH:mm"), Content = RichBoxContent, Sender = Client.Data.Username});
                    
                    Client.Instance.SendRequest(new SendMessageRequest()
                    {
                        Content = RichBoxContent,
                        ReceiverID = Friend.UserID
                    });

                    RichBoxContent = String.Empty;
                }
            });

            TestoweUsuwanie = new RelayCommand(o => 
            {
                MessageBox.Show("TEST");
            });
        }

        // Commands
        public RelayCommand SendMessageCommand { get; set; }
        public RelayCommand TestoweUsuwanie { get; set; }

        // Properties
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