using ClientApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.MVVM.ViewModel.PIWindowViewModel.ContactsViewModels
{
    internal class AddContactViewModel : ObservableObject
    {
        public AddContactViewModel()
        {
            SendInvitatationButtonCommand = new RelayCommand(o => 
            {
                //sacharoza            
            });
        }

        // Commands
        public RelayCommand SendInvitatationButtonCommand { get; set; }

        // Observable properties
        public string ContactName
        {
            get { return _ContactName; }
            set
            {
                _ContactName = value;
                OnPropertyChanged();
            }
        }
        private string _ContactName;
    }
}
