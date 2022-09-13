using System;
using System.Collections.Generic;
using System.Threading;

using System.Linq;
using System.Windows;

using ClientApp.Core;

using ClientApp.MVVM.View;
using ClientApp.MVVM.View.AppStartup;

using Simple.Wpf.Themes;
using Simple.Wpf.Themes.Common;

using Network.Client;
using Network.Client.DataProcessing;

using Network.Shared.Core;
using Network.Shared.DataTransfer.Base;

using Network.Shared.DataTransfer.Model.Account.VerifyAccessToken;
using Network.Shared.DataTransfer.Model.Account.Logout;

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

            // Login via access token
            Client.Data.AccessToken = ConfigManager.GetValue("AccessToken");

            while(Client.Data.IsConnectedViaAES == false) {
                Thread.Sleep(125);
            }

            Client.Instance.ResponseReceived += OnResponseReceived;
            Client.Instance.SendRequest(new VerifyAccessTokenRequest());
        }

        private static IEnumerable<Theme> Themes = new[] {
            new Theme("LightTheme", new Uri("/ClientApp;component/Resources/Themes/LightTheme.xaml", UriKind.Relative)),
            new Theme("DarkTheme", new Uri("/ClientApp;component/Resources/Themes/DarkTheme.xaml", UriKind.Relative))
        };

        // Response events
        private void OnResponseReceived(object sender, Response response) {
            App.Current.Dispatcher.Invoke(delegate {
                var dispatcher = new ResponseDispatcher(response);

                if (response.Result == ResponseResult.None) {
                    throw new NotImplementedException();
                }

                dispatcher.Dispatch<VerifyAccessTokenResponse>(OnVerifyAccessTokenResponse);
                dispatcher.Dispatch<LogoutResponse>(OnLogoutResponse);
            });
        }

        private void OnVerifyAccessTokenResponse(VerifyAccessTokenResponse response) {
            Client.Data.Username = response.Username;

            if (response.Result == ResponseResult.Success) {
                var main_window = new MainWindow();
                main_window.Show();
            }

            if (response.Result == ResponseResult.Failure) {
                var login_window = new LoginWindow();
                login_window.Show();
            }
        }

        private void OnLogoutResponse(LogoutResponse response) {
            Client.Instance.UnsubscribeAllEvents();
            Client.Instance.ResponseReceived += OnResponseReceived;

            Client.Data.AccessToken = null;
            Client.Data.Username = null;

            var login_window = new LoginWindow();
            login_window.Show();

            var main_window = App.Current.MainWindow as BaseWindow;
            main_window.Close();

            App.Current.MainWindow = login_window;
        }
    }

}