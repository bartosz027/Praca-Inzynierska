using System.Configuration;
using System.Windows;

namespace Network.Shared.Core {

    public static class ConfigManager {
        public static string GetValue(string key) {
            return ConfigurationManager.AppSettings[key];
        }

        public static void SetValue(string key, string value) {
            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            configuration.AppSettings.Settings[key].Value = value;
            configuration.Save(ConfigurationSaveMode.Minimal);

            ConfigurationManager.RefreshSection("appSettings");
        }
    }

}