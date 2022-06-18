using ClientApp.Core;
using ClientApp.MVVM.Model;
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
            NotificationListVM = new NotificationListViewModel();
            AddContactVM = new AddContactViewModel();

            PendingInvitations = new ObservableCollection<ContactManagerItemModel> {new ContactManagerItemModel { Username = "Mleko", IsEnabledDeclineOption = true, ItemInfo="Wysłałeś tej kurwie zaproszenie"} };
            Invitations = new ObservableCollection<ContactManagerItemModel> { new ContactManagerItemModel { Username = "Kakało", IsEnabledAcceptOption = true, IsEnabledDeclineOption = true, ItemInfo = "Ta kurwa wysłała ci zaproszenie" } };
            Blocked = new ObservableCollection<ContactManagerItemModel> { new ContactManagerItemModel { Username = "Herbapol", IsEnabledDeclineOption = true, ItemInfo = "Ta kurwa jest zablokowana" } };

            CurrentView = NotificationListVM;

            AddContactViewButtonCommand = new RelayCommand(o=>
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
        NotificationListViewModel NotificationListVM { get; set; }
        AddContactViewModel AddContactVM { get; set; }

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
    }
}
