using System;
using System.Collections.Generic;
using System.Linq;

using ClientApp.Core;
using Simple.Wpf.Themes.Common;

namespace ClientApp.MVVM.ViewModel.LoginWindowViewModel
{
    internal class LoginWindowViewModel : ObservableObject
    {
        public LoginWindowViewModel()
        {
            Themes = new[]
            {
                new Theme("Light theme",
                    new Uri("/ClientApp;component/Themes/LightTheme.xaml", UriKind.Relative)),
                new Theme("Dark theme",
                    new Uri("/ClientApp;component/Themes/DarkTheme.xaml", UriKind.Relative))
            };

            _SelectedTheme = Themes.First();
        }
        public IEnumerable<Theme> Themes { get; }

        public Theme SelectedTheme
        {
            get { 
                return _SelectedTheme; 
            }
            set
            {
                _SelectedTheme = value;
                OnPropertyChanged();
            }
        }
        private Theme _SelectedTheme;
    }
}