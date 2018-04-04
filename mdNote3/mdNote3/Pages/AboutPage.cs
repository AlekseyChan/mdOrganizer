using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mdOrganizer.UI;
using Xamarin.Forms;

namespace mdOrganizer.Pages
{
    public class AboutPage : ContentPage
    {
        private static AboutPage _aboutPage = null;
        public static AboutPage About
        {
            get
            {
                if (_aboutPage == null) _aboutPage = new AboutPage();
                return _aboutPage;
            }
        }

        public AboutPage()
        {

            Title = Strings.Loc.AboutPage_Title;
            Label label = new Label()
            {
                Text = Strings.Loc.AppSite,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0)
            };
            TapGestureRecognizer tapRecognizer = new TapGestureRecognizer();
            tapRecognizer.Tapped += (s, e) =>
            {
                Device.OpenUri(new System.Uri(Strings.Loc.AppSite));
            };

            label.GestureRecognizers.Add(tapRecognizer);

            Content = new StackLayout
            {
                Children = {
                    new AppTitleView()
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    },
                    new Label { Text =  Strings.Loc.AboutPage_AuthorLabel,
                        HorizontalOptions = LayoutOptions.CenterAndExpand,
                        HorizontalTextAlignment = TextAlignment.Center,
                        Margin = new Thickness(0, 20, 0, 0)
                    },
                    label,
                }
            };
        }
    }
}