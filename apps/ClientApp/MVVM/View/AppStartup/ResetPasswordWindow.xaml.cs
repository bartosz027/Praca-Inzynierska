using System;
using System.Windows;

using ClientApp.Core;
using ClientApp.Resources;

using Network.Client;
using Network.Client.DataProcessing;

using Network.Shared.Core;
using Network.Shared.DataTransfer.Base;

using Network.Shared.DataTransfer.Model.Account.ResetPassword;
using Network.Shared.DataTransfer.Model.Account.SendVerificationCode;

namespace ClientApp.MVVM.View.AppStartup {

    /// <summary>
    /// Logika interakcji dla klasy ForgotPasswordWindow.xaml
    /// </summary>
    public partial class ResetPasswordWindow : BaseWindow {
        public ResetPasswordWindow() {
            InitializeComponent();
            EnableResponseListener();
        }

        // Window events
        protected override void CloseWindowButton_Click(object sender, RoutedEventArgs e) {
            DisableResponseListener();
            DialogResult = false;
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e) {
            if (String.IsNullOrEmpty(EmailBox.Text) || String.IsNullOrEmpty(NewPasswordBox1.Password) || String.IsNullOrEmpty(NewPasswordBox2.Password)) {
                ShowErrorMessage(ValidatorMessage, ResourcesDictionary.FieldIsEmpty);
            }
            else if (NewPasswordBox1.Password != NewPasswordBox2.Password) {
                ShowErrorMessage(ValidatorMessage, ResourcesDictionary.NotSamePassword);
            }
            else if (NewPasswordBox1.Password.Length < Values.MinPasswordLength || NewPasswordBox1.Password.Length > Values.MaxPasswordLength) {
                ShowErrorMessage(ValidatorMessage, ResourcesDictionary.InvalidPassword);
            }
            else {
                Client.Instance.SendRequest(new SendVerificationCodeRequest() {
                    Email = EmailBox.Text
                });
            }
        }

        private void VerifyCode_Click(object sender, RoutedEventArgs e) {
            if (String.IsNullOrEmpty(CodeBox.Text)) {
                ShowErrorMessage(ValidatorMessageCode, ResourcesDictionary.FieldVerificationCodeIsEmpty);
            }
            else {
                Client.Instance.SendRequest(new ResetPasswordRequest() {
                    Email = EmailBox.Text,
                    VerificationCode = CodeBox.Text,
                    NewPassword = NewPasswordBox1.Password
                });
            }
        }

        // Response events
        protected override void OnResponseReceived(ResponseDispatcher dispatcher) {
            dispatcher.Dispatch<SendVerificationCodeResponse>(OnSendVerificationCodeResponse);
            dispatcher.Dispatch<ResetPasswordResponse>(OnResetPasswordResponse);
        }

        private void OnSendVerificationCodeResponse(SendVerificationCodeResponse response) {
            if (response.Result == ResponseResult.Success) {
                ChangePasswordForm.Visibility = Visibility.Hidden;
                VerifyCodeForm.Visibility = Visibility.Visible;
            }

            if (response.Errors.Count > 0) {
                switch (response.Errors[0]) {
                    case ErrorCode.AccountNotFound: {
                        ShowErrorMessage(ValidatorMessage, ResourcesDictionary.AccountNotFound);
                        break;
                    }
                    case ErrorCode.InvalidEmailAddress: {
                        ShowErrorMessage(ValidatorMessage, ResourcesDictionary.InvalidEmailAddress);
                        break;
                    }
                    default: {
                        throw new NotSupportedException();
                    }
                }
            }
        }

        private void OnResetPasswordResponse(ResetPasswordResponse response) {
            if (response.Result == ResponseResult.Success) {
                this.DisableResponseListener();
                this.Close();
            }

            if (response.Errors.Count > 0) {
                switch (response.Errors[0]) {
                    case ErrorCode.AccountNotVerified: {
                        VerifyCodeForm.Visibility = Visibility.Hidden;
                        ChangePasswordForm.Visibility = Visibility.Visible;

                        ShowErrorMessage(ValidatorMessage, ResourcesDictionary.AccountNotVerified);
                        break;
                    }
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