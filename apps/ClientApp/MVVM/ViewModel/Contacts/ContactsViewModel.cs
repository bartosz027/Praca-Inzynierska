using System;
using System.Linq;
using System.IO;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using ClientApp.Resources;
using System.Windows.Media.Imaging;

using ClientApp.Core;
using ClientApp.Core.Services.DialogService;

using ClientApp.MVVM.ViewModel.Contacts.Chat;
using ClientApp.MVVM.ViewModel.Contacts.Manager;

using Network.Client;
using Network.Client.DataProcessing;

using Network.Shared.Core;
using Network.Shared.DataTransfer.Base;

using Network.Shared.DataTransfer.Model.Account.Login;
using Network.Shared.DataTransfer.Model.Account.Logout;

using Network.Shared.DataTransfer.Model.Database.Friends.GetFriendList;
using Network.Shared.DataTransfer.Model.Database.Friends.GetInvitations;
using Network.Shared.DataTransfer.Model.Database.Friends.GetMessageHistory;

using Network.Shared.DataTransfer.Model.Database.Friends.SetMessageRead;
using Network.Shared.DataTransfer.Model.Database.Friends.SetStatusRequest;

using Network.Shared.DataTransfer.Model.Friends.ManageInvitations.AcceptFriendInvitation;
using Network.Shared.DataTransfer.Model.Friends.ManageInvitations.SendFriendInvitation;

using Network.Shared.DataTransfer.Model.Friends.ManageMessages.DeleteMessage;
using Network.Shared.DataTransfer.Model.Friends.ManageMessages.SendMessage;

using Network.Shared.DataTransfer.Model.Friends.VoiceChat.StartVoiceChat;
using Network.Shared.DataTransfer.Model.Friends.VoiceChat.AcceptVoiceChat;
using Network.Shared.DataTransfer.Model.Friends.VoiceChat.DisconnectVoiceChat;

using Network.Shared.DataTransfer.Model.Friends.ManageMessages.DownloadImage;

using Network.Shared.DataTransfer.Model.Settings.ChangeUsername;
using Network.Shared.DataTransfer.Model.Settings.ChangeAvatar;
using System.Windows;

namespace ClientApp.MVVM.ViewModel.Contacts {

    internal class FriendInfo : ObservableObject {
        public int ID { get; set; }
        public DateTime LastMessageSendDate { get; set; }

        public string GetUID { 
            get {
                return "UID: " + ID.ToString("000000000");
            }
        }

        public string Username {
            get { 
                return _Username; 
            }
            set {
                _Username = value;
                OnPropertyChanged();
            }
        }

        public bool Status {
            get { 
                return _Status; 
            }
            set {
                _Status = value;
                OnPropertyChanged();
            }
        } 

        public bool IsANewMessage {
            get {
                return _IsANewMessage;
            }
            set {
                _IsANewMessage = value;
                OnPropertyChanged();
            }
        }

        public BitmapImage UserImage {
            get {
                return _UserImage;
            }
            set {
                _UserImage = value;
                OnPropertyChanged();
            }
        }

        private string _Username;
        private bool _Status, _IsANewMessage;
        private BitmapImage _UserImage;
    }

    internal class ContactsViewModel : BaseVM {
        public ContactsViewModel() {
            EnableResponseListener();
            _dialogService = new DialogService();

            FriendList = new ObservableCollection<ChatViewModel>();
            UnreadedMessages = new ObservableCollection<int>();

            ContactManagerVM = new ManagerViewModel();
            Client.Instance.SendRequest(new GetFriendListRequest());

            ContactManagerButtonCommand = new RelayCommand(o => {
                SelectedFriend = null;
                CurrentView = ContactManagerVM;
            });

            DeleteFriendCommand = new RelayCommand(o => 
            {
                var chat = o as ChatViewModel;
                MessageBox.Show("Logika: Poziom 1. Strefa LPG imienia:" + chat.FriendInfo.Username);
            });

            EnableNotificationListener();
            CurrentView = ContactManagerVM;
        }

        // Methods
        public void UpdateNotifcationBall() {
            NotificationBall = (UnreadedMessages.Count > 0) || (ContactManagerVM.ReceivedInvitations.Count > 0);
        }

        // Services
        private DialogService _dialogService;

        // VM's
        public ManagerViewModel ContactManagerVM {
            get {
                return _ManagerViewModel;
            }
            private set {
                _ManagerViewModel = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ChatViewModel> FriendList {
            get {
                return _FriendList;
            } 
            private set {
                _FriendList = value;
                OnPropertyChanged();
            }
        }

        private ManagerViewModel _ManagerViewModel;
        private ObservableCollection<ChatViewModel> _FriendList;

        // Commands
        public RelayCommand ContactManagerButtonCommand { get; private set; }

        public RelayCommand DeleteFriendCommand { get; private set; }

        // Properties
        public bool NotificationBall {
            get {
                return _NotificationBall;
            }
            set {
                _NotificationBall = value;
                OnPropertyChanged();
            }
        }

        public ChatViewModel SelectedFriend {
            get {
                return _SelectedFriend;
            }
            set {
                _SelectedFriend = value;

                if (_SelectedFriend != null) {
                    if (_SelectedFriend.Initialized == false) {
                        _SelectedFriend.Init();
                    }

                    if (_SelectedFriend.FriendInfo.IsANewMessage) {
                        Client.Instance.SendRequest(new SetMessageReadRequest() {
                            FriendID = _SelectedFriend.FriendInfo.ID
                        });

                        _SelectedFriend.FriendInfo.IsANewMessage = false;
                        UnreadedMessages.Remove(_SelectedFriend.FriendInfo.ID);

                        UpdateNotifcationBall();
                    }

                    _SelectedFriend.IsFocused = true;
                    CurrentView = _SelectedFriend;
                }

                OnPropertyChanged();
            }
        }

        public ObservableCollection<int> UnreadedMessages {
            get {
                return _UnreadedMessages;
            }
            set {
                _UnreadedMessages = value;
                OnPropertyChanged();
            }
        }

        private bool _NotificationBall;
        private ChatViewModel _SelectedFriend;
        private ObservableCollection<int> _UnreadedMessages;

        // Current view
        public object CurrentView {
            get { 
                return _CurrentView; 
            }
            set {
                _CurrentView = value;
                OnPropertyChanged();
            }
        }
        private object _CurrentView;

        // Response events
        protected override void OnResponseReceived(ResponseDispatcher dispatcher) {
            // Database
            dispatcher.Dispatch<GetFriendListResponse>(OnGetFriendListResponse);
            dispatcher.Dispatch<GetInvitationsResponse>(OnGetInvitationsResponse);
            dispatcher.Dispatch<GetMessageHistoryResponse>(OnGetMessageHistoryResponse);

            // Invitations
            dispatcher.Dispatch<SendFriendInvitationResponse>(OnSendFriendInvitationResponse);
            dispatcher.Dispatch<AcceptFriendInvitationResponse>(OnAcceptFriendInvitationResponse);

            // Text chat
            dispatcher.Dispatch<DownloadImageResponse>(OnDownloadImageResponse);
            dispatcher.Dispatch<DeleteMessageResponse>(OnDeleteMessageResponse);
            dispatcher.Dispatch<SendMessageResponse>(OnSendMessageResponse);

            // Voice chat
            dispatcher.Dispatch<StartVoiceChatResponse>(OnStartVoiceChatResponse);
            dispatcher.Dispatch<AcceptVoiceChatResponse>(OnAcceptVoiceChatResponse);

            // Settings
            dispatcher.Dispatch<ChangeUsernameResponse>(OnChangeUsernameResponse);
        }

        private void OnGetFriendListResponse(GetFriendListResponse response) {
            foreach (var friend in response.FriendList) {
                var friend_info = new FriendInfo() {
                    ID = friend.UserID,
                    Status = friend.Status,
                    Username = friend.Username,
                    UserImage = ImageLoader.Load(friend.ImageBytes)
                };

                if(friend.IsLastMessageRead == false) {
                    friend_info.IsANewMessage = true;
                    UnreadedMessages.Add(friend.UserID);
                }

                friend_info.LastMessageSendDate = friend.LastMessageSendDate;
                FriendList.Add(new ChatViewModel(friend_info));
            }

            var sorted = FriendList.OrderByDescending(p => p.FriendInfo.LastMessageSendDate);
            FriendList = new ObservableCollection<ChatViewModel>(sorted);

            UpdateNotifcationBall();
        }

        private void OnGetInvitationsResponse(GetInvitationsResponse response) {
            foreach (var invitation in response.PendingInvitations) {
                var item = new ContactManagerItem() {
                    UserID = invitation.UserID,
                    Username = invitation.Username,

                    IsEnabledAcceptOption = false,
                    IsEnabledDeclineOption = true,

                    ItemInfoType = "PendingInvitation"
                };

                ContactManagerVM.PendingInvitations.Add(item);
            }

            foreach (var invitation in response.ReceivedInvitations) {
                var item = new ContactManagerItem() {
                    UserID = invitation.UserID,
                    Username = invitation.Username,

                    IsEnabledAcceptOption = true,
                    IsEnabledDeclineOption = true,

                    ItemInfoType = "ReceivedInvitation"
                };

                ContactManagerVM.ReceivedInvitations.Add(item);
            }

            UpdateNotifcationBall();
        }

        private void OnGetMessageHistoryResponse(GetMessageHistoryResponse response) {
            var messages = new List<Chat.MessageInfo>();

            var view_model = FriendList.Single(p => p.FriendInfo.ID == response.FriendID);
            var friend_info = view_model.FriendInfo;

            foreach (var message_info in response.Messages) {
                var message = new Chat.MessageInfo() {
                    ID = message_info.ID,

                    Date = message_info.SendDate.ToLocalTime().ToString("HH:mm"),
                    Content = message_info.Content,

                    Images = new ObservableCollection<BitmapImage>()
                };
                
                if (message_info.SenderID != friend_info.ID) {
                    message.Sender = Client.Data.Username;
                    message.IsMyMessage = true;
                }
                else {
                    message.Sender = friend_info.Username;
                    message.IsMyMessage = false;
                }

                foreach (var image in message_info.Images) {
                    Client.Instance.SendRequest(new DownloadImageRequest() {
                        FriendID = response.FriendID,
                        MessageID = message_info.ID,
                        Filename = image
                    });
                }

                view_model.FriendInfo.LastMessageSendDate = message_info.SendDate;
                messages.Add(message);
            }

            view_model.Messages = new ObservableCollection<Chat.MessageInfo>(messages);
            view_model.Initialized = true;
        }

        private void OnSendFriendInvitationResponse(SendFriendInvitationResponse response) {
            var view_model = ContactManagerVM;

            if(response.Result == ResponseResult.Success) {
                view_model.PendingInvitations.Add(new ContactManagerItem() {
                    UserID = response.UserID,
                    Username = response.Username,

                    IsEnabledAcceptOption = false,
                    IsEnabledDeclineOption = true,

                    ItemInfoType = "PendingInvitation"
                });

                view_model.AddContactVM.ErrorMessage = ResourcesDictionary.Info;
                view_model.AddContactVM.NotificationText = ResourceManager.GetValue(ResourcesDictionary.AddFriendSuccess);
            }

            if (response.Errors.Count > 0) {
                switch (response.Errors[0]) {
                    case ErrorCode.AccountNotFound: {
                        view_model.AddContactVM.ErrorMessage = ResourcesDictionary.Warning;
                        view_model.AddContactVM.NotificationText = ResourceManager.GetValue(ResourcesDictionary.AddFriendUserNotExist);
                        break;
                    }
                    case ErrorCode.InvitationSelfInvite: {
                        view_model.AddContactVM.ErrorMessage = ResourcesDictionary.Warning;
                        view_model.AddContactVM.NotificationText = ResourceManager.GetValue(ResourcesDictionary.AddFriendSelfRequest);
                        break;
                    }
                    case ErrorCode.InvitationAlreadyFriends: {
                        view_model.AddContactVM.ErrorMessage = ResourcesDictionary.Warning;
                        view_model.AddContactVM.NotificationText = ResourceManager.GetValue(ResourcesDictionary.AddFriendExistingFriend);
                        break;
                    }
                    case ErrorCode.InvitationDuplicate: {
                        view_model.AddContactVM.ErrorMessage = ResourcesDictionary.Warning;
                        view_model.AddContactVM.NotificationText = ResourceManager.GetValue(ResourcesDictionary.AddFriendExistingRequest);
                        break;
                    }
                }
            }
        }

        private void OnAcceptFriendInvitationResponse(AcceptFriendInvitationResponse response) {
            var friend_info = new FriendInfo() {
                ID = response.UserID,
                Status = response.Status,

                Username = response.Username,
                UserImage = ImageLoader.Load(response.ImageBytes)
            };

            var invitation = ContactManagerVM.ReceivedInvitations.Single(p => p.UserID == response.UserID);
            ContactManagerVM.ReceivedInvitations.Remove(invitation);

            FriendList.Insert(0, new ChatViewModel(friend_info));
            UpdateNotifcationBall();
        }

        private void OnDownloadImageResponse(DownloadImageResponse response) {
            var message = FriendList.Single(p => p.FriendInfo.ID == response.FriendID).Messages.Single(p => p.ID == response.MessageID);
            message.Images.Add(ImageLoader.Load(response.ImageBytes));
        }

        private void OnDeleteMessageResponse(DeleteMessageResponse response) {
            var view_model = FriendList.Single(p => p.FriendInfo.ID == response.FriendID);
            view_model.Messages.Remove(view_model.Messages.Single(p => p.ID == response.MessageID));
        }

        private void OnSendMessageResponse(SendMessageResponse response) {
            var message_info = new Chat.MessageInfo() {
                ID = response.MessageID,
                Date = response.SendDate.ToLocalTime().ToString("HH:mm"),
                Content = response.Content,

                Sender = Client.Data.Username,
                IsMyMessage = true,

                Images = new ObservableCollection<BitmapImage>()
            };


            foreach (var image in response.Images) {
                Client.Instance.SendRequest(new DownloadImageRequest() {
                    FriendID = response.FriendID,
                    MessageID = message_info.ID,
                    Filename = image
                });
            }

            var view_model = FriendList.Single(p => p.FriendInfo.ID == response.FriendID);
            view_model.FriendInfo.LastMessageSendDate = response.SendDate;

            if(view_model != FriendList[0]) {
                var index = FriendList.IndexOf(view_model);
                FriendList.Move(index, 0);
            }

            view_model.Messages.Add(message_info);
        }

        private void OnStartVoiceChatResponse(StartVoiceChatResponse response) {
            var prev_call = FriendList.SingleOrDefault(p => p.FriendInfo.ID != response.FriendID && p.IsOnCall);

            if (prev_call != null) {
                Client.Instance.SendRequest(new DisconnectVoiceChatRequest() {
                    FriendID = prev_call.FriendInfo.ID
                });

                prev_call.IsOnCall = false;
            }
        }

        private void OnAcceptVoiceChatResponse(AcceptVoiceChatResponse response) {
            if(response.Result == ResponseResult.Success) {
                Client.Data.ClientEndPoint = response.EndPoint;
            }
        }

        private void OnChangeUsernameResponse(ChangeUsernameResponse response) {
            foreach (var friend in FriendList) {
                foreach (var message in friend.Messages) {
                    if (friend.Initialized && message.IsMyMessage) {
                        message.Sender = response.Username;
                    }
                }
            }
        }

        // Notification events
        protected override void OnNotificationReceived(NotificationDispatcher dispatcher) {
            // Friend status
            dispatcher.Dispatch<LoginNotification>(OnLoginNotification);
            dispatcher.Dispatch<LogoutNotification>(OnLogoutNotification);
            dispatcher.Dispatch<SetStatusNotification>(OnSetStatusNotification);

            // Invitations
            dispatcher.Dispatch<SendFriendInvitationNotification>(OnSendFriendInvitationNotification);
            dispatcher.Dispatch<AcceptFriendInvitationNotification>(OnAcceptFriendInvitationNotification);

            // Text chat
            dispatcher.Dispatch<DeleteMessageNotification>(OnDeleteMessageNotification);
            dispatcher.Dispatch<SendMessageNotification>(OnSendMessageNotification);

            // Voice chat
            dispatcher.Dispatch<StartVoiceChatNotification>(OnStartVoiceChatNotification);
            dispatcher.Dispatch<AcceptVoiceChatNotification>(OnAcceptVoiceChatNotification);
            dispatcher.Dispatch<DisconnectVoiceChatNotification>(OnDisconnectVoiceChatNotification);

            // Settings
            dispatcher.Dispatch<ChangeAvatarNotification>(OnChangeAvatarNotification);
            dispatcher.Dispatch<ChangeUsernameNotification>(OnChangeUsernameNotification);
        }

        private void OnLoginNotification(LoginNotification notification) {
            var view_model = FriendList.SingleOrDefault(p => p.FriendInfo.ID == notification.ID);

            if (view_model != null) {
                view_model.FriendInfo.Status = notification.Status;
            }
        }

        private void OnLogoutNotification(LogoutNotification notification) {
            var view_model = FriendList.SingleOrDefault(p => p.FriendInfo.ID == notification.ID);

            if (view_model != null) {
                view_model.FriendInfo.Status = false;
            }

            if (notification.EndPoint.Equals(Client.Data.ClientEndPoint)) {
                Client.Data.ClientEndPoint = null;
            }
        }

        private void OnSetStatusNotification(SetStatusNotification notification) {
            var view_model = FriendList.SingleOrDefault(p => p.FriendInfo.ID == notification.ID);

            if (view_model != null) {
                view_model.FriendInfo.Status = notification.Status;
            }
        }

        private void OnSendFriendInvitationNotification(SendFriendInvitationNotification notification) {
            var item = new ContactManagerItem() {
                UserID = notification.UserID,
                Username = notification.Username,

                IsEnabledAcceptOption = true,
                IsEnabledDeclineOption = true,

                ItemInfoType = "ReceivedInvitation"
            };

            ContactManagerVM.ReceivedInvitations.Add(item);
            UpdateNotifcationBall();
        }

        private void OnAcceptFriendInvitationNotification(AcceptFriendInvitationNotification notification) {
            var invitation = ContactManagerVM.PendingInvitations.SingleOrDefault(p => p.UserID == notification.UserID);

            var friend_info = new FriendInfo() {
                ID = notification.UserID,
                Status = notification.Status,

                Username = notification.Username,
                UserImage = ImageLoader.Load(notification.ImageBytes),

                IsANewMessage = true,
                LastMessageSendDate = DateTime.UtcNow
            };

            if(invitation != null) {
                ContactManagerVM.PendingInvitations.Remove(invitation);
                FriendList.Insert(0, new ChatViewModel(friend_info));
            }
        }

        private void OnDeleteMessageNotification(DeleteMessageNotification notification) {
            var view_model = FriendList.SingleOrDefault(p => p.FriendInfo.ID == notification.FriendID);

            if (view_model != null && view_model.Initialized) {
                var message = view_model.Messages.Single(p => p.ID == notification.MessageID);
                view_model.Messages.Remove(message);
            }
        }

        private void OnSendMessageNotification(SendMessageNotification notification) {
            var view_model = FriendList.SingleOrDefault(p => p.FriendInfo.ID == notification.FriendID);

            if(view_model != null) {
                if (view_model.Initialized) {
                    var message = new Chat.MessageInfo() {
                        ID = notification.MessageID,
                        Date = notification.SendDate.ToLocalTime().ToString("HH:mm"),
                        Content = notification.Content,

                        Sender = view_model.FriendInfo.Username,
                        IsMyMessage = false,

                        Images = new ObservableCollection<BitmapImage>()
                    };

                    foreach (var image in notification.Images) {
                        Client.Instance.SendRequest(new DownloadImageRequest() {
                            FriendID = notification.FriendID,
                            MessageID = message.ID,
                            Filename = image
                        });
                    }

                    view_model.Messages.Add(message);
                }

                if (UnreadedMessages.Contains(view_model.FriendInfo.ID) == false) {
                    if (SelectedFriend == null || SelectedFriend.FriendInfo.ID != view_model.FriendInfo.ID) {
                        view_model.FriendInfo.IsANewMessage = true;
                        UnreadedMessages.Add(view_model.FriendInfo.ID);
                    }
                    else {
                        Client.Instance.SendRequest(new SetMessageReadRequest() {
                            FriendID = view_model.FriendInfo.ID
                        });
                    }
                }

                if (view_model != FriendList[0]) {
                    var index = FriendList.IndexOf(view_model);
                    FriendList.Move(index, 0);
                }

                view_model.FriendInfo.LastMessageSendDate = notification.SendDate;
                UpdateNotifcationBall();
            }
        }

        private void OnStartVoiceChatNotification(StartVoiceChatNotification notification) {
            var view_model = FriendList.SingleOrDefault(p => p.FriendInfo.ID == notification.FriendID);

            if(view_model != null) {
                var callMessageBoxVM = new CallMessageBoxViewModel();
                callMessageBoxVM.SetInfo(view_model.FriendInfo.UserImage, view_model.FriendInfo.Username);

                if (_dialogService.OpenDialog(callMessageBoxVM) == DialogResults.Yes) {
                    var info = FriendList.SingleOrDefault(p => p.FriendInfo.ID != notification.FriendID && p.IsOnCall);

                    if (info != null) {
                        Client.Instance.SendRequest(new DisconnectVoiceChatRequest() {
                            FriendID = info.FriendInfo.ID
                        });

                        info.IsOnCall = false;
                    }

                    Client.AES = new EncryptionAES();
                    view_model.IsOnCall = true;

                    Client.Instance.SendRequest(new AcceptVoiceChatRequest() { 
                        FriendID = view_model.FriendInfo.ID,
                        Key = Client.AES.GetKey(),
                        IV = Client.AES.GetIV()
                    });
                }
                else {
                    Client.Instance.SendRequest(new DisconnectVoiceChatRequest() {
                        FriendID = view_model.FriendInfo.ID
                    });
                }

                SelectedFriend = view_model;
            }
        }

        private void OnAcceptVoiceChatNotification(AcceptVoiceChatNotification notification) {
            var view_model = FriendList.SingleOrDefault(p => p.FriendInfo.ID == notification.FriendID);

            Client.AES = new EncryptionAES(notification.Key, notification.IV);
            Client.Data.ClientEndPoint = notification.EndPoint;

            if(view_model != null) {
                var info = FriendList.SingleOrDefault(p => p.FriendInfo.ID != notification.FriendID && p.IsOnCall);

                if (info != null) {
                    Client.Instance.SendRequest(new DisconnectVoiceChatRequest() {
                        FriendID = info.FriendInfo.ID
                    });

                    info.IsOnCall = false;
                }

                view_model.IsOnCall = true;
            }
        }

        private void OnChangeAvatarNotification(ChangeAvatarNotification notification) {
            var view_model = FriendList.SingleOrDefault(p => p.FriendInfo.ID == notification.FriendID);

            if (view_model != null) {
                view_model.FriendInfo.UserImage = ImageLoader.Load(notification.UserImage);
            }
        }

        private void OnChangeUsernameNotification(ChangeUsernameNotification notification) {
            var view_model = FriendList.SingleOrDefault(p => p.FriendInfo.ID == notification.FriendID);

            if(view_model != null) {
                view_model.FriendInfo.Username = notification.Username;

                foreach(var message in view_model.Messages) {
                    if(message.IsMyMessage == false) {
                        message.Sender = notification.Username;
                    }
                }
            }
        }

        private void OnDisconnectVoiceChatNotification(DisconnectVoiceChatNotification notification) {
            var view_model = FriendList.SingleOrDefault(p => p.FriendInfo.ID == notification.FriendID);

            Client.AES = null;
            Client.Data.ClientEndPoint = null;

            if (view_model != null) {
                view_model.IsOnCall = false;
            }
        }
    }

}