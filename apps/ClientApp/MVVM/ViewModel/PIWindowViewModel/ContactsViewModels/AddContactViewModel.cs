using Network.Client;
using Network.Shared.DataTransfer.Model.Friends.AddFriend;

using ClientApp.Core;
using System.Collections.ObjectModel;
using ClientApp.MVVM.Model;

namespace ClientApp.MVVM.ViewModel.PIWindowViewModel.ContactsViewModels
{
    internal class AddContactViewModel : ObservableObject
    {
        public AddContactViewModel(ObservableCollection<ContactManagerItemModel> pending)
        {
            Pending = pending;

            SendInvitatationButtonCommand = new RelayCommand(o => 
            {
                Client.Instance.SendRequest(new AddFriendRequest() {
                    Username = ContactName
                });

                Pending.Add(new ContactManagerItemModel() {
                    Username = ContactName,
                    IsEnabledAcceptOption = false,
                    IsEnabledDeclineOption = true,
                    ItemInfo = "Pending"
                });;
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

        public ObservableCollection<ContactManagerItemModel> Pending { get; set; }
    }
}