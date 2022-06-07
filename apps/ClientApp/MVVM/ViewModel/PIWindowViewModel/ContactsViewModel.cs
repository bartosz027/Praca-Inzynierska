using ClientApp.Core;
using ClientApp.MVVM.Model;
using ClientApp.MVVM.ViewModel.PIWindowViewModel.ContactsViewModels;
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
            FriendList = new ObservableCollection<FriendModel> { new FriendModel { Username = "DzikiSzczur", Status= true },
                                                                 new FriendModel { Username = "Jaszczomb", Status= true },
                                                                 new FriendModel { Username = "LeniwyMalpiszon", Status= true },
                                                                 new FriendModel { Username = "KrzysJanuszek", Status= false }
            };
               ChatViewModel = new ChatViewModel();
            CurrentView = ChatViewModel;
        }
        // VM's
        public ChatViewModel ChatViewModel { get; set; }

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
        public ObservableCollection<FriendModel> FriendList { get; set; }
        public FriendModel SelecteFriend
        {
            get { return _SelecteFriend; }
            set
            {
                _SelecteFriend = value;
                OnPropertyChanged();
            }
        }
        private FriendModel _SelecteFriend;
    }
}
