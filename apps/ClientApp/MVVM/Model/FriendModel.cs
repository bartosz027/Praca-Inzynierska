using ClientApp.Core;

namespace ClientApp.MVVM.Model
{
    internal class FriendModel : ObservableObject
    {
        public int UserID 
        {
            get { return _UserID; }
            set 
            {
                _UserID = value;
                OnPropertyChanged();
            }
        }
        private int _UserID;

        public string Username
        {
            get { return _Username; }
            set
            {
                _Username = value;
                OnPropertyChanged();
            }
        }
        private string _Username;

        public bool Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                OnPropertyChanged();
            }
        }
        private bool _Status;
    }
}