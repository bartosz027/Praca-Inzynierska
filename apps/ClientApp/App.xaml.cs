using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

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
            this.ConnectToServer(this, null);

            // Load application settings
            var theme = ConfigManager.GetValue("Theme");
            var language = ConfigManager.GetValue("Language");

            var themes = new Themes {
                ItemsSource = Themes,
                SelectedItem = Themes.FirstOrDefault(p => p.Name == theme)
            };

            var dictionary = new ResourceDictionary() {
                Source = new Uri("/ClientApp;component/Resources/Languages/" + language + ".xaml", UriKind.Relative)
            };

            this.Resources.MergedDictionaries.Add(dictionary);
        }

        private void ConnectToServer(object sender, EventArgs e) {
            Client.Instance.UnsubscribeAllEvents();

            var ip = ConfigManager.GetValue("Server_IP");
            var port = ConfigManager.GetValue("Server_PORT");

            // Connect to server
            while(Client.Data.TCP.Connected == false) {
                try {
                    Client.Instance.Connect(ip, int.Parse(port));
                    Client.Instance.EnableSecureConnection();

                    while(Client.Data.IsConnectedViaAES == false) {
                        Thread.Sleep(25);
                    }
                }
                catch {
                    Thread.Sleep(5000);
                }
            }

            // Login via access token
            Client.Instance.ConnectionLost += ConnectToServer;
            Client.Instance.ResponseReceived += OnResponseReceived;

            if (String.IsNullOrEmpty(Client.Data.AccessToken)) {
                Client.Data.AccessToken = ConfigManager.GetValue("AccessToken");
            }

            Client.Instance.SendRequest(new VerifyAccessTokenRequest());
        }

        private IEnumerable<Theme> Themes = new[] {
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
            BaseWindow window = null;

            Client.Data.ID = response.ID;
            Client.Data.Status = response.Status;
            Client.Data.Username = response.Username;

            switch(response.Result) {
                case ResponseResult.Success: {
                    byte[] data = Encoding.ASCII.GetBytes(Client.Data.ID.ToString());
                    Client.UDPClient.Send(data, data.Length, Client.ServerEndPoint);

                    window = new MainWindow();
                    break;
                }
                case ResponseResult.Failure: {
                    window = new LoginWindow();
                    break;
                }
            }

            window.Show();
            MainWindow = window;

            for (int i = 0; i < Current.Windows.Count - 2; i++) {
                Current.Windows[0].Close();
            }
        }

        private void OnLogoutResponse(LogoutResponse response) {
            Client.Instance.UnsubscribeAllEvents();

            Client.Instance.ConnectionLost += ConnectToServer;
            Client.Instance.ResponseReceived += OnResponseReceived;

            Client.Data.ID = 0;
            Client.Data.Status = false;
            Client.Data.Username = null;
            Client.Data.AccessToken = null;
            Client.Data.ExternalEndPoint = null;

            var login_window = new LoginWindow();
            login_window.Show();

            var main_window = App.Current.MainWindow as BaseWindow;
            main_window.Close();

            App.Current.MainWindow = login_window;
        }
    }

}