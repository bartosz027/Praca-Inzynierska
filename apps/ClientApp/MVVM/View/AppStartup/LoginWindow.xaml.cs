using System;
using System.Windows;
using System.Windows.Controls;

using ClientApp.Core;
using ClientApp.Resources;

using Network.Client;
using Network.Client.DataProcessing;

using Network.Shared.Core;
using Network.Shared.DataTransfer.Base;
using Network.Shared.DataTransfer.Model.Account.Login;

namespace ClientApp.MVVM.View.AppStartup {

    /// <summary>
    /// Logika interakcji dla klasy LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : BaseWindow {
        public LoginWindow() {
            InitializeComponent();
            EnableResponseListener();

            #if DEBUG
                AutoLoginButtons.Visibility = Visibility.Visible;
            #else
                AutoLoginButtons.Visibility = Visibility.Hidden;
            #endif
        }

        // Window events
        private void Login_Click(object sender, RoutedEventArgs e) {
            Client.Instance.SendRequest(new LoginRequest() {
                Email = EmailBox.Text,
                Password = PasswordBox.Password
            });

            LoginButton.IsEnabled = false;
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e) {
            var window = new ResetPasswordWindow();
            window.ShowDialog();
        }

        private void Register_Click(object sender, RoutedEventArgs e) {
            var window = new RegisterWindow();
            window.ShowDialog();
        }

        // Debug events
        private void DebugAccount1_Click(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            button.IsEnabled = false;

            Client.Instance.SendRequest(new LoginRequest() {
                Email = "test1@gmail.com",
                Password = "okon1"
            });
        }

        private void DebugAccount2_Click(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            button.IsEnabled = false;

            Client.Instance.SendRequest(new LoginRequest() {
                Email = "test2@gmail.com",
                Password = "okon2"
            });
        }

        private void DebugAccount3_Click(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            button.IsEnabled = false;

            Client.Instance.SendRequest(new LoginRequest() {
                Email = "test3@gmail.com",
                Password = "okon3"
            });
        }

        private void DebugAccount4_Click(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            button.IsEnabled = false;

            Client.Instance.SendRequest(new LoginRequest() {
                Email = "test4@gmail.com",
                Password = "okon1"
            });
        }

        private void DebugAccount5_Click(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            button.IsEnabled = false;

            Client.Instance.SendRequest(new LoginRequest() {
                Email = "test5@gmail.com",
                Password = "okon2"
            });
        }

        private void DebugAccount6_Click(object sender, RoutedEventArgs e) {
            var button = sender as Button;
            button.IsEnabled = false;

            Client.Instance.SendRequest(new LoginRequest() {
                Email = "test6@gmail.com",
                Password = "okon3"
            });
        }

        // Response events
        protected override void OnResponseReceived(ResponseDispatcher dispatcher) {
            dispatcher.Dispatch<LoginResponse>(OnLoginResponse);
        }

        private void OnLoginResponse(LoginResponse response) {
            if (response.Result == ResponseResult.Success) {
                Client.Data.ID = response.ID;
                Client.Data.Status = response.Status;
                Client.Data.Username = response.Username;
                Client.Data.AccessToken = response.AccessToken;

                if (RememberMe_CheckBox.IsChecked == true) {
                    ConfigManager.SetValue("AccessToken", response.AccessToken);
                }
                else {
                    ConfigManager.SetValue("AccessToken", "NULL");
                }

                var window = new MainWindow();
                window.Show();

                App.Current.MainWindow = window;
                this.Close();
            }

            if (response.Errors.Count > 0) {
                switch (response.Errors[0]) {
                    case ErrorCode.InvalidEmailOrPassword: {
                        ShowErrorMessage(ValidatorMessage, ResourcesDictionary.InvalidEmailOrPassword);
                        break;
                    }
                    case ErrorCode.AccountNotVerified: {
                        ShowErrorMessage(ValidatorMessage, ResourcesDictionary.AccountNotVerified);
                        break;
                    }
                    default: {
                        throw new NotSupportedException();
                    }
                }
            }

            LoginButton.IsEnabled = true;
        }
    }

}