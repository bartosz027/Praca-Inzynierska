using System;
using System.Collections.ObjectModel;

using ClientApp.Core;

using Network.Client;
using Network.Client.DataProcessing;

using Network.Shared.DataTransfer.Base;
using Network.Shared.DataTransfer.Model.Friends.ManageInvitations.SendFriendInvitation;

namespace ClientApp.MVVM.ViewModel.Contacts.Manager {

    internal class AddContactViewModel : BaseViewModel {
        public AddContactViewModel(ObservableCollection<ContactManagerItem> invitations) {
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
                    ErrorMessage = "InvalidIdentifierFormat";
                }
            });

            PendingInvitations = invitations;
        }

        // Commands
        public RelayCommand SendInvitatationButtonCommand { get; set; }

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
        protected override void ResponseReceived(ResponseDispatcher dispatcher) {
            dispatcher.Dispatch<SendFriendInvitationResponse>(OnSendFriendInvitationResponse);
        }

        private void OnSendFriendInvitationResponse(SendFriendInvitationResponse response) {
            switch (response.Result) {
                case Result.Success: {
                    PendingInvitations.Add(new ContactManagerItem() {
                        UserID = response.UserID,
                        Username = response.Username,

                        IsEnabledAcceptOption = false,
                        IsEnabledDeclineOption = true,

                        ItemInfoType = "PendingInvitation"
                    });

                    ErrorMessage = "InvitationSent";
                    break;
                }
                case Result.Failure: {
                    // TODO: Implement error codes
                    ErrorMessage = "Failure";
                    break;
                }
            }
        }
    }

}