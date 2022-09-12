using ClientApp.Core;
using ClientApp.Resources;

using Network.Client;
using Network.Shared.DataTransfer.Model.Friends.ManageInvitations.SendFriendInvitation;

namespace ClientApp.MVVM.ViewModel.Contacts.Manager {

    internal class AddContactViewModel : BaseVM {
        public AddContactViewModel() {
            SendInvitationButtonCommand = new RelayCommand(o => {
                if (ContactID.Length == 9) {
                    Client.Instance.SendRequest(new SendFriendInvitationRequest() {
                        UserID = int.Parse(ContactID)
                    });
                }
                else {
                    ErrorMessage = ResourcesDictionary.Warning;
                    NotificationText = ResourceManager.GetValue(ResourcesDictionary.AddFriendInvalidID);
                }
            });
        }

        // Commands
        public RelayCommand SendInvitationButtonCommand { get; private set; }

        // Properties
        public string ContactID {
            get { 
                return _ContactID; 
            }
            set {
                _ContactID = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage {
            get { 
                return _ErrorMessage; 
            }
            set {
                _ErrorMessage = value;
                OnPropertyChanged();
            }
        }

        public string NotificationText {
            get {
                return _NotificationText;
            }
            set {
                _NotificationText = value;
                OnPropertyChanged();
            }
        }

        private string _ContactID = "";
        private string _ErrorMessage = "";
        private string _NotificationText = "";
    }

}