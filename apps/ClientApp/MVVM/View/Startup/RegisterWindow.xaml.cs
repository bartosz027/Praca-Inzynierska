using System;
using System.Windows;

using ClientApp.Core;

using Network.Client;
using Network.Client.DataProcessing;

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
        }

        // Window events
        protected override void CloseWindowButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }

        private void RegisterAccount_Click(object sender, RoutedEventArgs e) {
            if (!String.IsNullOrEmpty(EmailBox.Text) && !String.IsNullOrEmpty(PasswordBox.Password) && !String.IsNullOrEmpty(UsernameBox.Text)) {
                Client.Instance.SendRequest(new RegisterRequest() {
                    Email = EmailBox.Text,
                    Password = PasswordBox.Password,

                    Username = UsernameBox.Text,
                    DateOfBirth = DateTime.UtcNow
                });
            }
            else {
                ValidatorMessage.Visibility = Visibility.Visible;
            }
        }

        private void VerifyEmail_Click(object sender, RoutedEventArgs e) {
            if (!String.IsNullOrEmpty(CodeBox.Text)) {
                Client.Instance.SendRequest(new VerifyEmailRequest() {
                    Email = EmailBox.Text,
                    Code = CodeBox.Text
                });
            }
            else {
                ValidatorMessageCode.Visibility = Visibility.Visible;
            }
        }

        // Response events
        protected override void ResponseReceived(ResponseDispatcher dispatcher) {
            dispatcher.Dispatch<RegisterResponse>(OnRegisterResponse);
            dispatcher.Dispatch<VerifyEmailResponse>(OnVerifyEmailResponse);
        }

        private void OnRegisterResponse(RegisterResponse response) {
            switch (response.Result) {
                case Result.Success: {
                    VerifyEmailForm.Visibility = Visibility.Visible;
                    break;
                }
                case Result.Failure: {
                    // TODO: Implement error codes
                    ValidatorMessage.Visibility = Visibility.Visible;
                    break;
                }
            }
        }

        private void OnVerifyEmailResponse(VerifyEmailResponse response) {
            switch (response.Result) {
                case Result.Success: {
                    this.Dispose();
                    this.Close();
                    break;
                }
                case Result.Failure: {
                    // TODO: Implement error codes
                    ValidatorMessageCode.Visibility = Visibility.Visible;
                    break;
                }
            }
        }
    }

}