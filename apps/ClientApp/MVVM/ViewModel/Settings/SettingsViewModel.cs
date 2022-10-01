using System.Windows.Media.Imaging;

using ClientApp.Core;
using ClientApp.MVVM.ViewModel.Settings.Options;

using Network.Client;
using Network.Client.DataProcessing;

using Network.Shared.DataTransfer.Model.Account.Logout;
using Network.Shared.DataTransfer.Model.Database.Friends.GetAvatar;

namespace ClientApp.MVVM.ViewModel.Settings {

    internal class SettingsViewModel : BaseVM {
        public SettingsViewModel() {
            EnableResponseListener();

            AccountSettingVM = new AccountSettingsViewModel();
            ProfileSettingVM = new ProfileSettingViewModel();

            ThemeSettingsVM = new ThemesSettingViewModel();
            LanguageSettingsVM = new LanguageSettingsViewModel();

            AccountOptionCommand = new RelayCommand(o => {
                CurrentView = AccountSettingVM;
            });

            ProfileOptionCommand = new RelayCommand(o => {
                ProfileSettingVM.SetMockData(Username, UserID, UserImage);
                CurrentView = ProfileSettingVM;
            });

            ThemeOptionCommand = new RelayCommand(o => {
                CurrentView = ThemeSettingsVM;
            });

            LanguageOptionCommand = new RelayCommand(o => {
                CurrentView = LanguageSettingsVM;
            });

            LogoutOptionCommand = new RelayCommand(o => {
                Client.Instance.SendRequest(new LogoutRequest());
            });

            UserID = "UID: " + Client.Data.ID.ToString("000000000");
            Username = Client.Data.Username;

            ProfileSettingVM.SetUpdateInferfaceCallback((username, image) => {
                Username = username;
                UserImage = image;
            });

            Client.Instance.SendRequest(new GetAvatarRequest());
        }

        // VM's
        public AccountSettingsViewModel AccountSettingVM { get; private set; }
        public ProfileSettingViewModel ProfileSettingVM { get; private set; }

        public ThemesSettingViewModel ThemeSettingsVM { get; private set; }
        public LanguageSettingsViewModel LanguageSettingsVM { get; private set; }

        // Commands
        public RelayCommand AccountOptionCommand { get; private set; }
        public RelayCommand ProfileOptionCommand { get; private set; }

        public RelayCommand ThemeOptionCommand { get; private set; }
        public RelayCommand LanguageOptionCommand { get; private set; }

        public RelayCommand LogoutOptionCommand { get; private set; }

        // Properties
        public string UserID {
            get {
                return _UserID;
            }
            set {
                _UserID = value;
                OnPropertyChanged();
            }
        }

        public string Username { 
            get {
                return _Username;
            }
            set {
                _Username = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage UserImage { 
            get {
                return _UserImage;
            }
            set {
                _UserImage = value;
                OnPropertyChanged();
            }
        }

        private string _UserID, _Username;
        private BitmapImage _UserImage;

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

        // Response events
        protected override void OnResponseReceived(ResponseDispatcher dispatcher) {
            dispatcher.Dispatch<GetAvatarResponse>(OnGetAvatarResponse);
        }

        private void OnGetAvatarResponse(GetAvatarResponse response) {
            UserImage = ImageLoader.Load(response.ImageBytes);
            ProfileOptionCommand.Execute(null);
        }
    }

}