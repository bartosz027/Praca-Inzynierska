using System;
using System.Windows;

namespace ClientApp.Resources {

    public static class ResourceManager {
        public static string GetValue(string key) {
            return Application.Current.Resources[key].ToString();
        }

        public static string GetValue(string key, params object [] values) {
            var baseValue = Application.Current.Resources[key].ToString();
            return String.Format(baseValue, values);
        }
    }

}