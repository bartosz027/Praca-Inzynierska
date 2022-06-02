using Network.Client;
using Network.Client.DataProcessing;
using Network.Shared.DataTransfer.Base;
using Network.Shared.DataTransfer.Model.Account.Register;
using System;
using System.Windows;
using System.Windows.Input;

namespace ClientApp.MVVM.View.LoginWindowView
{
    /// <summary>
    /// Logika interakcji dla klasy RegisterWindow.xaml
    /// </summary>
    ///
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
            Client.Instance.ResponseReceived += OnResponseReceived;
        }

        private void ResizeWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        // Click
        private void SendCode_Click(object sender, RoutedEventArgs e)
        {
            Client.Instance.SendRequest(new RegisterRequest()
            {
                Email = EmailBox.Text,
                Username = UsernameBox.Text,
                Password = PasswordBox.Password
            });
        }
        private void VerifyCode_Click(object sender, RoutedEventArgs e)
        {
            Client.Instance.SendRequest(new VerifyEmailRequest()
            {
                Email = EmailBox.Text,
                Code = CodeBox.Text
            });
        }

        // Response Event Handling
        private void OnResponseReceived(object sender, Response response)
        {
            var dispatcher = new ResponseDispatcher(response);
            dispatcher.Dispatch<RegisterResponse>(OnRegisterResponse);
            dispatcher.Dispatch<VerifyEmailResponse>(OnVerifyEmailResponse);
        }

        private void OnRegisterResponse(RegisterResponse response)
        {
            switch (response.Status)
            {
                case STATUS.SUCCESS:
                {
                    App.Current.Dispatcher.Invoke(delegate {
                        RegisterCode.Visibility = Visibility.Visible;
                    });
                    break;
                }
                    
                case STATUS.FAILURE:
                    MessageBox.Show("TODO: LUX powiadomienie");
                    break;
            }
        }
        
        private void OnVerifyEmailResponse(VerifyEmailResponse response)
        {
            switch (response.Status)
            {
                case STATUS.SUCCESS:
                {
                    App.Current.Dispatcher.Invoke(delegate {
                        LoginWindow LoginWindow = new LoginWindow();
                        
                        LoginWindow.Show();

                        Application.Current.Windows[0].Close();
                    });
                    break;
                }
                case STATUS.FAILURE:
                    MessageBox.Show("TODO: LUX powiadomienie");
                    break;
            }
        }

    }
}