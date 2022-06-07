using ClientApp.Core;
using ClientApp.MVVM.Model;
using ClientApp.MVVM.ViewModel.PIWindowViewModel.ContactsViewModels;
using Network.Client;
using Network.Client.DataProcessing;
using Network.Shared.DataTransfer.Base;
using Network.Shared.DataTransfer.Model.Database.Friends;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.MVVM.ViewModel.PIWindowViewModel
{
    internal class ContactsViewModel : ObservableObject
    {
        public ContactsViewModel()
        {
            Client.Instance.ResponseReceived += OnResponseReceived;

            FriendList = new ObservableCollection<ChatViewModel>();

            Client.Instance.SendRequest(new FriendsListRequest()
            {
                
            });

            CurrentView = SelectedFriend;
        }
        // Current View
        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }
        private object _currentView;


        // items
        public ObservableCollection<ChatViewModel> FriendList { get; set; }
        public ChatViewModel SelectedFriend
        {
            get { return _SelectedFriend; }
            set
            {
                _SelectedFriend = value;
                CurrentView = _SelectedFriend;
                OnPropertyChanged();
            }
        }
        private ChatViewModel _SelectedFriend;

        private void OnResponseReceived(object sender, Response response)
        {
            var dispatcher = new ResponseDispatcher(response);
            dispatcher.Dispatch<FriendsListResponse>(OnFriendsListResponse);
        }

        private void OnFriendsListResponse(FriendsListResponse response)
        {
            App.Current.Dispatcher.Invoke(delegate 
            {
                foreach (var friend in response.FriendsList)
                    FriendList.Add(new ChatViewModel(new FriendModel
                    {
                        UserID = friend.UserID,
                        Username = friend.Username,
                        Status = true
                    }));
            });
        }
    }
}
