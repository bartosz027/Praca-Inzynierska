using ClientApp.Core;
using Network.Shared.Core.Audio;

using ClientApp.MVVM.ViewModel.Contacts;
using ClientApp.MVVM.ViewModel.Settings;

namespace ClientApp.MVVM.ViewModel {

    internal class MainWindowViewModel : BaseVM {
        public MainWindowViewModel() {
            ContactsVM = new ContactsViewModel();
            SettingsVM = new SettingsViewModel();

            Audio.MuteMicrophone(false);
            Audio.MuteHeadphones(false);

            ContactsButtonCommand = new RelayCommand(o => {
                CurrentView = ContactsVM;
            });

            SettingsButtonCommand = new RelayCommand(o => {
                ContactsVM.ContactManagerButtonCommand.Execute(null);
                SettingsVM.ProfileOptionCommand.Execute(null);
                CurrentView = SettingsVM;
            });

            MuteMicrophoneButtonCommand = new RelayCommand(o => {
                Audio.MuteMicrophone();
            });

            MuteHeadphonesButtonCommand = new RelayCommand(o => {
                Audio.MuteHeadphones();
            });

            CurrentView = ContactsVM;
        }

        // VM's
        public ContactsViewModel ContactsVM { get; private set; }
        public SettingsViewModel SettingsVM { get; private set; }

        // Commands
        public RelayCommand ContactsButtonCommand { get; private set; }
        public RelayCommand SettingsButtonCommand { get; private set; }

        public RelayCommand MuteMicrophoneButtonCommand { get; private set; }
        public RelayCommand MuteHeadphonesButtonCommand { get; private set; }

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