using ClientApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.MVVM.ViewModel.PIWindowViewModel
{
    internal class PIWindowViewModel : ObservableObject
    {
        PIWindowSettingsViewModel SettingsViewModel { get; set; } 
        public PIWindowViewModel()
        {
            SettingsViewModel = new PIWindowSettingsViewModel();
            CurrentView = SettingsViewModel;
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
