using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace mdOrganizer.Services
{
    public class DeviceServices
    {
        public static async Task CreateDocumentAsync()
        {
            await FileSystem.CreateDocumentAsync();
        }

        public static async Task SwitchDocumentAsync()
        {
            await FileSystem.SwitchDocumentAsync();
        }

        public static async Task SaveDocumentAsync()
        {
            await FileSystem.SaveDocumentAsync();
        }

        public static async Task CloneDocumentAsync()
        {
            await FileSystem.CloneDocumentAsync();
        }

        public static async Task ReadDocumentAsync()
        {
            await FileSystem.ReadDocumentAsync();
            mdOrganizer.Pages.ViewerPage.Viewer.RefreshWebView();
        }

        public static async Task ExitAsync()
        {
            await FileSystem.ExitAsync();
        }


        public static IFileSystem FileSystem = DependencyService.Get<IFileSystem>();

        public static string BaseUrl = DependencyService.Get<IBase>().GetBaseUrl();
        public static string BaseResource = DependencyService.Get<IBase>().GetBaseResource();
    }
}
