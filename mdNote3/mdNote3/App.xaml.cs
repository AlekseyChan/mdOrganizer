using System;
using System.Reflection;
using Xamarin.Forms;

namespace mdOrganizer
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            var assembly = typeof(App).GetTypeInfo().Assembly;
            foreach (var res in assembly.GetManifestResourceNames())
                System.Diagnostics.Debug.WriteLine("found resource: " + res);
            Strings.Loc.Culture = new System.Globalization.CultureInfo(Services.Settings.Language);

            MainPage = new Pages.MainPage();
            if (String.IsNullOrEmpty(mdOrganizer.Services.Settings.CurrentFile))
            {
                mdOrganizer.Services.DeviceServices.CreateDocumentAsync();
            }
            else
            {
                mdOrganizer.Services.DeviceServices.ReadDocumentAsync();
            }
        }
    }
}