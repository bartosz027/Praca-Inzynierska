using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClientApp.Resources.Languages
{
    public static class ResourcesManager
    {
        public static string GetValue(string key) 
        {
            return Application.Current.Resources[key].ToString();
        }
    }
}
