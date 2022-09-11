using ClientApp.Core;
using ClientApp.MVVM.ViewModel.Contacts;
using ClientApp.MVVM.ViewModel.Settings;

namespace ClientApp.MVVM.ViewModel {

    internal class MainWindowViewModel : BaseVM {
        public MainWindowViewModel() {
            EnableNotificationListener();

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
        public ContactsViewModel ContactsVM 
        {
            get { return _ContactsVM ; }
            private set 
            {
                _ContactsVM = value;
                OnPropertyChanged();
            } 
        }
        private ContactsViewModel _ContactsVM;
        public SettingsViewModel SettingsVM { get; private set; }


        // Commands
        public RelayCommand ContactsButtonCommand { get; private set; }
        public RelayCommand SettingsButtonCommand { get; private set; }

        //properties
     
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