using System;
using System.Windows;

using ClientApp.Core;
using ClientApp.Resources.Languages;

namespace ClientApp.MVVM.View.Startup {

    /// <summary>
    /// Logika interakcji dla klasy ForgotPasswordWindow.xaml
    /// </summary>
    public partial class ForgotPasswordWindow : BaseWindow {
        public ForgotPasswordWindow() {
            InitializeComponent();
        }

        // Window events
        protected override void CloseWindowButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }

        private void VerifyEmail_Click(object sender, RoutedEventArgs e) {
            if (!String.IsNullOrEmpty(EmailBox.Text)) 
            {
                if (!EmailBox.Text.Contains("@"))
                {
                    ShowErrorMessage(ValidatorMessage, ResourcesDictionary.NotValidEmail);
                }
                else
                {
                    VerifyEmailForm.Visibility = Visibility.Hidden;
                    VerifyCodeForm.Visibility = Visibility.Visible;
                }
            }
            else 
            {
                ShowErrorMessage(ValidatorMessage, ResourcesDictionary.RegisterNotAllData);
            } 
        }

        private void VerifyCode_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(CodeBox.Text))
            {
                if (CodeBox.Text == "CHUJ") // DEV ONLY
                {
                    ShowErrorMessage(ValidatorMessageCode, ResourcesDictionary.IncorrectCode);
                }
                else if (CodeBox.Text == "CIPA") // DEV ONLY
                {
                    ShowErrorMessage(ValidatorMessageCode, ResourcesDictionary.ExpiredCode);
                }
                else
                {
                    VerifyCodeForm.Visibility = Visibility.Hidden;
                    ChangePasswordForm.Visibility = Visibility.Visible;
                }
            }
            else 
            {
                ShowErrorMessage(ValidatorMessageCode, ResourcesDictionary.EmptyCode);
            }
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e) {
            if (!String.IsNullOrEmpty(NewPasswordBox.Password) && !String.IsNullOrEmpty(NewPasswordBox2.Password))
            {
                if (NewPasswordBox.Password.Length < 8)
                {
                    ShowErrorMessage(ValidatorMessage2, ResourcesDictionary.RegisterWeekPassword);
                }
                else if (NewPasswordBox.Password != NewPasswordBox2.Password)
                {
                    ShowErrorMessage(ValidatorMessage2, ResourcesDictionary.NotSamePassword);
                }
            }
            else 
            {
                ShowErrorMessage(ValidatorMessage2, ResourcesDictionary.RegisterNotAllData);
            }
            // TODO: Implement change password request
        }
    }

}