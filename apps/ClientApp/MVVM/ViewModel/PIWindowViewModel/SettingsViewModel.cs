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

        // Current View
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