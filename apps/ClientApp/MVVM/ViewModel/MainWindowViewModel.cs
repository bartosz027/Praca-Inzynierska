using System.Windows;

using ClientApp.Core;
using Network.Client;

using ClientApp.MVVM.ViewModel.Contacts;
using ClientApp.MVVM.ViewModel.Settings;

using Network.Shared.Core.Audio;
using Network.Shared.DataTransfer.Model.Database.Friends.SetStatusRequest;

namespace ClientApp.MVVM.ViewModel {

    internal class MainWindowViewModel : BaseVM {
        public MainWindowViewModel() {
            ContactsVM = new ContactsViewModel();
            SettingsVM = new SettingsViewModel();

            Audio.MuteMicrophone(false);
            Audio.MuteHeadphones(false);

            Status = Client.Data.Status;

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

            AvailableStatusCommand = new RelayCommand(o => 
            {
                if (Status == false) {
                    Client.Instance.SendRequest(new SetStatusRequest() {
                        ID = Client.Data.ID,
                        Status = true
                    });

                    Status = true;
                }
            });

            InvisibleStatusCommand = new RelayCommand(o => 
            {
                if (Status == true) {
                    Client.Instance.SendRequest(new SetStatusRequest() {
                        ID = Client.Data.ID,
                        Status = false
                    });

                    Status = false;
                }
            });

            CopyIDCommand = new RelayCommand(o => 
            {
                Clipboard.SetText(SettingsVM.UserID);
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

        public RelayCommand AvailableStatusCommand { get; private set; }
        public RelayCommand InvisibleStatusCommand { get; private set; }
        public RelayCommand CopyIDCommand { get; private set; }
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

        public bool Status
        {
            get
            {
                return _Status;
            }
            set
            {
                _Status = value;
                OnPropertyChanged();
            }
        }
        private bool _Status;
    }

}