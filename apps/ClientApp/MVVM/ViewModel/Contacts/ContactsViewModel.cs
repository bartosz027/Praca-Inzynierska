using ClientApp.Core;

using System.Collections.ObjectModel;
using System.Linq;

using ClientApp.MVVM.ViewModel.Contacts.Chat;
using ClientApp.MVVM.ViewModel.Contacts.Manager;

using Network.Client;
using Network.Client.DataProcessing;

using Network.Shared.DataTransfer.Model.Database.Friends.GetFriendList;
using Network.Shared.DataTransfer.Model.Friends.ManageInvitations.AcceptFriendInvitation;
using Network.Shared.DataTransfer.Model.Friends.ManageMessages.SendMessage;
using System.Collections.Generic;
using System;

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

        public bool IsANewMessage
        {
            get
            {
                return _IsANewMessage;
            }
            set
            {
                _IsANewMessage = value;
                OnPropertyChanged();
            }
        }
        private bool _IsANewMessage;
    }

    internal class ContactsViewModel : BaseVM {
        public ContactsViewModel() {
            EnableResponseListener();
            EnableNotificationListener();

            ContactManagerVM = new ManagerViewModel();
            FriendList = new ObservableCollection<ChatViewModel>();
            UnreaderdMessages = new ObservableCollection<int>();

            ContactManagerButtonCommand = new RelayCommand(o => {
                SelectedFriend = null;
                CurrentView = ContactManagerVM;
            });

            Client.Instance.SendRequest(new GetFriendListRequest());
        }

        // VM's
        public ManagerViewModel ContactManagerVM { get; private set; }
        public ObservableCollection<ChatViewModel> FriendList { get; private set; }
        
        // Commands
        public RelayCommand ContactManagerButtonCommand { get; private set; }

        // Properties
        public ChatViewModel SelectedFriend {
            get { 
                return _SelectedFriend; 
            }
            set {
                if (_SelectedFriend != null) 
                {
                    _SelectedFriend.IsSelected = false;
                }

                _SelectedFriend = value;

                if (_SelectedFriend != null) {
                    if (_SelectedFriend.FriendInfo.IsANewMessage == true)
                    {
                        _SelectedFriend.FriendInfo.IsANewMessage = false;
                        _SelectedFriend.IsSelected = true;
                        UnreaderdMessages.Remove(_SelectedFriend.FriendInfo.ID);
                        NotificationBall = UnreaderdMessages.Count > 0;
                    }
                    if (_SelectedFriend.Initialized == false)
                    {
                        _SelectedFriend.Init();
                    }
                }

                CurrentView = _SelectedFriend;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<int> UnreaderdMessages
        {
            get
            {
                return _UnreaderdMessages;
            }
            set
            {
                _UnreaderdMessages = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<int> _UnreaderdMessages;

        public bool NotificationBall
        {
            get
            {
                return _NotificationBall;
            }
            set
            {
                _NotificationBall = value;
                OnPropertyChanged();
            }
        }
        private bool _NotificationBall;

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
        protected override void OnResponseReceived(ResponseDispatcher dispatcher) {
            dispatcher.Dispatch<GetFriendListResponse>(OnGetFriendListResponse);
            dispatcher.Dispatch<AcceptFriendInvitationResponse>(OnAcceptFriendInvitationResponse);
        }

        private void OnGetFriendListResponse(GetFriendListResponse response) {
            var friendlist = response.FriendList;

            foreach (var friend in friendlist) {
                var friend_info = new FriendInfo() {
                    ID = friend.UserID,
                    Username = friend.Username,
                    Status = true,
                    
                    // TODO: Receive "status" from server
                };
                
                FriendList.Add(new ChatViewModel(friend_info));
            }
        }

        private void OnAcceptFriendInvitationResponse(AcceptFriendInvitationResponse response) {
            var friend = new FriendInfo() {
                ID = response.UserID,
                Username = response.Username,
                Status = true,
                IsANewMessage = true

                // TODO: Receive "status" from server
            };

            var invitation = ContactManagerVM.ReceivedInvitations.Single(p => p.UserID == response.UserID);
            ContactManagerVM.ReceivedInvitations.Remove(invitation);
            ContactManagerVM.UpdateNotifcationBall();

            FriendList.Add(new ChatViewModel(friend));
        }

        // Notification events
        protected override void OnNotificationReceived(NotificationDispatcher dispatcher) {
            dispatcher.Dispatch<AcceptFriendInvitationNotification>(OnAcceptFriendInvitationNotification);
            dispatcher.Dispatch<SendMessageNotification>(OnSendMessageNotification);
        }

        private void OnSendMessageNotification(SendMessageNotification obj)
        {
            var id = obj.FriendID;
            if (!UnreaderdMessages.Contains(id))
            {
                if(SelectedFriend == null || SelectedFriend.FriendInfo.ID != id)
                UnreaderdMessages.Add(id);
            }
            NotificationBall = UnreaderdMessages.Count > 0;
        }

        private void OnAcceptFriendInvitationNotification(AcceptFriendInvitationNotification notification) {
            var friend = new FriendInfo() {
                ID = notification.UserID,
                Username = notification.Username,
                Status = true 
                
                // TODO: Receive "status" from server
            };

            var invitation = ContactManagerVM.PendingInvitations.Single(p => p.UserID == notification.UserID);
            ContactManagerVM.PendingInvitations.Remove(invitation);

            FriendList.Add(new ChatViewModel(friend));
        }

    }

}