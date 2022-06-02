using ClientApp.Core;
using ClientApp.MVVM.ViewModel.PIWindowViewModel.SettingsViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.MVVM.ViewModel.PIWindowViewModel
{
    internal class SettingsViewModel : ObservableObject
    {
        public ThemesSettingViewModel ThemesSettingViewModel { get; set; }
        public RelayCommand ThemesSettingCommand { get; set; }
        public SettingsViewModel()
        {
            ThemesSettingViewModel = new ThemesSettingViewModel();
            ThemesSettingCommand = new RelayCommand(o => 
            {
                CurrentView = ThemesSettingViewModel;
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
