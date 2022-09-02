using System;
using System.Windows;

using ClientApp.Core;

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
            VerifyEmailForm.Visibility = Visibility.Hidden;
            ChangePasswordForm.Visibility = Visibility.Visible;
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e) {
            // TODO: Implement change password request
        }
    }

}