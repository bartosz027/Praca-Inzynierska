using ClientApp.Core;
using System.Collections.ObjectModel;

using Network.Client;
using Network.Shared.DataTransfer.Model.Friends.ManageInvitations.AcceptFriendInvitation;

namespace ClientApp.MVVM.ViewModel.Contacts.Manager {

    internal class InvitationsViewModel : BaseVM {
        public InvitationsViewModel() {
            AcceptButtonCommand = new RelayCommand(o => {
                var item = o as ContactManagerItem;

                Client.Instance.SendRequest(new AcceptFriendInvitationRequest() {
                    UserID = item.UserID
                });
            });

            DeclineButtonCommand = new RelayCommand(o => {
                // TODO: Implement invitation decline
            });
        }

        // Commands
        public RelayCommand AcceptButtonCommand { get; private set; }
        public RelayCommand DeclineButtonCommand { get; private set; }

        // Properties
        public string ContactName {
            get { 
                return _ContactName; 
            }
            set {
                _ContactName = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ContactManagerItem> CurrentList {
            get { 
                return _CurrentList; 
            }
            set {
                _CurrentList = value;
                OnPropertyChanged();
            }
        }

        private string _ContactName;
        private ObservableCollection<ContactManagerItem> _CurrentList;
    }

}