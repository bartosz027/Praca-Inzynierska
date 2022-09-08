using System;
using System.Windows;

using ClientApp.Core;
using ClientApp.Resources;

using Network.Client;
using Network.Client.DataProcessing;

using Network.Shared.Core;
using Network.Shared.DataTransfer.Base;
using Network.Shared.DataTransfer.Model.Account.Register;

namespace ClientApp.MVVM.View.Startup {

    /// <summary>
    /// Logika interakcji dla klasy RegisterWindow.xaml
    /// </summary>
    ///
    public partial class RegisterWindow : BaseWindow {
        public RegisterWindow() {
            InitializeComponent();
            EnableResponseListener();
        }

        // Window events
        protected override void CloseWindowButton_Click(object sender, RoutedEventArgs e) {
            DisableResponseListener();
            DialogResult = false;
        }

        private void RegisterAccount_Click(object sender, RoutedEventArgs e) {
            if (!String.IsNullOrEmpty(EmailBox.Text) && !String.IsNullOrEmpty(UsernameBox.Text) && !String.IsNullOrEmpty(PasswordBox.Password)) {
                Client.Instance.SendRequest(new RegisterRequest() {
                    Email = EmailBox.Text,
                    Username = UsernameBox.Text,
                    Password = PasswordBox.Password
                });
            }
            else {
                ValidatorMessage.Text = ResourceManager.GetValue(ResourcesDictionary.RegisterNotAllData);
                ValidatorMessage.Visibility = Visibility.Visible;
            }
        }

        private void VerifyCode_Click(object sender, RoutedEventArgs e) {
            if (!String.IsNullOrEmpty(CodeBox.Text)) {
                Client.Instance.SendRequest(new VerifyEmailRequest() {
                    Email = EmailBox.Text,
                    Code = CodeBox.Text
                });
            }
            else {
                ShowErrorMessage(ValidatorMessageCode, ResourcesDictionary.EmptyCode);
            }
        }

        // Response events
        protected override void OnResponseReceived(ResponseDispatcher dispatcher) {
            dispatcher.Dispatch<RegisterResponse>(OnRegisterResponse);
            dispatcher.Dispatch<VerifyEmailResponse>(OnVerifyEmailResponse);
        }

        private void OnRegisterResponse(RegisterResponse response) {
            if (response.Result == ResponseResult.Success) {
                VerifyCodeForm.Visibility = Visibility.Visible;
            }

            if(response.Errors.Count > 0) {
                switch (response.Errors[0]) {
                    case ErrorCode.InvalidEmailAddress: {
                        ShowErrorMessage(ValidatorMessage, ResourcesDictionary.NotValidEmail);
                        break;
                    }
                    case ErrorCode.EmailAddressTaken: {
                        ShowErrorMessage(ValidatorMessage, ResourcesDictionary.RegisterEmailExist);
                        break;
                    }
                    case ErrorCode.InvalidUsername: {
                        ShowErrorMessage(ValidatorMessage, ResourcesDictionary.RegisterUsernameTooShort);
                        break;
                    }
                    case ErrorCode.InvalidPassword: {
                        ShowErrorMessage(ValidatorMessage, ResourcesDictionary.RegisterWeakPassword);
                        break;
                    }
                    default: {
                        throw new NotSupportedException();
                    }
                }
            }
        }

        private void OnVerifyEmailResponse(VerifyEmailResponse response) {
            if (response.Result == ResponseResult.Success) {
                this.DisableResponseListener();
                this.Close();
            }

            if (response.Errors.Count > 0) {
                switch (response.Errors[0]) {
                    case ErrorCode.InvalidVerificationCode: {
                        ShowErrorMessage(ValidatorMessageCode, ResourcesDictionary.InvalidCode);
                        break;
                    }
                    case ErrorCode.ExpiredVerificationCode: {
                        ShowErrorMessage(ValidatorMessageCode, ResourcesDictionary.ExpiredCode);
                        break;
                    }
                    default: {
                        throw new NotSupportedException();
                    }
                }
            }
        }
    }

}