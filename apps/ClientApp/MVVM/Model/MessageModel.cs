using ClientApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.MVVM.Model
{
    internal class MessageModel : ObservableObject
    {
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
    }
}
