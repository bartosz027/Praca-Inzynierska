using ClientApp.Core;
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
    public partial class RegisterWindow : WindowBase
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
            if (EmailBox.Text != String.Empty && PasswordBox.Password != String.Empty && UsernameBox.Text != String.Empty)
            {
                Client.Instance.SendRequest(new RegisterRequest()
                {
                    Email = EmailBox.Text,
                    Password = PasswordBox.Password,

                    Username = UsernameBox.Text,
                    DateOfBirth = DateTime.Now
                });
            }
            else 
            {
                ValidatorMessage.Visibility = Visibility.Visible;
            }
        }

        private new void CloseWindowButton_Click(object sender, RoutedEventArgs e) 
        {
            var test = Application.Current.Windows;
            this.Close();
            Application.Current.Windows[0].Show();
        }
        private void VerifyCode_Click(object sender, RoutedEventArgs e)
        {
            if (CodeBox.Text != String.Empty)
            {
                Client.Instance.SendRequest(new VerifyEmailRequest()
                {
                    Email = EmailBox.Text,
                    Code = CodeBox.Text
                });
            }
            else 
            {
                ValidatorMessageCode.Visibility = Visibility.Visible;
            }
        }

        // Response Event Handling
        private void OnResponseReceived(object sender, Response response)
        {
            var dispatcher = new ResponseDispatcher(response);

            App.Current.Dispatcher.Invoke(delegate {
                dispatcher.Dispatch<RegisterResponse>(OnRegisterResponse);
                dispatcher.Dispatch<VerifyEmailResponse>(OnVerifyEmailResponse);
            });
        }

        private void OnRegisterResponse(RegisterResponse response)
        {
            switch (response.Status)
            {
                case STATUS.SUCCESS:
                {
                    RegisterCode.Visibility = Visibility.Visible;
                    break;
                }
                case STATUS.FAILURE: 
                {
                    ValidatorMessage.Visibility = Visibility.Visible;
                    break;
                }
            }
        }
        
        private void OnVerifyEmailResponse(VerifyEmailResponse response)
        {
            switch (response.Status)
            {
                case STATUS.SUCCESS:
                {
                    // Close current window
                    Client.Instance.ResponseReceived -= OnResponseReceived;
                    Application.Current.Windows[1].Hide();
                    Application.Current.Windows[0].Show();

                    break;
                }
                case STATUS.FAILURE: 
                {
                    ValidatorMessageCode.Visibility = Visibility.Visible;
                    break;
                }
            }
        }
    }
}