﻿using System;
using System.Collections.ObjectModel;
using System.Windows;

using ClientApp.Core;
using Network.Client;

using Network.Shared.Core;
using Network.Shared.DataTransfer.Model.Database.Friends.GetMessageHistory;

using Network.Shared.DataTransfer.Model.Friends.ManageMessages.DeleteMessage;
using Network.Shared.DataTransfer.Model.Friends.ManageMessages.SendMessage;

using Network.Shared.DataTransfer.Model.Friends.VoiceChat.StartVoiceChat;

namespace ClientApp.MVVM.ViewModel.Contacts.Chat {

    internal class MessageInfo : ObservableObject {
        public int ID { get; set; }
        public bool IsMyMessage { get; set; }

        public string Date {
            get { 
                return _Date; 
            }
            set {
                _Date = value;
                OnPropertyChanged();
            }
        }

        public string Sender {
            get { 
                return _Sender; 
            }
            set {
                _Sender = value;
                OnPropertyChanged();
            }
        }

        public string Content {
            get {
                return _Content;
            }
            set {
                _Content = value;
                OnPropertyChanged();
            }
        }

        private string _Date;
        private string _Sender;
        private string _Content;
    }

    internal class ChatViewModel : BaseVM {
        public ChatViewModel(FriendInfo friend_info) {
            FriendInfo = friend_info;
            Messages = new ObservableCollection<MessageInfo>();

            SendMessageCommand = new RelayCommand(o => {
                if (RichBoxContent.Length > 0 && RichBoxContent.Length <= Values.MaxMessageLength) {
                    Client.Instance.SendRequest(new SendMessageRequest() {
                        FriendID = FriendInfo.ID,
                        Content = RichBoxContent,
                    });

                    RichBoxContent = String.Empty;
                }

                // TODO: Error -> message too long
            });

            RemoveMessageCommand = new RelayCommand(o => {
                var message = o as MessageInfo;

                Client.Instance.SendRequest(new DeleteMessageRequest() {
                    FriendID = FriendInfo.ID,
                    MessageID = message.ID
                });
            });

            CopyMessageCommand = new RelayCommand(o => {
                var message = o as MessageInfo;
                Clipboard.SetText(message.Content);
            });

            CallFriendCommand = new RelayCommand(o => {
                Client.Instance.SendRequest(new StartVoiceChatRequest() {
                    FriendID = FriendInfo.ID
                });
            });
        }

        // Methods
        public void Init() {
            Client.Instance.SendRequest(new GetMessageHistoryRequest() {
                FriendID = FriendInfo.ID
            });
        }
        public bool Initialized { get; set; }

        // Commands
        public RelayCommand SendMessageCommand { get; private set; }
        public RelayCommand RemoveMessageCommand { get; private set; }
        public RelayCommand CopyMessageCommand { get; private set; }
        public RelayCommand CallFriendCommand { get; private set; }

        // Properties
        public bool IsFocused {
            get {
                return _IsFocused;
            }
            set {
                _IsFocused = value;
                OnPropertyChanged();
            }
        }
        public bool IsOnCall
        {
            get
            {
                return _IsOnCall;
            }
            set
            {
                _IsOnCall = value;
                OnPropertyChanged();
            }
        }

        public string RichBoxContent {
            get {
                return _RichBoxContent;
            }
            set {
                _RichBoxContent = value;
                OnPropertyChanged();
            }
        }

        public FriendInfo FriendInfo {
            get {
                return _Friend;
            }
            set {
                _Friend = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<MessageInfo> Messages {
            get { 
                return _Messages; 
            }
            set {
                _Messages = value;
                OnPropertyChanged();
            }
        }

        private bool _IsOnCall;
        private bool _IsFocused;
        private string _RichBoxContent;

        private FriendInfo _Friend;
        private ObservableCollection<MessageInfo> _Messages;
    }

}