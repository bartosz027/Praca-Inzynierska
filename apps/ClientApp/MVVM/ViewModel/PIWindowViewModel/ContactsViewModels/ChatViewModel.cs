using ClientApp.Core;
using ClientApp.MVVM.Model;
using Network.Client;
using Network.Client.DataProcessing;
using Network.Shared.DataTransfer.Base;
using Network.Shared.DataTransfer.Model.Friends.SendMessage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClientApp.MVVM.ViewModel.PIWindowViewModel.ContactsViewModels
{
    internal class ChatViewModel : ObservableObject
    {
       
        public ChatViewModel(FriendModel friendModel)
        {
            Messages = new ObservableCollection<MessageModel>();
            CurrentFriend = friendModel;
            Client.Instance.NotificationReceived += OnNotificationReceived;

            SendMessageCommand = new RelayCommand(o => 
            {
                if (RichBoxContent.Length >1)
                {
                    Messages.Add(new MessageModel { Date = DateTime.Now.ToString("HH:mm"), Content = RichBoxContent,  Sender = Client.Data.Username});
                    
                    Client.Instance.SendRequest(new SendMessageRequest()
                    {
                        Content = RichBoxContent,
                        ReceiverID = CurrentFriend.UserID
                    });

                    RichBoxContent = String.Empty;
                }
            });

            TestoweUsuwanie = new RelayCommand(o => 
            {
                MessageBox.Show("TEST");
            });
        }

        public RelayCommand SendMessageCommand { get; set; }
        public RelayCommand TestoweUsuwanie { get; set; }
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

        public FriendModel CurrentFriend
        {
            get { return _CurrentFriend; }
            set
            {
                _CurrentFriend = value;
                OnPropertyChanged();
            }
        }
        private FriendModel _CurrentFriend;

        // 
        private void OnNotificationReceived(object sender, Notification notification)
        {
            var dispatcher = new NotificationDispatcher(notification);
            dispatcher.Dispatch<SendMessageNotification>(OnSendMessageNotification);
        }

        private void OnSendMessageNotification(SendMessageNotification notification)
        {
            if(notification.SenderID == CurrentFriend.UserID)
            {
                App.Current.Dispatcher.Invoke(delegate {
                    Messages.Add(new MessageModel { Date = DateTime.Now.ToString("HH:mm"), Content = notification.Content, Sender = CurrentFriend.Username });
                });
            }
        }
    }
}