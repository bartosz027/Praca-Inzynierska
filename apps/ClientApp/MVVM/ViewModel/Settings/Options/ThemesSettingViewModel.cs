using System;
using System.Collections.Generic;
using System.Linq;

using ClientApp.Core;
using Network.Shared.Core;

using Simple.Wpf.Themes;
using Simple.Wpf.Themes.Common;

namespace ClientApp.MVVM.ViewModel.Settings.Options {

    internal class ThemesSettingViewModel : ObservableObject {
        public ThemesSettingViewModel() {
            var themes = new Themes();

            ThemesList = new[] {
                new Theme("LightTheme", new Uri("/ClientApp;component/Resources/Themes/LightTheme.xaml", UriKind.Relative)),
                new Theme("DarkTheme", new Uri("/ClientApp;component/Resources/Themes/DarkTheme.xaml", UriKind.Relative))
            };

            LightThemeButtonCommand = new RelayCommand(o => {
                themes.ItemsSource = ThemesList;
                themes.SelectedItem = ThemesList.First(p => p.Name == "LightTheme");

                ConfigManager.SetValue("Theme", "LightTheme");
            });

            DarkThemeButtonCommand = new RelayCommand(o => {
                themes.ItemsSource = ThemesList;
                themes.SelectedItem = ThemesList.First(p => p.Name == "DarkTheme");

                ConfigManager.SetValue("Theme", "DarkTheme");
            });
        }

        // Commands
        public RelayCommand LightThemeButtonCommand { get; set; }
        public RelayCommand DarkThemeButtonCommand { get; set; }

        // Themes
        public IEnumerable<Theme> ThemesList { get; private set; }
    }

}