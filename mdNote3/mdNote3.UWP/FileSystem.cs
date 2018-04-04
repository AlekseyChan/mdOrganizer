using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mdOrganizer.Markdown;
using mdOrganizer.Services;
using Xamarin.Forms;


[assembly: Dependency(typeof(mdOrganizer.UWP.FileSystem))]
namespace mdOrganizer.UWP
{
    public class FileSystem : mdOrganizer.Services.IFileSystem
    {
        public async Task CloneDocumentAsync()
        {
            Windows.Storage.Pickers.FileSavePicker picker = new Windows.Storage.Pickers.FileSavePicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            picker.FileTypeChoices.Add("Markdown", new List<string>() { ".md" });
            picker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            Windows.Storage.StorageFile file = await picker.PickSaveFileAsync();
            if (file == null) return;
            Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);

            await Windows.Storage.FileIO.WriteTextAsync(file, NoteNavigator.Document.ToString(), Windows.Storage.Streams.UnicodeEncoding.Utf8);
            Settings.CurrentFile = file.Path;
        }

        private string loadTemplate()
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            System.IO.Stream stream = assembly.GetManifestResourceStream(DeviceServices.BaseResource + "." + "Templates.defaultbook.md");
            using (var reader = new System.IO.StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public async Task CreateDocumentAsync()
        {
            Windows.Storage.Pickers.FileSavePicker picker = new Windows.Storage.Pickers.FileSavePicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            picker.FileTypeChoices.Add("Markdown", new List<string>() { ".md" });
            picker.FileTypeChoices.Add("Plain Text", new List<string>() { ".txt" });
            Windows.Storage.StorageFile file = await picker.PickSaveFileAsync();
            if (file == null) return;
            Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);

            await Windows.Storage.FileIO.WriteTextAsync(file, loadTemplate(), Windows.Storage.Streams.UnicodeEncoding.Utf8);
            Settings.CurrentFile = file.Path;
        }

        public async Task ReadDocumentAsync()
        {
            if (String.IsNullOrEmpty(Settings.CurrentFile))
            {
                await DeviceServices.ExitAsync();
                return;
            }
            Windows.Storage.StorageFile file = await Windows.Storage.StorageFile.GetFileFromPathAsync(Settings.CurrentFile);
            NoteNavigator.Reset();
            NoteNavigator.Document.Parse(new System.IO.StringReader(await Windows.Storage.FileIO.ReadTextAsync(file)));
            mdOrganizer.Services.NoteNavigator.FileName = file.DisplayName;
        }

        public async Task SaveDocumentAsync()
        {
            Windows.Storage.StorageFile file = await Windows.Storage.StorageFile.GetFileFromPathAsync(Settings.CurrentFile);
            await Windows.Storage.FileIO.WriteTextAsync(file, NoteNavigator.Document.ToString(), Windows.Storage.Streams.UnicodeEncoding.Utf8);
        }

        public async Task SwitchDocumentAsync()
        {
            Windows.Storage.Pickers.FileOpenPicker picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
            picker.FileTypeFilter.Add("*");
            picker.FileTypeFilter.Add(".txt");
            picker.FileTypeFilter.Add(".md");
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();

            if (file == null) return;

            Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(file);
            Settings.CurrentFile = file.Path;
        }

        public async Task ExitAsync()
        {
            mdOrganizer.UWP.App.Current.Exit();
        }
    }
}
