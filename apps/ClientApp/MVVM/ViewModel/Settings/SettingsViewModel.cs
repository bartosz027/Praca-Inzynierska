using ClientApp.Core;
using ClientApp.MVVM.ViewModel.Settings.Options;

namespace ClientApp.MVVM.ViewModel.Settings {

    internal class SettingsViewModel : BaseViewModel {
        public SettingsViewModel() {
            ThemesSettingVM = new ThemesSettingViewModel();
            LanguageSettingsVM = new LanguageSettingsViewModel();

            ThemesSettingCommand = new RelayCommand(o => {
                CurrentView = ThemesSettingVM;
            });

            LanguageSettingCommand = new RelayCommand(o => {
                CurrentView = LanguageSettingsVM;
            });
        }

        // VM's
        public ThemesSettingViewModel ThemesSettingVM { get; set; }
        public LanguageSettingsViewModel LanguageSettingsVM { get; set; }

        // Commands
        public RelayCommand ThemesSettingCommand { get; set; }
        public RelayCommand LanguageSettingCommand { get; set; }

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