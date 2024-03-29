﻿using ClientApp.Core;
using System.Collections.ObjectModel;

using Network.Client;
using Network.Shared.DataTransfer.Model.Database.Friends.GetInvitations;

namespace ClientApp.MVVM.ViewModel.Contacts.Manager {

    internal class ContactManagerItem : ObservableObject {
        public int UserID { get; set; }

        public bool IsEnabledAcceptOption { get; set; }
        public bool IsEnabledDeclineOption { get; set; }

        public string Username {
            get {
                return _Username;
            }
            set {
                _Username = value;
                OnPropertyChanged();
            }
        }

        public string ItemInfoType {
            get {
                return _ItemInfoType;
            }
            set {
                _ItemInfoType = value;
                OnPropertyChanged();
            }
        }

        private string _Username;
        private string _ItemInfoType;
    }

    internal class ManagerViewModel : BaseVM {
        public ManagerViewModel() {
            AddContactVM = new AddContactViewModel();
            InvitationsVM = new InvitationsViewModel();

            PendingInvitations = new ObservableCollection<ContactManagerItem>();
            ReceivedInvitations = new ObservableCollection<ContactManagerItem>();

            ShowPendingInvitationsButtonCommand = new RelayCommand(o => {
                InvitationsVM.CurrentList = PendingInvitations;
                CurrentView = InvitationsVM;
            });

            ShowReceivedInvitationsButtonCommand = new RelayCommand(o => {
                InvitationsVM.CurrentList = ReceivedInvitations;
                CurrentView = InvitationsVM;
            });

            AddContactViewButtonCommand = new RelayCommand(o => {
                AddContactVM.NotificationText = null;
                CurrentView = AddContactVM;
            });

            Client.Instance.SendRequest(new GetInvitationsRequest());
        }

        // VM's
        public AddContactViewModel AddContactVM { get; private set; }
        public InvitationsViewModel InvitationsVM { get; private set; }

        // Commands
        public RelayCommand ShowFriendListButtonCommand { get; private set; }
        public RelayCommand ShowPendingInvitationsButtonCommand  { get; private set; }
        public RelayCommand ShowReceivedInvitationsButtonCommand { get; private set; }
        public RelayCommand AddContactViewButtonCommand { get; private set; }

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

        public ObservableCollection<ContactManagerItem> ReceivedInvitations {
            get {
                return _ReceivedInvitations;
            }
            set {
                _ReceivedInvitations = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ContactManagerItem> _PendingInvitations;
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
    }

}