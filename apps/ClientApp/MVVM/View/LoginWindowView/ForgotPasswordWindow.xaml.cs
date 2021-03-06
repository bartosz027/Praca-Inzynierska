using System.Windows;
using System.Windows.Input;

namespace ClientApp.MVVM.View.LoginWindowView
{
    /// <summary>
    /// Logika interakcji dla klasy ForgotPasswordWindow.xaml
    /// </summary>
    public partial class ForgotPasswordWindow : Window
    {
        public ForgotPasswordWindow()
        {
            InitializeComponent();
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
            ForgotPasswordCode.Visibility = Visibility.Visible;
        }

        private void NewPasswordWindow_Click(object sender, RoutedEventArgs e)
        {
            var window = new NewPasswordWindow();
            window.Show();

            // Close current window
            Application.Current.Windows[0].Close();
        }
    }
}