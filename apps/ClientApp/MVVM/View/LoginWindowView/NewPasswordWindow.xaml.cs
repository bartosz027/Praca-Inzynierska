using System.Windows;
using System.Windows.Input;

namespace ClientApp.MVVM.View.LoginWindowView
{
    /// <summary>
    /// Logika interakcji dla klasy NewPasswordWindow.xaml
    /// </summary>
    public partial class NewPasswordWindow : Window
    {
        public NewPasswordWindow()
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}