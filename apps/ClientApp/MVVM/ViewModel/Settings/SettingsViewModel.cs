using ClientApp.Core;
using ClientApp.MVVM.ViewModel.Settings.Options;

using Network.Client;
using Network.Shared.DataTransfer.Model.Account.Logout;

namespace ClientApp.MVVM.ViewModel.Settings {

    internal class SettingsViewModel : BaseVM {
        public SettingsViewModel() {
            ThemesSettingVM = new ThemesSettingViewModel();
            LanguageSettingsVM = new LanguageSettingsViewModel();

            ThemeOptionCommand = new RelayCommand(o => {
                CurrentView = ThemesSettingVM;
            });

            LanguageOptionCommand = new RelayCommand(o => {
                CurrentView = LanguageSettingsVM;
            });

            LogoutOptionCommand = new RelayCommand(o => {
                Client.Instance.SendRequest(new LogoutRequest());
            });
        }

        // VM's
        public ThemesSettingViewModel ThemesSettingVM { get; private set; }
        public LanguageSettingsViewModel LanguageSettingsVM { get; private set; }

        // Commands
        public RelayCommand ProfileOptionCommand { get; private set; }
        public RelayCommand AccountOptionCommand { get; private set; }

        public RelayCommand ThemeOptionCommand { get; private set; }
        public RelayCommand LanguageOptionCommand { get; private set; }

        public RelayCommand LogoutOptionCommand { get; private set; }

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