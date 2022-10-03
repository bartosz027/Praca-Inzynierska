using System.Windows.Media.Imaging;

using ClientApp.Core;
using ClientApp.Core.Services.DialogService;

namespace ClientApp.MVVM.ViewModel.Contacts.Chat
{
    internal class CallMessageBoxViewModel : DialogViewModelBase<DialogResults>
    {
        public CallMessageBoxViewModel() : base()
        {
            YesCommand = new RelayCommand(o => 
            {
                CloseDialogWithResult((o as IDialogWindow), DialogResults.Yes);
            });

            NoCommand = new RelayCommand(o => 
            {
                CloseDialogWithResult((o as IDialogWindow), DialogResults.No);
            });
        }

        public void SetInfo(BitmapImage friendImageSource, string friendUsername) 
        {
            FriendImageSource = friendImageSource;
            FriendUsername = friendUsername;    
        }

        public RelayCommand YesCommand { get; set; }
        public RelayCommand NoCommand { get; set; }

        public BitmapImage FriendImageSource
        {
            get
            {
                return _friendImageSource;
            }
            set
            {
                _friendImageSource = value;
                OnPropertyChanged();
            }
        }
        private BitmapImage _friendImageSource;

        public string FriendUsername
        {
            get
            {
                return _friendUsername;
            }
            set
            {
                _friendUsername = value;
                OnPropertyChanged();
            }
        }
        private string _friendUsername;
    }

}