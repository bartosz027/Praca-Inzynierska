using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using Simple.Wpf.Themes;
using Simple.Wpf.Themes.Common;

using Network.Client;

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
            Client.Instance.Connect("127.0.0.1", 65535);

            var themes = new Themes();
            themes.ItemsSource = _Themes;
            themes.SelectedItem = _Themes.First(p => p.Name == "Dark theme");

            ResourceDictionary resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/ClientApp;component/LangResources/LangResources-fr.xaml", UriKind.Relative);

            this.Resources.MergedDictionaries.Add(resourceDictionary);
        }

        static IEnumerable<Theme> _Themes = new[] 
        {
            new Theme("Light theme", new Uri("/ClientApp;component/Themes/LightTheme.xaml", UriKind.Relative)),
            new Theme("Dark theme", new Uri("/ClientApp;component/Themes/DarkTheme.xaml", UriKind.Relative))
        };
    }
}