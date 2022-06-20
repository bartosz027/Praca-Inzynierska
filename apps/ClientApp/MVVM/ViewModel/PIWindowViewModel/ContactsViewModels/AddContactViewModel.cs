using Network.Client;
using Network.Shared.DataTransfer.Model.Friends.AddFriend;

using ClientApp.Core;
using System.Collections.ObjectModel;
using ClientApp.MVVM.Model;
using Network.Shared.DataTransfer.Base;
using System;
using Network.Client.DataProcessing;

namespace ClientApp.MVVM.ViewModel.PIWindowViewModel.ContactsViewModels
{
    internal class AddContactViewModel : ObservableObject
    {
        public AddContactViewModel(ObservableCollection<ContactManagerItemModel> pending)
        {
            Client.Instance.ResponseReceived += OnResponseReceived;
            Pending = pending;

            SendInvitatationButtonCommand = new RelayCommand(o => 
            {
                if (!String.IsNullOrEmpty(ContactName))
                {
                    Client.Instance.SendRequest(new AddFriendRequest()
                    {
                        Username = ContactName
                    });
                }
                else
                    ErrorMessage = "Invalid";
            });
        }

        // Commands
        public RelayCommand SendInvitatationButtonCommand { get; set; }

        //Common  properties

        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set
            {
                _ErrorMessage = value;
                OnPropertyChanged();
            }
        }
        private string _ErrorMessage;

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

        // Response Event Handling
        private void OnResponseReceived(object sender, Response response)
        {
            var dispatcher = new ResponseDispatcher(response);

            App.Current.Dispatcher.Invoke(delegate {
                dispatcher.Dispatch<AddFriendResponse>(OnAddFriendResponse);
            });
        }

        private void OnAddFriendResponse(AddFriendResponse response)
        {
            switch (response.Status) 
            {
                case STATUS.SUCCESS: 
                {
                    Pending.Add(new ContactManagerItemModel()
                    {
                        Username = ContactName,
                        IsEnabledAcceptOption = false,
                        IsEnabledDeclineOption = true,
                        ItemInfo = "Pending"
                    }); ;

                    ErrorMessage = "Success";
                    break;
                }
                case STATUS.FAILURE:
                {
                    ErrorMessage = "Failure";
                    break;
                }
            }
            
        }
    }
}