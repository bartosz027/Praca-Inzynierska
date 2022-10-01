using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

using ClientApp.Core;
using ClientApp.Resources;

using Microsoft.Win32;
using Network.Client;

using Network.Shared.Core;
using Network.Shared.DataTransfer.Model.Settings.ChangeAvatar;
using Network.Shared.DataTransfer.Model.Settings.ChangeUsername;

namespace ClientApp.MVVM.ViewModel.Settings.Options
{
    internal class ProfileSettingViewModel : ObservableObject
    {
        public ProfileSettingViewModel()
        {
            AddAvatarButtonCommand = new RelayCommand(o => 
            {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Files|*.jpg;*.jpeg;*.png;";
                openFileDialog.Multiselect = false;
                
                if (openFileDialog.ShowDialog() == true)
                {
                    var image = Image.FromFile(openFileDialog.FileName);
                    var bitmap_image = ImageResizer.Resize(image, 64, 64);

                    MockImageSource = bitmap_image;
                    isAvatarChanged = true;

                    if (MockUsername.Length >= 2) {
                        IsValidToSave = true;
                    }
                }
            });

            SaveChangesButtonCommand = new RelayCommand(o => {
                if (MockUsername != Client.Data.Username) {
                    Client.Instance.SendRequest(new ChangeUsernameRequest() { 
                        Username = MockUsername
                    });

                    OrginalUsername = MockUsername;
                    Client.Data.Username = MockUsername;
                }

                if (isAvatarChanged) {
                    var encoder = new JpegBitmapEncoder();

                    using (var ms = new MemoryStream()) {
                        encoder.Frames.Add(BitmapFrame.Create(MockImageSource));
                        encoder.Save(ms);

                        Client.Instance.SendRequest(new ChangeAvatarRequest() {
                            UserImage = ms.ToArray()
                        });
                    }

                    isAvatarChanged = false;
                }

                _UpdateInterfaceCallback(MockUsername, MockImageSource);
                IsValidToSave = ((isAvatarChanged || _mockUsername != OrginalUsername) && _mockUsername.Length >= 2);
            });
        }

        public void SetUpdateInferfaceCallback(Action<string, BitmapImage> callback) {
            _UpdateInterfaceCallback = callback;
        }
        private Action<string, BitmapImage> _UpdateInterfaceCallback;

        private string OrginalUsername;
        public string MockUsername 
        { 
            get 
            { 
                return _mockUsername; 
            }
            set 
            {
                _mockUsername = value;
                OnPropertyChanged();

                if (_mockUsername.Length < 2)
                {
                    ErrorMessage = ResourceManager.GetValue(ResourcesDictionary.InvalidUsername, Values.MinUsernameLength, Values.MaxUsernameLength);
                }
                else
                {
                    ErrorMessage = "";
                }

                IsValidToSave = ((isAvatarChanged || _mockUsername != OrginalUsername) && _mockUsername.Length >= 2);
            } 
        }
        private string _mockUsername;
        public string UserID
        {
            get 
            { 
                return _userID; 
            }
            set
            {
                _userID = value;
                OnPropertyChanged();
            }
        }
        private string _userID;
        public BitmapImage MockImageSource
        {
            get 
            { 
                return _imageSource; 
            }
            set
            {
                _imageSource = value;
                OnPropertyChanged();
            }
        }
        private BitmapImage _imageSource;

        public bool IsValidToSave 
        {
            get 
            { 
                return _isValidToSave; 
            }
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
            get 
            { 
                return _errorMessage; 
            }
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
            UserID = userid;
            OrginalUsername = username;

            MockUsername = username;
            MockImageSource = mockImageSource;
        }
    }
}