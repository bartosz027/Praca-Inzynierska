using ClientApp.Core;
using System.Collections.ObjectModel;

using Network.Client;
using Network.Client.DataProcessing;

using Network.Shared.DataTransfer.Base;
using Network.Shared.DataTransfer.Model.Friends.ManageInvitations.SendFriendInvitation;

namespace ClientApp.MVVM.ViewModel.Contacts.Manager {

    internal class AddContactViewModel : BaseVM {
        public AddContactViewModel(ObservableCollection<ContactManagerItem> invitations) {
            EnableResponseListener();

            SendInvitatationButtonCommand = new RelayCommand(o => {
                var digits_only = true;
                var greater_than_zero = false;

                foreach (char c in ContactID) {
                    if (c >= '1' && c <= '9') {
                        greater_than_zero = true;
                    }

                    if (c < '0' || c > '9') {
                        digits_only = false;
                        break;
                    }
                }

                if (digits_only == true && greater_than_zero == true && ContactID.Length == 9) {
                    var id = ContactID.TrimStart('0');

                    Client.Instance.SendRequest(new SendFriendInvitationRequest() {
                        UserID = int.Parse(id)
                    });
                }
                else {
                    ErrorMessage = "InvalidID";
                }
            });

            PendingInvitations = invitations;
        }

        // Commands
        public RelayCommand SendInvitatationButtonCommand { get; private set; }

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
        private string _ContactID = "";

        public string ErrorMessage {
            get { 
                return _ErrorMessage; 
            }
            set {
                _ErrorMessage = value;
                OnPropertyChanged();
            }
        }
        private string _ErrorMessage = "";

        public ObservableCollection<ContactManagerItem> PendingInvitations {
            get {
                return _PendingInvitations;
            }
            set {
                _PendingInvitations = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ContactManagerItem> _PendingInvitations;

        // Response events
        protected override void OnResponseReceived(ResponseDispatcher dispatcher) {
            dispatcher.Dispatch<SendFriendInvitationResponse>(OnSendFriendInvitationResponse);
        }

        private void OnSendFriendInvitationResponse(SendFriendInvitationResponse response) {
            // TODO: Implement error codes

            switch (response.Result) {
                case ResponseResult.Success: {
                    PendingInvitations.Add(new ContactManagerItem() {
                        UserID = response.UserID,
                        Username = response.Username,

                        IsEnabledAcceptOption = false,
                        IsEnabledDeclineOption = true,

                        ItemInfoType = "PendingInvitation"
                    });

                    ErrorMessage = "Success";
                    break;
                }
                case ResponseResult.Failure: {
                    ErrorMessage = "Failure";
                    break;
                }
            }
        }
    }

}