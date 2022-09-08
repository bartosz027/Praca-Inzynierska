using System;
using System.Collections.Generic;

using System.Linq;
using System.Windows;

using Simple.Wpf.Themes;
using Simple.Wpf.Themes.Common;

using Network.Client;
using Network.Shared.Core;

namespace ClientApp {

    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : Application {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            var ip = ConfigManager.GetValue("Server_IP");
            var port = ConfigManager.GetValue("Server_PORT");

            Client.Instance.Connect(ip, int.Parse(port));
            Client.Instance.EnableSecureConnection();

            // Load application settings
            var theme = ConfigManager.GetValue("Theme");
            var language = ConfigManager.GetValue("Language");

            var themes = new Themes {
                ItemsSource = Themes,
                SelectedItem = Themes.FirstOrDefault(p => p.Name == theme)
            };

            var dictionary = new ResourceDictionary();
            dictionary.Source = new Uri("/ClientApp;component/Resources/Languages/" + language + ".xaml", UriKind.Relative);

            this.Resources.MergedDictionaries.Add(dictionary);
        }

        public static IEnumerable<Theme> Themes = new[] {
            new Theme("LightTheme", new Uri("/ClientApp;component/Resources/Themes/LightTheme.xaml", UriKind.Relative)),
            new Theme("DarkTheme", new Uri("/ClientApp;component/Resources/Themes/DarkTheme.xaml", UriKind.Relative))
        };
    }

}