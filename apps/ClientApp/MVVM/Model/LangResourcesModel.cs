using ClientApp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.MVVM.Model
{
    internal class LangResourcesModel : ObservableObject
    {
        public string LangResourcesName
        {
            get { return _LangResourcesName; }
            set
            {
                _LangResourcesName = value;
                OnPropertyChanged();
            }
        }
        private string _LangResourcesName;

        public string Language
        {
            get { return _Language; }
            set
            {
                _Language = value;
                OnPropertyChanged();
            }
        }
        private string _Language;
    }
}
