using ClientApp.Core;
using ClientApp.MVVM.Model;
using Network.Client;
using Network.Client.DataProcessing;
using Network.Shared.DataTransfer.Base;
using Network.Shared.DataTransfer.Model.Friends.AddFriend;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.MVVM.ViewModel.PIWindowViewModel.ContactsViewModels
{
    internal class ContactManagerViewModel : ObservableObject
    {
        public ContactManagerViewModel()
        {
            Client.Instance.NotificationReceived += OnNotificationReceived;

            PendingInvitations = new ObservableCollection<ContactManagerItemModel>();
            Invitations = new ObservableCollection<ContactManagerItemModel>();
            Blocked = new ObservableCollection<ContactManagerItemModel>();

            NotificationListVM = new NotificationListViewModel();
            AddContactVM = new AddContactViewModel(PendingInvitations);

            CurrentView = NotificationListVM;

            AddContactViewButtonCommand = new RelayCommand(o =>
            {
                CurrentView = AddContactVM;
            });
            PendingInvitationsButtonCommand = new RelayCommand(o =>
            {
                NotificationListVM.CurrentList = PendingInvitations;
                CurrentView = NotificationListVM;
            });
            InvitationsButtonCommand = new RelayCommand(o =>
            {
                NotificationListVM.CurrentList = Invitations;
                CurrentView = NotificationListVM;
            });
            BlockedButtonCommand = new RelayCommand(o =>
            {
                NotificationListVM.CurrentList = Blocked;
                CurrentView = NotificationListVM;
            });
        }

        // VM' s
        AddContactViewModel AddContactVM { get; set; }
        NotificationListViewModel NotificationListVM { get; set; }

        // Observable properties
        public ObservableCollection<ContactManagerItemModel> PendingInvitations
        {
            get { return _PendingInvitations; }
            set
            {
                _PendingInvitations = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ContactManagerItemModel> _PendingInvitations;

        public ObservableCollection<ContactManagerItemModel> Invitations
        {
            get { return _Invitations; }
            set
            {
                _Invitations = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ContactManagerItemModel> _Invitations;

        public ObservableCollection<ContactManagerItemModel> Blocked
        {
            get { return _Blocked; }
            set
            {
                _Blocked = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ContactManagerItemModel> _Blocked;


        // Commands
        public RelayCommand PendingInvitationsButtonCommand  { get; set; }
        public RelayCommand InvitationsButtonCommand { get; set; }
        public RelayCommand BlockedButtonCommand { get; set; }
        public RelayCommand AddContactViewButtonCommand { get; set; }

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

        // Notification event handling
        private void OnNotificationReceived(object sender, Notification notification) {
            var dispatcher = new NotificationDispatcher(notification);

            App.Current.Dispatcher.Invoke(delegate {
                dispatcher.Dispatch<AddFriendNotification>(OnAddFriendNotification);
            });
        }

        private void OnAddFriendNotification(AddFriendNotification notification) {
            var item = new ContactManagerItemModel { 
                Username = notification.Username, 
                IsEnabledAcceptOption = true, 
                IsEnabledDeclineOption = true, 
                ItemInfo = "Invitation" 
            };

            Invitations.Add(item);
        }
    }
}