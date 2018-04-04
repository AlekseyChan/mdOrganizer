using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace mdOrganizer.UI
{
    public class Styles
    {
        public static double DetailsFactor = 0.7;
        public static double FontSize = 15;
        public static double IconGap = 27;
        public static double MenuHeight = 20;
        public static bool FixedGap = true;

        public static Thickness SeparatorMargin = new Thickness(10, 0);
        public static Color SeparatorColor = Color.FromHex("C0C0C0");
        public static int SeparatorHeight = 1;

        public static Color MenuBackground = Color.FromHex("00263D");
        public static Color MenuColor = Color.FromHex("FAFAFA");

        public static Color EntryBackground = Color.FromHex("A0A0A0");

        public static Thickness Padding = new Thickness(20, 10);
        public static double ControlSpacing = 10;
        public static Thickness ButtonPadding = new Thickness(16, 14);


        public static Color AppTitleBackgroundColor = Color.FromHex("B24F88");
        public static Color AppTitleColor = Color.WhiteSmoke;
        public static Color AppTitleShadow = Color.FromHex("51243E");
        public static Thickness AppTitlePadding = new Thickness(20, 60, 20, 10);
        public static double AppTitleSize = 36;
        public static int AppTitleShadowTilt = 2;

        /*        public static double CaptionFontSize = 10.5;
                public static Color BackgroundColor = Color.FromHex("004f88");
                public static Color TextColor = Color.FromHex("fafafa");
                public static Color CaptionBackgroundColor = Color.FromHex("004f88");
                public static Color CaptionColor = Color.FromHex("C0C0C0");
                public static Thickness CaptionMargin = new Thickness(5, 10, 5, 0);*/
    }
}
