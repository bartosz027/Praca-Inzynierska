using ClientApp.Core;
using ClientApp.MVVM.ViewModel.PIWindowViewModel.ContactsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.MVVM.ViewModel.PIWindowViewModel
{
    internal class ContactsViewModel : ObservableObject
    {
        public ContactsViewModel()
        {
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
    }
}
