using System.Windows;
using System.Windows.Input;

namespace ClientApp.MVVM.View.PIWindowView
{
    /// <summary>
    /// Logika interakcji dla klasy PIWindow.xaml
    /// </summary>
    public partial class PIWindow : Window
    {
        public PIWindow()
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
    }
}