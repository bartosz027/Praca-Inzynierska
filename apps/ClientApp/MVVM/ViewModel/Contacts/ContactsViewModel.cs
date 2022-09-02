using System;
using System.Collections.ObjectModel;
using System.Linq;

using ClientApp.Core;

using ClientApp.MVVM.ViewModel.Contacts.Chat;
using ClientApp.MVVM.ViewModel.Contacts.Manager;

using Network.Client;
using Network.Client.DataProcessing;

using Network.Shared.DataTransfer.Model.Database.Friends;
using Network.Shared.DataTransfer.Model.Friends.ManageInvitations.AcceptFriendInvitation;

namespace ClientApp.MVVM.ViewModel.Contacts {

    internal class FriendInfo : ObservableObject {
        public int ID { get; set; }

        public string Username {
            get { 
                return _Username; 
            }
            set {
                _Username = value;
                OnPropertyChanged();
            }
        }
        private string _Username;

        public bool Status {
            get { 
                return _Status; 
            }
            set {
                _Status = value;
                OnPropertyChanged();
            }
        } 
        private bool _Status;
    }

    internal class ContactsViewModel : BaseViewModel {
        public ContactsViewModel() {
            ContactManagerVM = new ManagerViewModel();

            FriendList = new ObservableCollection<ChatViewModel>();
            Client.Instance.SendRequest(new GetFriendListRequest());

            ContactManagerButtonCommand = new RelayCommand(o => {
                SelectedFriend = null;
                CurrentView = ContactManagerVM;
            });
        }

        // VM's
        public ManagerViewModel ContactManagerVM { get; set; }
        public ObservableCollection<ChatViewModel> FriendList { get; set; }

        // Commands
        public RelayCommand ContactManagerButtonCommand { get; set; }

        // Properties
        public ChatViewModel SelectedFriend {
            get { 
                return _SelectedFriend; 
            }
            set {
                _SelectedFriend = value;

                if (_SelectedFriend != null && !_SelectedFriend.Initialized) {
                    _SelectedFriend.Init();
                }

                CurrentView = _SelectedFriend;
                OnPropertyChanged();
            }
        }
        private ChatViewModel _SelectedFriend;

        // Current view
        public object CurrentView {
            get { 
                return _CurrentView; 
            }
            set {
                _CurrentView = value;
                OnPropertyChanged();
            }
        }
        private object _CurrentView;

        // Response events
        protected override void ResponseReceived(ResponseDispatcher dispatcher) {
            dispatcher.Dispatch<GetFriendListResponse>(OnGetFriendListResponse);
            dispatcher.Dispatch<AcceptFriendInvitationResponse>(OnAcceptFriendInvitationResponse);
        }

        private void OnGetFriendListResponse(GetFriendListResponse response) {
            var friendlist = response.FriendList;

            foreach (var friend in friendlist) {
                var friend_info = new FriendInfo() {
                    ID = friend.UserID,
                    Username = friend.Username,
                    Status = true
                    
                    // TODO: Receive "status" from server
                };

                FriendList.Add(new ChatViewModel(friend_info));
            }
        }

        private void OnAcceptFriendInvitationResponse(AcceptFriendInvitationResponse response) {
            var friend = new FriendInfo() {
                ID = response.UserID,
                Username = response.Username,
                Status = true // TODO: Receive "status" from server
            };

            var invitation = ContactManagerVM.ReceivedInvitations.Single(p => p.UserID == response.UserID);
            ContactManagerVM.ReceivedInvitations.Remove(invitation);

            FriendList.Add(new ChatViewModel(friend));
        }

        // Notification events
        protected override void NotificationReceived(NotificationDispatcher dispatcher) {
            dispatcher.Dispatch<AcceptFriendInvitationNotification>(OnAcceptFriendInvitationNotification);
        }

        private void OnAcceptFriendInvitationNotification(AcceptFriendInvitationNotification notification) {
            var friend = new FriendInfo() {
                ID = notification.UserID,
                Username = notification.Username,
                Status = true // TODO: Receive "status" from server
            };

            var invitation = ContactManagerVM.PendingInvitations.Single(p => p.UserID == notification.UserID);
            ContactManagerVM.PendingInvitations.Remove(invitation);

            FriendList.Add(new ChatViewModel(friend));
        }
    }

}