using System;
using System.Windows.Media.Imaging;

using ClientApp.Core;
using ClientApp.Resources;

using Microsoft.Win32;
using Network.Shared.Core;

namespace ClientApp.MVVM.ViewModel.Settings.Options
{
    internal class ProfileSettingViewModel : ObservableObject
    {
        public ProfileSettingViewModel()
        {

            IsValidToSave = false;
            string fileName;

            AddAvatarButtonCommand = new RelayCommand(o => 
            {
                 var openFileDialog = new OpenFileDialog();
                 openFileDialog.Filter = "Files|*.jpg;*.jpeg;*.png;";
                 openFileDialog.Multiselect = false;
                 
                 var result = openFileDialog.ShowDialog();
                 BitmapImage bi = new BitmapImage();
                 if (result == true)
                 {
                     fileName = openFileDialog.FileName;
                     bi.BeginInit();
                     bi.UriSource = new Uri(fileName, UriKind.RelativeOrAbsolute);
                     bi.EndInit();
                 }
                MockImageSource = bi;
                isAvatarChanged = true;
                if (MockUsername.Length >= 2)
                {
                    IsValidToSave = true;
                }
            });

            SaveChangesButtonCommand = new RelayCommand(o =>
            {
                // Avatar = MockImageSource
            });
        }

        private string OrginalUsername;
        public string MockUsername 
        { 
            get { return _mockUsername; }
            set 
            {
                _mockUsername = value;
                OnPropertyChanged();
                IsValidToSave = ((isAvatarChanged || _mockUsername != OrginalUsername) && _mockUsername.Length >= 2);
                if (_mockUsername.Length < 2)
                {
                    ErrorMessage = ResourceManager.GetValue(ResourcesDictionary.InvalidUsername, Values.MinUsernameLength, Values.MaxUsernameLength);
                }
                else
                {
                    ErrorMessage = "";
                }
            } 
        }
        private string _mockUsername;
        public string UserID
        {
            get { return _userID; }
            set
            {
                _userID = value;
                OnPropertyChanged();
            }
        }
        private string _userID;
        public BitmapImage MockImageSource
        {
            get { return _imageSource; }
            set
            {
                _imageSource = value;
                OnPropertyChanged();
            }
        }
        private BitmapImage _imageSource;

        public bool IsValidToSave 
        {
            get { return _isValidToSave; }
            set 
            {
                _isValidToSave = value;
                OnPropertyChanged();
            }
        }
        private bool _isValidToSave;
        private bool isAvatarChanged;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged();;
            }
        }
        private string _errorMessage;

        // Commands
        public RelayCommand AddAvatarButtonCommand { get; set;}
        public RelayCommand SaveChangesButtonCommand { get; set; }

        public void SetMockData(string username, string userid, BitmapImage mockImageSource) 
        {
            OrginalUsername = username;
            UserID = userid;
            MockImageSource = mockImageSource;
            MockUsername = username;
        }
    }
}
