using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApp.Resources.Languages
{
    public static class ResourcesDictionary
    {
        public static string EnterEmail = "EnterEmail_String"; //TODO: dodac wszystkie resouces
        //< !--Okrzyki rejestracja-- >
        public static string NotValidEmail = "NotValidEmail_String";
        public static string RegisterNotAllData = "RegisterNotAllData_String";
        public static string RegisterEmailExist = "RegisterEmailExist_String";
        public static string RegisterUsernameTooShort = "RegisterUsernameTooShort_String";
        public static string RegisterWeekPassword = "RegisterWeekPassword_String";

        //< !--Okrzyki Kod-- >
        public static string IncorrectCode = "IncorrectCode_String";
        public static string ExpiredCode = "ExpiredCode_String";
        public static string EmptyCode = "EmptyCode_String";

        //<!-- Okrzyki walidacyjne nowe haslo -->
        public static string NotSamePassword = "NotSamePassword_String";

        //< !--Jezyki-- >
        public static string LANGUAGE_PL = "LangResources-pl";
        public static string LANGUAGE_FR = "LangResources-fr";
    }
}
//< !--Okrzyki walidacyjne kod -->
//    <system:String x:Key = "IncorrectCode_String" > Kod nieprawidłowy.</ system:String >
     
//         < system:String x:Key = "ExpiredCode_String" > Kod nieprawidłowy.</ system:String >