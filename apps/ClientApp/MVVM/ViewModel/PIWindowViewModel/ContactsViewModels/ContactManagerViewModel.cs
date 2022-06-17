using ClientApp.Core;
using System;
using System.Collections.Generic;
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
            CurrentView = NotificationListVM;

            AddContactViewButtonCommand = new RelayCommand(o=>
            {
                CurrentView = AddContactVM;
            });
            PendingInvitationsButtonCommand = new RelayCommand(o =>
            {
                CurrentView = NotificationListVM;
            });
            InvitationsButtonCommand = new RelayCommand(o =>
            {
                CurrentView = NotificationListVM;
            });
            BlockedButtonCommand = new RelayCommand(o =>
            {
                CurrentView = NotificationListVM;
            });
        }

        // VM' s
        NotificationListViewModel NotificationListVM { get; set; }
        AddContactViewModel AddContactVM { get; set; }

        // Obserable properties


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
