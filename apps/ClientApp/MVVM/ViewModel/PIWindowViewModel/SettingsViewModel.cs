using ClientApp.Core;
using ClientApp.MVVM.ViewModel.PIWindowViewModel.SettingsViewModels;

namespace ClientApp.MVVM.ViewModel.PIWindowViewModel
{
    internal class SettingsViewModel : ObservableObject
    {
        public SettingsViewModel()
        {
            ThemesSettingViewModel = new ThemesSettingViewModel();
            ThemesSettingCommand = new RelayCommand(o => 
            {
                CurrentView = ThemesSettingViewModel;
            });
        }

        // VM's
        public ThemesSettingViewModel ThemesSettingViewModel { get; set; }

        // Commands
        public RelayCommand ThemesSettingCommand { get; set; }

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