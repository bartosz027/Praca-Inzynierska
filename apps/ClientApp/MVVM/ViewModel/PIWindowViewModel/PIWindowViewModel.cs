using ClientApp.Core;

namespace ClientApp.MVVM.ViewModel.PIWindowViewModel
{
    internal class PIWindowViewModel : ObservableObject
    {
        public PIWindowViewModel()
        {
            SettingsVM = new SettingsViewModel();
            ContactsVM = new ContactsViewModel();

            ContactsButtonCommand = new RelayCommand(o => 
            {
                CurrentView = ContactsVM;
            });

            SettingsButtonCommand = new RelayCommand(o => 
            {
                CurrentView = SettingsVM;
            });

            CurrentView = ContactsVM;
        }

        // VM's
        public SettingsViewModel SettingsVM { get; set; }
        public ContactsViewModel ContactsVM { get; set; }

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