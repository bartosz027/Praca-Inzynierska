using System;
using System.Collections.ObjectModel;

using System.Linq;
using System.Windows;

using ClientApp.Core;
using ClientApp.Resources;

using Network.Shared.Core;

namespace ClientApp.MVVM.ViewModel.Settings.Options  {

    internal class LanguageInfo {
        public string Filename { get; set; }
        public string ResourceName { get; set; }
    }

    internal class LanguageSettingsViewModel : ObservableObject {
        public LanguageSettingsViewModel() {
            LanguageList = new ObservableCollection<LanguageInfo> {
                new LanguageInfo { Filename = "Lang-pl", ResourceName = ResourceManager.GetValue(ResourcesDictionary.LANGUAGE_PL)},
                new LanguageInfo { Filename = "Lang-fr", ResourceName = ResourceManager.GetValue(ResourcesDictionary.LANGUAGE_FR)}
            };

            var language = ConfigManager.GetValue("Language");
            SelectedLanguage = LanguageList.FirstOrDefault(p => p.Filename == language);
        }

        // Properties
        public ObservableCollection<LanguageInfo> LanguageList { 
            get {
                return _LanguageList;
            }
            set {
                _LanguageList = value;
            }
        }
        private ObservableCollection<LanguageInfo> _LanguageList;

        public LanguageInfo SelectedLanguage {
            get { 
                return _SelectedLanguage; 
            }
            set {
                _SelectedLanguage = value;
                OnPropertyChanged();

                if (SelectedLanguage != null) {
                    var dictionary = new ResourceDictionary();
                    dictionary.Source = new Uri("/ClientApp;component/Resources/Languages/" + SelectedLanguage.Filename + ".xaml", UriKind.Relative);

                    Application.Current.Resources.MergedDictionaries.Add(dictionary);
                    ConfigManager.SetValue("Language", SelectedLanguage.Filename);
                }
            }
        }
        private LanguageInfo _SelectedLanguage;
    }

}