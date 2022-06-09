using System;
using System.Collections.ObjectModel;

using ClientApp.Core;

using ClientApp.MVVM.Model;
using ClientApp.MVVM.ViewModel.PIWindowViewModel.ContactsViewModels;

using Network.Client;
using Network.Client.DataProcessing;

using Network.Shared.DataTransfer.Base;
using Network.Shared.DataTransfer.Model.Database.Friends;

namespace ClientApp.MVVM.ViewModel.PIWindowViewModel
{
    internal class ContactsViewModel : ObservableObject
    {
        public ContactsViewModel()
        {
            Client.Instance.ResponseReceived += OnResponseReceived;

            FriendList = new ObservableCollection<ChatViewModel>();
            Client.Instance.SendRequest(new FriendListRequest());

            CurrentView = SelectedFriend;
        }

        // VM's
        public ObservableCollection<ChatViewModel> FriendList { get; set; }

        // Obserable properties
        public ChatViewModel SelectedFriend
        {
            get { return _SelectedFriend; }
            set 
            {
                _SelectedFriend = value;

                if(!SelectedFriend.Initialized) 
                {
                    SelectedFriend.Init();
                }

                CurrentView = SelectedFriend;
                OnPropertyChanged();
            }
        }
        private ChatViewModel _SelectedFriend;

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
    }
}