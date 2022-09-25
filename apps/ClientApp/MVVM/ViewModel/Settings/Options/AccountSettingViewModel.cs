using ClientApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.MVVM.ViewModel.Settings.Options
{
    internal class AccountSettingViewModel : ObservableObject
    {
        public AccountSettingViewModel()
        {
            Email = "example@example.com";
        }
        public string Email 
        {
            get { return _email; }
            set 
            {
                _email = value;
                OnPropertyChanged();
            }
        }
        private string _email;
    }
}
