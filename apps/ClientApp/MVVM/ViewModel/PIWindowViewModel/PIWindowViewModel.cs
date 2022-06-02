using ClientApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.MVVM.ViewModel.PIWindowViewModel
{
    internal class PIWindowViewModel : ObservableObject
    {
        public RelayCommand ContactsButtonCommand { get; set; }
        public RelayCommand SettingsButtonCommand { get; set; }
        SettingsViewModel SettingsViewModel { get; set; } 
        ContactsViewModel ContactsViewModel { get; set; }
        public PIWindowViewModel()
        {
            SettingsViewModel = new SettingsViewModel();
            ContactsViewModel = new ContactsViewModel();
            CurrentView = ContactsViewModel;

            ContactsButtonCommand = new RelayCommand(o => 
            {
                CurrentView = ContactsViewModel;
            });
            SettingsButtonCommand = new RelayCommand(o =>
            {
                CurrentView = SettingsViewModel;
            });
        }
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
