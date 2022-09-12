using System;
using System.ComponentModel.DataAnnotations;
using System.Windows;

using ClientApp.Core;
using ClientApp.Resources;

using Network.Client;
using Network.Client.DataProcessing;

using Network.Shared.Core;
using Network.Shared.DataTransfer.Base;

using Network.Shared.DataTransfer.Model.Account.Register;
using Network.Shared.DataTransfer.Model.Account.SendVerificationCode;
using Network.Shared.DataTransfer.Model.Account.VerifyEmail;

namespace ClientApp.MVVM.View.AppStartup {

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
            if (String.IsNullOrEmpty(EmailBox.Text) || String.IsNullOrEmpty(UsernameBox.Text) || String.IsNullOrEmpty(PasswordBox.Password)) {
                ShowErrorMessage(ValidatorMessage, ResourcesDictionary.FieldIsEmpty);
            }
            else if (new EmailAddressAttribute().IsValid(EmailBox.Text) == false) {
                ShowErrorMessage(ValidatorMessage, ResourcesDictionary.InvalidEmailAddress);
            }
            else if (UsernameBox.Text.Length < Values.MinUsernameLength || UsernameBox.Text.Length > Values.MaxUsernameLength) {
                ShowErrorMessage(ValidatorMessage, ResourcesDictionary.InvalidUsername, Values.MinUsernameLength, Values.MaxUsernameLength);
            }
            else if (PasswordBox.Password.Length < Values.MinPasswordLength || PasswordBox.Password.Length > Values.MaxPasswordLength) {
                ShowErrorMessage(ValidatorMessage, ResourcesDictionary.InvalidPassword, Values.MinPasswordLength);
            }
            else {
                Client.Instance.SendRequest(new RegisterRequest() {
                    Email = EmailBox.Text,
                    Username = UsernameBox.Text,
                    Password = PasswordBox.Password
                });
            }
        }

        private void VerifyCode_Click(object sender, RoutedEventArgs e) {
            if (String.IsNullOrEmpty(CodeBox.Text)) {
                ShowErrorMessage(ValidatorMessageCode, ResourcesDictionary.FieldVerificationCodeIsEmpty);
            }
            else {
                Client.Instance.SendRequest(new VerifyEmailRequest() {
                    Email = EmailBox.Text,
                    VerificationCode = CodeBox.Text
                });
            }
        }

        // Response events
        protected override void OnResponseReceived(ResponseDispatcher dispatcher) {
            dispatcher.Dispatch<RegisterResponse>(OnRegisterResponse);
            dispatcher.Dispatch<VerifyEmailResponse>(OnVerifyEmailResponse);
        }

        private void OnRegisterResponse(RegisterResponse response) {
            if (response.Result == ResponseResult.Success) {
                Client.Instance.SendRequest(new SendVerificationCodeRequest() {
                    Email = EmailBox.Text
                });

                VerifyCodeForm.Visibility = Visibility.Visible;
            }

            if(response.Errors.Count > 0) {
                switch (response.Errors[0]) {
                    case ErrorCode.EmailAddressTaken: {
                        ShowErrorMessage(ValidatorMessage, ResourcesDictionary.EmailAddressTaken);
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
                        ShowErrorMessage(ValidatorMessageCode, ResourcesDictionary.InvalidVerificationCode);
                        break;
                    }
                    case ErrorCode.ExpiredVerificationCode: {
                        ShowErrorMessage(ValidatorMessageCode, ResourcesDictionary.ExpiredVerificationCode);
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