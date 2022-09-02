using System;
using System.Windows;

using Network.Client;
using Network.Client.DataProcessing;

using Network.Shared.DataTransfer.Base;
using Network.Shared.DataTransfer.Model.Account.Login;

using ClientApp.Core;

namespace ClientApp.MVVM.View.Startup {

    /// <summary>
    /// Logika interakcji dla klasy LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : BaseWindow {
        public LoginWindow() {
            InitializeComponent();

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
        }

        private void Register_Click(object sender, RoutedEventArgs e) {
            var window = new RegisterWindow();
            window.ShowDialog();
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e) {
            var window = new ForgotPasswordWindow();
            window.ShowDialog();
        }

        // Debug events
        private void DebugAccount1_Click(object sender, RoutedEventArgs e) {
            Client.Instance.SendRequest(new LoginRequest() {
                Email = "test1@gmail.com",
                Password = "okon1"
            });
        }

        private void DebugAccount2_Click(object sender, RoutedEventArgs e) {
            Client.Instance.SendRequest(new LoginRequest() {
                Email = "test2@gmail.com",
                Password = "okon2"
            });
        }

        private void DebugAccount3_Click(object sender, RoutedEventArgs e) {
            Client.Instance.SendRequest(new LoginRequest() {
                Email = "test3@gmail.com",
                Password = "okon3"
            });
        }

        // Response events
        protected override void ResponseReceived(ResponseDispatcher dispatcher) {
            dispatcher.Dispatch<LoginResponse>(OnLoginResponse);
        }

        private void OnLoginResponse(LoginResponse response) {
            switch (response.Result) {
                case Result.Success: {
                    Client.Data.Username = response.Username;
                    Client.Data.AccessToken = response.AccessToken;

                    var window = new MainWindow();
                    window.Show();

                    this.Dispose();
                    this.Close();

                    break;
                }
                case Result.Failure: {
                    ValidatorMessage.Visibility = Visibility.Visible;
                    break;
                }
            }
        }
    }

}