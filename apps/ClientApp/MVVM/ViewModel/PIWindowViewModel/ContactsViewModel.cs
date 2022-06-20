using System;
using System.Collections.ObjectModel;
using System.Linq;
using ClientApp.Core;

using ClientApp.MVVM.Model;
using ClientApp.MVVM.ViewModel.PIWindowViewModel.ContactsViewModels;

using Network.Client;
using Network.Client.DataProcessing;

using Network.Shared.DataTransfer.Base;
using Network.Shared.DataTransfer.Model.Database.Friends;
using Network.Shared.DataTransfer.Model.Friends.AcceptFriendInvitation;

namespace ClientApp.MVVM.ViewModel.PIWindowViewModel
{
    internal class ContactsViewModel : ObservableObject
    {
        public ContactsViewModel()
        {
            Client.Instance.ResponseReceived += OnResponseReceived;
            Client.Instance.NotificationReceived += OnNotificationReceived;

            ContactManagerVM = new ContactManagerViewModel();
            FriendList = new ObservableCollection<ChatViewModel>();

            Client.Instance.SendRequest(new FriendListRequest());
            CurrentView = SelectedFriend;

            ContactManagerButtonCommand = new RelayCommand(o => 
            {
                   SelectedFriend = null;
                   CurrentView = ContactManagerVM;
            });
        }

        // VM's
        public ObservableCollection<ChatViewModel> FriendList { get; set; }
        public ContactManagerViewModel ContactManagerVM { get; set; }

        // Obserable properties
        public ChatViewModel SelectedFriend
        {
            get { return _SelectedFriend; }
            set 
            {
                _SelectedFriend = value;

                if(SelectedFriend != null && !SelectedFriend.Initialized) 
                {
                    SelectedFriend.Init();
                }

                CurrentView = SelectedFriend;
                OnPropertyChanged();
            }
        }
        private ChatViewModel _SelectedFriend;

        // Commands
        public RelayCommand ContactManagerButtonCommand { get; set; }

        // Current view
        public object CurrentView
        {
            get { return _CurrentView; }
            set
            {
                _CurrentView = value;
                OnPropertyChanged();
            }
        }
        private object _CurrentView;

        // Response event handling
        private void OnResponseReceived(object sender, Response response)
        {
            var dispatcher = new ResponseDispatcher(response);

            App.Current.Dispatcher.Invoke(delegate {
                dispatcher.Dispatch<FriendListResponse>(OnFriendsListResponse);
                dispatcher.Dispatch<AcceptFriendInvitationResponse>(OnAcceptFriendInvitationResponse);
            });
        }

        private void OnFriendsListResponse(FriendListResponse response)
        {
            foreach (var friend_info in response.FriendList)
            {
                var friend = new FriendModel()
                {
                    UserID = friend_info.UserID,
                    Username = friend_info.Username
                };

                friend.Status = true;
                FriendList.Add(new ChatViewModel(friend));
            }
        }

        private void OnAcceptFriendInvitationResponse(AcceptFriendInvitationResponse response) 
        {
            var friend = new FriendModel() {
                UserID = response.UserID,
                Username = response.Username,
                Status = true
            };

            FriendList.Add(new ChatViewModel(friend));
        }

        // Notification event handling
        private void OnNotificationReceived(object sender, Notification notification) {
            var dispatcher = new NotificationDispatcher(notification);

            App.Current.Dispatcher.Invoke(delegate {
                dispatcher.Dispatch<AcceptFriendInvitationNotification>(OnAcceptFriendInvitationNotification);
            });
        }

        private void OnAcceptFriendInvitationNotification(AcceptFriendInvitationNotification notification) {
            var friend = new FriendModel() {
                UserID = notification.UserID,
                Username = notification.Username,
                Status = true
            };

            var inv = ContactManagerVM.PendingInvitations.Where(p => p.Username == notification.Username).Single();
            ContactManagerVM.PendingInvitations.Remove(inv);

            FriendList.Add(new ChatViewModel(friend));
        }
    }
}