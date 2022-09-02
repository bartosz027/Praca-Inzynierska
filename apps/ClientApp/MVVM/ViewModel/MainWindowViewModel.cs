using ClientApp.Core;

using ClientApp.MVVM.ViewModel.Contacts;
using ClientApp.MVVM.ViewModel.Settings;

namespace ClientApp.MVVM.ViewModel {

    internal class MainWindowViewModel : BaseViewModel {
        public MainWindowViewModel() {
            ContactsVM = new ContactsViewModel();
            SettingsVM = new SettingsViewModel();

            ContactsButtonCommand = new RelayCommand(o => {
                CurrentView = ContactsVM;
            });

            SettingsButtonCommand = new RelayCommand(o => {
                CurrentView = SettingsVM;
            });

            CurrentView = ContactsVM;
        }

        // VM's
        public ContactsViewModel ContactsVM { get; set; }
        public SettingsViewModel SettingsVM { get; set; }

        // Commands
        public RelayCommand ContactsButtonCommand { get; set; }
        public RelayCommand SettingsButtonCommand { get; set; }

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