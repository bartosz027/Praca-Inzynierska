using System.Windows;
using System.Windows.Input;
using ClientApp.MVVM.View.PIWindowView;
using ClientApp.MVVM.ViewModel.PIWindowViewModel;
using Network.Client;
using Network.Shared.DataTransfer.Model.Account.Login;
using Network.Shared.DataTransfer.Base;
using Network.Client.DataProcessing;
using System;
using ClientApp.Core;

namespace ClientApp.MVVM.View.LoginWindowView
{
    /// <summary>
    /// Logika interakcji dla klasy LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : WindowBase
    {
        public LoginWindow()
        {
            InitializeComponent();
            Client.Instance.ResponseReceived += OnResponseReceived;

            #if DEBUG
                AutoLoginButtons.Visibility = Visibility.Visible;
            #else
                AutoLoginButtons.Visibility = Visibility.Hidden;
            #endif
        }

     

        // Click
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var window = new RegisterWindow();
            window.Show();

            // Close current window
            Client.Instance.ResponseReceived -= OnResponseReceived;
            Application.Current.Windows[0].Hide();
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            var window = new ForgotPasswordWindow();
            window.Show();

            // Close current window
            Client.Instance.ResponseReceived -= OnResponseReceived;
            Application.Current.Windows[0].Hide();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            Client.Instance.SendRequest(new LoginRequest() { 
                Email = EmailBox.Text, 
                Password = PasswordBox.Password 
            });
        }

        private void Account1_Click(object sender, RoutedEventArgs e) {
            Client.Instance.SendRequest(new LoginRequest() {
                Email = "test1@gmail.com",
                Password = "okon1"
            });
        }

        private void Account2_Click(object sender, RoutedEventArgs e) {
            Client.Instance.SendRequest(new LoginRequest() {
                Email = "test2@gmail.com",
                Password = "okon2"
            });
        }

        private void Account3_Click(object sender, RoutedEventArgs e) {
            Client.Instance.SendRequest(new LoginRequest() {
                Email = "test3@gmail.com",
                Password = "okon3"
            });
        }

        // Response Event Handling
        private void OnResponseReceived(object sender, Response response)
        {
            var dispatcher = new ResponseDispatcher(response);

            App.Current.Dispatcher.Invoke(delegate {
                dispatcher.Dispatch<LoginResponse>(OnLoginResponse);
            });
        }

        private void OnLoginResponse(LoginResponse response)
        {
            switch (response.Status)
            {
                case STATUS.SUCCESS:
                {
                    Client.Data.Username = response.Username;
                    Client.Data.AccessToken = response.AccessToken;
                    
                    var window = new PIWindow();
                    window.Show();

                    // Close current window
                    Client.Instance.ResponseReceived -= OnResponseReceived;
                    Application.Current.Windows[0].Close();

                    break;
                }
                case STATUS.FAILURE: 
                {
                    ValidatorMessage.Visibility = Visibility.Visible;
                    break;
                }
            }
        }
    }
}