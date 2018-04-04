using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using mdOrganizer.Services;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Threading.Tasks;
using Plugin.CurrentActivity;

[assembly: Dependency(typeof(mdOrganizer.Droid.FileSystem))]
namespace mdOrganizer.Droid
{
    public class FileSystem : IFileSystem
    {
        public async Task CloneDocumentAsync()
        {
            await ((MainActivity)CrossCurrentActivity.Current.Activity).CloneDocumentAsync();
        }

        public async Task CreateDocumentAsync()
        {
            await ((MainActivity)CrossCurrentActivity.Current.Activity).CreateDocumentAsync();
        }

        public async Task ExitAsync()
        {
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }

        public async Task ReadDocumentAsync()
        {
            await((MainActivity)CrossCurrentActivity.Current.Activity).ReadDocumentAsync();
        }

        public async Task SaveDocumentAsync()
        {
            await ((MainActivity)CrossCurrentActivity.Current.Activity).SaveDocumentAsync();
        }

        public async Task SwitchDocumentAsync()
        {
            await ((MainActivity)CrossCurrentActivity.Current.Activity).SwitchDocumentAsync();
        }
    }
}