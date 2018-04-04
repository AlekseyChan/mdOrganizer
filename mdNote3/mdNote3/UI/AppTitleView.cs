using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mdOrganizer.UI;
using Xamarin.Forms;

namespace mdOrganizer.UI
{
    public class AppTitleView : ContentView
    {
        public event EventHandler OnTap;

        private TapGestureRecognizer tapRecognizer;

        public AppTitleView()
        {
            BackgroundColor = Styles.AppTitleBackgroundColor;
            Padding = Styles.AppTitlePadding;

            Label shadow = new Label
            {
                FontSize = Styles.AppTitleSize,
                Text = Strings.Loc.AppTitle,
                FontAttributes = FontAttributes.Bold,
                TextColor = Styles.AppTitleShadow,
                TranslationX = Styles.AppTitleShadowTilt,
                TranslationY = Styles.AppTitleShadowTilt
            };

            Label mainTitle = new Label
            {
                FontSize = Styles.AppTitleSize,
                Text = Strings.Loc.AppTitle,
                FontAttributes = FontAttributes.Bold,
                TextColor = Styles.AppTitleColor
            };

            Content = new Grid()
            {
                Children = { shadow, mainTitle }
            };

            tapRecognizer = new TapGestureRecognizer();
            tapRecognizer.Tapped += TapRecognizer_Tapped;
            GestureRecognizers.Add(tapRecognizer);
        }

        private void TapRecognizer_Tapped(object sender, EventArgs e)
        {
            OnTap?.Invoke(this, e);
        }
    }
}
