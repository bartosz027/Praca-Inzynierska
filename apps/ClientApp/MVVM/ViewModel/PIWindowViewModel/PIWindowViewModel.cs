using ClientApp.Core;

namespace ClientApp.MVVM.ViewModel.PIWindowViewModel
{
    internal class PIWindowViewModel : ObservableObject
    {
        public PIWindowViewModel()
        {
            SettingsViewModel = new SettingsViewModel();
            ContactsViewModel = new ContactsViewModel();

            ContactsButtonCommand = new RelayCommand(o => 
            {
                CurrentView = ContactsViewModel;
            });

            SettingsButtonCommand = new RelayCommand(o => 
            {
                CurrentView = SettingsViewModel;
            });

            CurrentView = ContactsViewModel;
        }

        // VM's
        public SettingsViewModel SettingsViewModel { get; set; }
        public ContactsViewModel ContactsViewModel { get; set; }

        // Commands
        public RelayCommand ContactsButtonCommand { get; set; }
        public RelayCommand SettingsButtonCommand { get; set; }

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