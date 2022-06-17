using ClientApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.MVVM.Model
{
    internal class ContactManagerItemModel : ObservableObject
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
        public bool IsEnabledAcceptOption
        {
            get { return _IsEnabledAcceptOption; }
            set
            {
                _IsEnabledAcceptOption = value;
                OnPropertyChanged();
            }
        }
        private bool _IsEnabledAcceptOption;

        public bool IsEnabledDeclineOption
        {
            get { return _IsEnabledDeclineOption; }
            set
            {
                _IsEnabledDeclineOption = value;
                OnPropertyChanged();
            }
        }
        private bool _IsEnabledDeclineOption;
    }
}
