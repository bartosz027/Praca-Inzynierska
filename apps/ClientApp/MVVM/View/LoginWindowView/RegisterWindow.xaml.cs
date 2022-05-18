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
        }

        private void ResizeWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void SendCode_Click(object sender, RoutedEventArgs e)
        {
            RegisterCode.Visibility = Visibility.Visible;
        }
    }
}