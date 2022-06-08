using System;
using System.Collections.Generic;
using System.Linq;

using ClientApp.Core;

using Simple.Wpf.Themes;
using Simple.Wpf.Themes.Common;

namespace ClientApp.MVVM.ViewModel.PIWindowViewModel.SettingsViewModels
{
    internal class ThemesSettingViewModel : ObservableObject
    {
        public ThemesSettingViewModel()
        {
            var themes = new Themes();

            ThemesList = new[]
            {
                new Theme("Light theme", new Uri("/ClientApp;component/Themes/LightTheme.xaml", UriKind.Relative)),
                new Theme("Dark theme", new Uri("/ClientApp;component/Themes/DarkTheme.xaml", UriKind.Relative))
            };

            LightThemeButtonCommand = new RelayCommand(o =>
            {
                themes.ItemsSource = ThemesList;
                themes.SelectedItem = ThemesList.First(p=>p.Name == "Light theme");
            });

            DarkThemeButtonCommand = new RelayCommand(o =>
            {
                themes.ItemsSource = ThemesList;
                themes.SelectedItem = ThemesList.First(p => p.Name == "Dark theme");
            });
        }

        // Themes
        public IEnumerable<Theme> ThemesList { get; private set; }

        // Commands
        public RelayCommand LightThemeButtonCommand { get; set; }
        public RelayCommand DarkThemeButtonCommand { get; set; }
    }
}