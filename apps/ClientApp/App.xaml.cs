using Network.Client;
using System;
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
            //Client.Instance.Connect("127.0.0.1", 65535);
        }
    }
}