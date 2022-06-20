using ClientApp.Core;
using ClientApp.MVVM.Model;
using Network.Client;
using Network.Shared.DataTransfer.Model.Friends.AcceptFriendInvitation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.MVVM.ViewModel.PIWindowViewModel.ContactsViewModels
{
    internal class NotificationListViewModel : ObservableObject
    {
        public NotificationListViewModel()
        {
            CurrentList = new ObservableCollection<ContactManagerItemModel>();

            AcceptButtonCommand = new RelayCommand(o => 
            {
                var item = o as ContactManagerItemModel;

                Client.Instance.SendRequest(new AcceptFriendInvitationRequest() {
                    Username = item.Username
                });
                
                CurrentList.Remove(item);
            });

            DeclineButtonCommand = new RelayCommand(o =>
            {
                var item = o as ContactManagerItemModel;
                //sacharoza
                CurrentList.Remove(item);
            });
        }

        // Commands
        public RelayCommand AcceptButtonCommand { get; set; }
        public RelayCommand DeclineButtonCommand { get; private set; }

        // Observable properties
        public string ContactName 
        {
            get { return _ContactName; }
            set 
            {
                _ContactName = value;
                OnPropertyChanged();
            }
        }
        private string _ContactName;

        public ObservableCollection<ContactManagerItemModel> CurrentList
        {
            get { return _CurrentList; }
            set
            {
                _CurrentList = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ContactManagerItemModel> _CurrentList;
    }
}