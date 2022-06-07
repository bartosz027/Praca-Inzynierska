using ClientApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.MVVM.Model
{
    internal class FriendModel : ObservableObject
    {
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
