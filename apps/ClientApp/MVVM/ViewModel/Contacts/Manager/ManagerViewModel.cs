using System;
using System.Collections.ObjectModel;

using ClientApp.Core;

using Network.Client;
using Network.Client.DataProcessing;

using Network.Shared.DataTransfer.Model.Database.Friends;
using Network.Shared.DataTransfer.Model.Friends.ManageInvitations.SendFriendInvitation;

namespace ClientApp.MVVM.ViewModel.Contacts.Manager {

    internal class ContactManagerItem : ObservableObject {
        public int UserID { get; set; }

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

        public string ItemInfoType {
            get { 
                return _ItemInfoType; 
            }
            set {
                _ItemInfoType = value;
                OnPropertyChanged();
            }
        }
        private string _ItemInfoType;

        public bool IsEnabledAcceptOption { get; set; }
        public bool IsEnabledDeclineOption { get; set; }
    }

    internal class ManagerViewModel : BaseViewModel {
        public ManagerViewModel() {
            PendingInvitations = new ObservableCollection<ContactManagerItem>();
            ReceivedInvitations = new ObservableCollection<ContactManagerItem>();

            AddContactVM = new AddContactViewModel(PendingInvitations);
            InvitationsVM = new InvitationsViewModel();

            AddContactViewButtonCommand = new RelayCommand(o => {
                CurrentView = AddContactVM;
            });

            PendingInvitationsButtonCommand = new RelayCommand(o => {
                InvitationsVM.CurrentList = PendingInvitations;
                CurrentView = InvitationsVM;
            });

            ReceivedInvitationsButtonCommand = new RelayCommand(o => {
                InvitationsVM.CurrentList = ReceivedInvitations;
                CurrentView = InvitationsVM;
            });

            BlockedButtonCommand = new RelayCommand(o => {
                // TODO: Implement blocked contacts
            });

            Client.Instance.SendRequest(new GetInvitationsRequest());
        }

        // VM's
        public AddContactViewModel AddContactVM { get; set; }
        public InvitationsViewModel InvitationsVM { get; set; }

        // Commands
        public RelayCommand AddContactViewButtonCommand { get; set; }
        public RelayCommand PendingInvitationsButtonCommand  { get; set; }
        public RelayCommand ReceivedInvitationsButtonCommand { get; set; }
        public RelayCommand BlockedButtonCommand { get; set; }

        // Properties
        public ObservableCollection<ContactManagerItem> PendingInvitations {
            get {
                return _PendingInvitations;
            }
            set {
                _PendingInvitations = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ContactManagerItem> _PendingInvitations;

        public ObservableCollection<ContactManagerItem> ReceivedInvitations {
            get {
                return _ReceivedInvitations;
            }
            set {
                _ReceivedInvitations = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ContactManagerItem> _ReceivedInvitations;

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
            dispatcher.Dispatch<GetInvitationsResponse>(OnGetInvitationsResponse);
        }

        private void OnGetInvitationsResponse(GetInvitationsResponse response) {
            foreach (var invitation in response.PendingInvitations) {
                var item = new ContactManagerItem() {
                    UserID = invitation.UserID,
                    Username = invitation.Username,

                    IsEnabledAcceptOption = false,
                    IsEnabledDeclineOption = true,

                    ItemInfoType = "PendingInvitation"
                };

                PendingInvitations.Add(item);
            }

            foreach (var invitation in response.ReceivedInvitations) {
                var item = new ContactManagerItem() {
                    UserID = invitation.UserID,
                    Username = invitation.Username,

                    IsEnabledAcceptOption = true,
                    IsEnabledDeclineOption = true,

                    ItemInfoType = "ReceivedInvitation"
                };

                ReceivedInvitations.Add(item);
            }
        }

        // Notification events
        protected override void NotificationReceived(NotificationDispatcher dispatcher) {
            dispatcher.Dispatch<SendFriendInvitationNotification>(OnSendFriendInvitationNotification);
        }

        private void OnSendFriendInvitationNotification(SendFriendInvitationNotification notification) {
            var item = new ContactManagerItem() {
                UserID = notification.UserID,
                Username = notification.Username,

                IsEnabledAcceptOption = true,
                IsEnabledDeclineOption = true,

                ItemInfoType = "ReceivedInvitation"
            };

            ReceivedInvitations.Add(item);
        }
    }

}