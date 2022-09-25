using ClientApp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            if (OldPasswordBox.Password == "" || NewPasswordBox1.Password == "" || NewPasswordBox2.Password == "")
            {
                ErrorMessage.Text = ResourceManager.GetValue(ResourcesDictionary.FieldIsEmpty);
                ErrorMessage.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#CB1919"));
            }
            else if (NewPasswordBox1.Password != NewPasswordBox2.Password) 
            {
                ErrorMessage.Text = ResourceManager.GetValue(ResourcesDictionary.NotSamePassword);
                ErrorMessage.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#CB1919"));
            }
            else 
            {
                ErrorMessage.Text = ResourceManager.GetValue(ResourcesDictionary.PasswordChanged);
                ErrorMessage.Foreground = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#007C77"));
            }
        }

    }
}
