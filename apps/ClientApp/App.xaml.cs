using Network.Client;
using Simple.Wpf.Themes;
using Simple.Wpf.Themes.Common;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ClientApp
{
    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // LUX
            Client.Instance.Connect("127.0.0.1", 65535);

            IEnumerable<Theme> Themes = new[]
            {
                new Theme("Light theme",
                    new Uri("/ClientApp;component/Themes/LightTheme.xaml", UriKind.Relative)),
                new Theme("Dark theme",
                    new Uri("/ClientApp;component/Themes/DarkTheme.xaml", UriKind.Relative))
            };

            Themes themes = new Themes();
            themes.ItemsSource = Themes;
            themes.SelectedItem = new Theme("Dark theme", new Uri("/ClientApp;component/Themes/DarkTheme.xaml", UriKind.Relative));
        }
        
    }
}