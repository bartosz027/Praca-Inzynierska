using ClientApp.Core;

namespace ClientApp.MVVM.Model
{
    internal class MessageModel : ObservableObject
    {
        public int ID 
        {
            get { return _ID; }
            set 
            {
                _ID = value;
                OnPropertyChanged();
            }
        }
        private int _ID;

        public string Content
        {
            get { return _Content; }
            set
            {
                _Content = value;
                OnPropertyChanged();
            }
        }
        private string _Content;

        public string Date
        {
            get { return _Date; }
            set
            {
                _Date = value;
                OnPropertyChanged();
            }
        }
        private string _Date;

        public string Sender
        {
            get { return _Sender; }
            set
            {
                _Sender = value;
                OnPropertyChanged();
            }
        }
        private string _Sender;

        public bool IsMyMessage
        {
            get { return _IsMyMessage; }
            set
            {
                _IsMyMessage = value;
                OnPropertyChanged();
            }
        }
        private bool _IsMyMessage;
    }
}