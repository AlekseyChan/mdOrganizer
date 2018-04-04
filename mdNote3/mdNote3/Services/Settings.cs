using System;
using System.Collections.Generic;
using System.Text;

namespace mdOrganizer.Services
{
    public class Settings
    {
        public static int GetValue(string key, int defValue)
        {
            if (App.Current.Properties.ContainsKey(key))
                return (int)App.Current.Properties[key];
            return defValue;
        }

        public static bool GetValue(string key, bool defValue = true)
        {
            if (App.Current.Properties.ContainsKey(key))
                return (bool)App.Current.Properties[key];
            return defValue;
        }

        public static string GetValue(string key, string defValue = null)
        {
            if (App.Current.Properties.ContainsKey(key))
                return (string)App.Current.Properties[key];
            return defValue;
        }

        public static void SetValue(string key, object value)
        {
            App.Current.Properties[key] = value;
            App.Current.SavePropertiesAsync();
        }

        public static int BaseFontSize
        {
            get => GetValue("BaseFontSize", 14);
            set => SetValue("BaseFontSize", value);
        }

        public static bool PreviewMode
        {
            get => GetValue("PreviewMode", true);
            set => SetValue("PreviewMode", value);
        }

        public static bool ManualCategory
        {
            get => GetValue("ManualCategory", false);
            set => SetValue("ManualCategory", value);
        }
        
        public static string CurrentFile
        {
            get => GetValue("CurrentFile", "");
            set
            {
                SetValue("CurrentFile", value);
                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () => { await DeviceServices.ReadDocumentAsync(); });
                
            }
        }

        private static Dictionary<string, string> supportedLanguages = new Dictionary<string, string>()
        {
            { "en", "English"},
            { "ru", "Русский"}
        };
        public static Dictionary<string, string> SupportedLanguages { get => supportedLanguages;  }

        public static string Language {
            get {
                string currentLang = System.Globalization.CultureInfo.CurrentCulture.Name;
                if (!SupportedLanguages.ContainsKey(currentLang))
                    currentLang = "en";
                return GetValue("Language", currentLang);
            }
            set
            {
                SetValue("Language", value);
            }
        }

    }
}
