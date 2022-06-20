using ClientApp.Core;
using ClientApp.MVVM.Model;
using ClientApp.MVVM.ViewModel.PIWindowViewModel.ContactsViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ClientApp.MVVM.ViewModel.PIWindowViewModel.SettingsViewModels
{
    internal class LanguageSettingsViewModel :ObservableObject
    {
        public LanguageSettingsViewModel()
        {
            LanguageList = new ObservableCollection<LangResourcesModel> 
            { 
                new LangResourcesModel { LangResourcesName = "LangResources-pl", Language =  Application.Current.Resources["LangResources-pl"].ToString()},
                new LangResourcesModel { LangResourcesName = "LangResources-fr", Language =  Application.Current.Resources["LangResources-fr"].ToString()}};
            
        }
        // Obserable properties
        public ObservableCollection<LangResourcesModel> LanguageList { get; set; }

        
        // Obserable properties
        public LangResourcesModel SelectedLanguage
        {
            get { return _SelectedLanguage; }
            set
            {
                _SelectedLanguage = value;
                OnPropertyChanged();
                if (SelectedLanguage != null)
                    SetLanguage();
            }
        }
        private LangResourcesModel _SelectedLanguage;

        private void SetLanguage() 
        {
            ResourceDictionary resourceDictionary = new ResourceDictionary();
            resourceDictionary.Source = new Uri("/ClientApp;component/LangResources/" + SelectedLanguage.LangResourcesName + ".xaml", UriKind.Relative);

            Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            ConfigManager.SetSetting("LanguageResource", SelectedLanguage.LangResourcesName);
        }
    }
}
