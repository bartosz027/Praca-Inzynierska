using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Network.Client;
using ClientApp.Resources;

using Network.Shared.Core;
using Network.Shared.DataTransfer.Model.Settings.ChangePassword;

namespace ClientApp.MVVM.View.Settings.Options
{
    /// <summary>
    /// Logika interakcji dla klasy AccountSettingView.xaml
    /// </summary>
    public partial class AccountSettingView : UserControl
    {
        public AccountSettingView()
        {
            InitializeComponent();
        }

        private void ResetPassword_Click(object sender, RoutedEventArgs e)
        {
            ResetButtonPanel.Visibility = Visibility.Hidden;
            ResetFormPanel.Visibility = Visibility.Visible;
        }
        private void SendResetPasswordRequest_Click(object sender, RoutedEventArgs e)
        {
            if (NewPasswordBox1.Password != NewPasswordBox2.Password) {
                ErrorMessage.Text = ResourceManager.GetValue(ResourcesDictionary.NotSamePassword);
                ErrorMessage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CB1919"));
            }
            else if (NewPasswordBox1.Password.Length < Values.MinPasswordLength || NewPasswordBox1.Password.Length > Values.MaxPasswordLength) 
            {
                ErrorMessage.Text = ResourceManager.GetValue(ResourcesDictionary.InvalidPassword, Values.MinPasswordLength, Values.MaxPasswordLength);
                ErrorMessage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CB1919"));
            }
            else 
            {
                Client.Instance.SendRequest(new ChangePasswordRequest() { 
                    Password = NewPasswordBox1.Password
                });

                ErrorMessage.Text = ResourceManager.GetValue(ResourcesDictionary.PasswordChanged);
                ErrorMessage.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#007C77"));
            }
        }

    }
}
