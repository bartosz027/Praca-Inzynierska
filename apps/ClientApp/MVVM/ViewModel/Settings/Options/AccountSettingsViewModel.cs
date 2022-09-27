using ClientApp.Core;
using Network.Client;

namespace ClientApp.MVVM.ViewModel.Settings.Options
{
    internal class AccountSettingsViewModel : ObservableObject
    {
        public AccountSettingsViewModel()
        {
            Email = Client.Data.Email;
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
