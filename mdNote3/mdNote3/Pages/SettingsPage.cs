using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mdOrganizer.UI;
using Xamarin.Forms;

namespace mdOrganizer.Pages
{
    public class SettingsPage : ContentPage
    {
        private static SettingsPage _settings = null;
        public static SettingsPage Settings
        {
            get
            {
                if (_settings == null) _settings = new SettingsPage();
                return _settings;
            }
        }
        private Switch initSwitch(string key, bool defValue)
        {
            Switch result = new Switch
            {
                IsToggled = mdOrganizer.Services.Settings.GetValue(key, defValue),
                Margin = new Thickness(0, 0, 0, Styles.ControlSpacing),
            };
            result.Toggled += (s, e) => { mdOrganizer.Services.Settings.SetValue(key, result.IsToggled); };
            return result;
        }

        private Picker LanguageSelector;
        public SettingsPage()
        {
            Title = Strings.Loc.SettingsPage_Title;

            StackLayout ControlsStack = new StackLayout
            {
                Padding = Styles.Padding,
            };


            LanguageSelector = new Picker()
            {
                Title = Strings.Loc.SettingsPage_ChooseLanguage,
            };

            int i = -1;
            string currentLang = mdOrganizer.Services.Settings.Language;
            foreach (string lang in Services.Settings.SupportedLanguages.Keys)
            {
                i++;
                LanguageSelector.Items.Add(Services.Settings.SupportedLanguages[lang]);
                if (lang.Equals(currentLang))
                    LanguageSelector.SelectedIndex = i;
            }
            LanguageSelector.SelectedIndexChanged += LanguageSelector_SelectedIndexChanged;


            //            ControlsStack.Children.Add(new Label { Text = "Always manually choose category before adding note on \"Share...\" " });
            ControlsStack.Children.Add(new Label {
                Text = Strings.Loc.SettingsPage_ManualCategoryLabel,
                Margin = new Thickness(0, Styles.ControlSpacing, 0, 0)
            });
            ControlsStack.Children.Add(initSwitch("ManualCategory", mdOrganizer.Services.Settings.ManualCategory));

            ControlsStack.Children.Add(new Label
            {
                Text = Strings.Loc.SettingsPage_RestartSettings,
                Margin = new Thickness(0, Styles.ControlSpacing, 0, 0)
            });
            ControlsStack.Children.Add(LanguageSelector);

            Content = ControlsStack;
        }

        private void LanguageSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            Services.Settings.Language = Services.Settings.SupportedLanguages.Keys.ElementAt(LanguageSelector.SelectedIndex);
        }
    }
}