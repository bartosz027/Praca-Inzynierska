using System.Windows;
using System.Windows.Input;
using ClientApp.MVVM.View.PIWindowView;
using ClientApp.MVVM.ViewModel.PIWindowViewModel;

namespace ClientApp.MVVM.View.LoginWindowView
{
    /// <summary>
    /// Logika interakcji dla klasy LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
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

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow RegisterWindow = new RegisterWindow();
            RegisterWindow.Show();

            // Close current window
            Application.Current.Windows[0].Close();
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            ForgotPasswordWindow ForgotPasswordWindow = new ForgotPasswordWindow();
            ForgotPasswordWindow.Show();

            // Close current window
            Application.Current.Windows[0].Close();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {

            PIWindow PIWindow = new PIWindow();
            PIWindowViewModel PIWindowViewModel = new PIWindowViewModel();
            PIWindow.DataContext = PIWindowViewModel;
            PIWindow.Show();
            // Close current window
           // Application.Current.Windows[0].Close();
        }
    }
}