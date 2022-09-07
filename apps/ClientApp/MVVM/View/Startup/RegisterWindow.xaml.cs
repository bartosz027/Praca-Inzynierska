using System;
using System.Windows;

using ClientApp.Core;
using ClientApp.Resources.Languages;
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

                if (!EmailBox.Text.Contains("@")) // DEV ONLY
                {
                    ShowErrorMessage(ValidatorMessage, ResourcesDictionary.NotValidEmail);
                }
                else if(UsernameBox.Text.Length < 3) // DEV ONLY
                {
                    ShowErrorMessage(ValidatorMessage, ResourcesDictionary.RegisterUsernameTooShort);
                }
                else if (PasswordBox.Password.Length < 8) // DEV ONLY
                {
                    ShowErrorMessage(ValidatorMessage, ResourcesDictionary.RegisterWeekPassword);
                }
                else
                {
                    Client.Instance.SendRequest(new RegisterRequest()
                    {
                        Email = EmailBox.Text,
                        Password = PasswordBox.Password,

                        Username = UsernameBox.Text,
                        DateOfBirth = DateTime.UtcNow
                    });
                    VerifyCodeForm.Visibility = Visibility.Visible;// DEV ONLY
                }
                
            }
            else {
                ValidatorMessage.Text = ResourcesManager.GetValue(ResourcesDictionary.RegisterNotAllData);
                ValidatorMessage.Visibility = Visibility.Visible;
            }
        }

        private void VerifyCode_Click(object sender, RoutedEventArgs e) {
            if (!String.IsNullOrEmpty(CodeBox.Text)) {
                if(CodeBox.Text == "CHUJ") // DEV ONLY
                {
                    ShowErrorMessage(ValidatorMessageCode, ResourcesDictionary.IncorrectCode);
                }
                else if (CodeBox.Text == "CIPA") // DEV ONLY
                {
                    ShowErrorMessage(ValidatorMessageCode, ResourcesDictionary.ExpiredCode);
                }
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
        protected override void ResponseReceived(ResponseDispatcher dispatcher) {
            dispatcher.Dispatch<RegisterResponse>(OnRegisterResponse);
            dispatcher.Dispatch<VerifyEmailResponse>(OnVerifyEmailResponse);
        }

        private void OnRegisterResponse(RegisterResponse response) {
            switch (response.Result) {
                case Result.Success: {
                    VerifyCodeForm.Visibility = Visibility.Visible;
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