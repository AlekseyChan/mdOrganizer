using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mdOrganizer.UI;
using Xamarin.Forms;

namespace mdOrganizer.Pages
{
    public class MainPage : MasterDetailPage
    {
        public static NavigationPage Navigator;
        public static MenuView Menu { get; set; }

        public MainPage()
        {
            Navigator = new NavigationPage(ViewerPage.Viewer);

            Menu = new MenuView();

            Menu.AddMenuItem(Strings.Loc.Menu_NewFile, "", "newfile.png", async (o) =>
            {
                await mdOrganizer.Services.DeviceServices.CreateDocumentAsync();
                Menu.BlockTap = false;
            }, null);
            Menu.AddMenuItem(Strings.Loc.Menu_OpenFile, "", "openfile.png", async (o) =>
            {
                await mdOrganizer.Services.DeviceServices.SwitchDocumentAsync();
                Menu.BlockTap = false;
            }, null);
            Menu.AddMenuItem(Strings.Loc.Menu_CloneFile, "", "clonefile.png", async (o) =>
            {
                await mdOrganizer.Services.DeviceServices.CloneDocumentAsync();
                Menu.BlockTap = false;
            }, null);

            Menu.AddMenuItem(Strings.Loc.Menu_Settings, null, "settings.png", (o) =>
            {
                Navigator.PushAsync(SettingsPage.Settings, true);
                Menu.BlockTap = false;
            }, null);

            Menu.AddMenuItem(Strings.Loc.Menu_Exit, null, "exit.png", async (o) =>
            {
                await Services.DeviceServices.ExitAsync();
                Menu.BlockTap = false;
            }, null);

            if (Device.RuntimePlatform.Equals(Device.Android))
            {
                AppTitleView AboutCommand = new AppTitleView();
                AboutCommand.OnTap += (s, e) =>
                {
                    Navigator.PushAsync(AboutPage.About, true);
                    Menu.BlockTap = false;
                };

                Menu.SetHeader(AboutCommand);
            } else {
                Menu.AddMenuItem(Strings.Loc.Menu_About, null, "about.png", (o) =>
                {
                    Navigator.PushAsync(AboutPage.About, true);
                    Menu.BlockTap = false;
                }, null);
            }

            Menu.OnMenuTap += (s, e) =>
            {
                IsPresented = false;
            };

            Master = new ContentPage { Title = Strings.Loc.AppTitle, Content = Menu, BackgroundColor = Styles.MenuBackground };
            Master.Appearing += Master_Appearing;

            Navigator.Pushed += (s, e) =>
            {
                IsPresented = false;
                Menu.BlockTap = false;
            };
            Navigator.Popped += (s, e) =>
            {
                IsPresented = false;
                Menu.BlockTap = false;
            };

            Detail = Navigator;

            MasterBehavior = MasterBehavior.Popover;
        }

        private void Master_Appearing(object sender, EventArgs e)
        {
            Menu.BlockTap = false;
        }
    }
}