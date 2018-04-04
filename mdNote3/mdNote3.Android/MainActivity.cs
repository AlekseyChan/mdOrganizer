using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Reflection;

namespace mdOrganizer.Droid
{
    [Activity(Label = "mdOrganizer.Android", Icon = "@drawable/icon", Theme = "@style/splashscreen", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [IntentFilter(new[] { Android.Content.Intent.ActionSend }, Categories = new[] { Android.Content.Intent.CategoryDefault }, DataMimeType = "text/*")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public const int REQUEST_CODE_OPEN_FILE = 2;
        public const int REQUEST_CODE_CLONE_FILE = 3;
        public const int REQUEST_CODE_CREATE_FILE = 4;

        private static bool Initialized = false;

        private void PrepareIntent(Intent intent)
        {
            intent.AddCategory(Intent.CategoryOpenable);
            intent.SetType("*/*");
        }

        public async Task CreateDocumentAsync()
        {
            var intent = new Intent(Intent.ActionCreateDocument);
            PrepareIntent(intent);
            intent.PutExtra(Intent.ExtraTitle, "New file");
            StartActivityForResult(intent, REQUEST_CODE_CREATE_FILE);
        }

        public async Task SwitchDocumentAsync()
        {
            var intent = new Intent(Intent.ActionOpenDocument);
            PrepareIntent(intent);
            StartActivityForResult(intent, REQUEST_CODE_OPEN_FILE);
        }

        private async Task writeContent(string file, string content)
        {
            System.IO.Stream stream = null;
            System.IO.StreamWriter writer = null;
            Android.Net.Uri uri = Android.Net.Uri.Parse(file);
            try
            {
                stream = ContentResolver.OpenOutputStream(uri);
                writer = new System.IO.StreamWriter(stream, System.Text.Encoding.UTF8);
                await writer.WriteAsync(content);
                writer.Close();
                stream.Close();
            }
            finally
            {
                if (writer != null) writer.Dispose();
                if (stream != null) stream.Dispose();
            }

        }

        public async Task SaveDocumentAsync()
        {
            await writeContent(mdOrganizer.Services.Settings.CurrentFile, mdOrganizer.Services.NoteNavigator.Document.ToString());
        }

        public async Task ReadDocumentAsync()
        {
            if (String.IsNullOrEmpty(mdOrganizer.Services.Settings.CurrentFile))
            {
                await mdOrganizer.Services.DeviceServices.ExitAsync();
                return;
            }
            Android.Net.Uri uri = Android.Net.Uri.Parse(mdOrganizer.Services.Settings.CurrentFile);
            System.IO.Stream stream = null;
            System.IO.StreamReader reader = null;
            try
            {
                stream = ContentResolver.OpenInputStream(uri);
                reader = new System.IO.StreamReader(stream, System.Text.Encoding.UTF8);
                mdOrganizer.Services.NoteNavigator.Reset();
                mdOrganizer.Services.NoteNavigator.Document.Parse(new System.IO.StringReader(await reader.ReadToEndAsync()));
                reader.Close();
                stream.Close();
            }
            finally
            {
                if (reader != null) reader.Dispose();
                if (stream != null) stream.Dispose();
            }
            mdOrganizer.Services.NoteNavigator.FileName = System.IO.Path.GetFileNameWithoutExtension(uri.Path);
        }

        public async Task CloneDocumentAsync()
        {
            var intent = new Intent(Intent.ActionCreateDocument);
            PrepareIntent(intent);
            StartActivityForResult(intent, REQUEST_CODE_CLONE_FILE);

        }

        private string loadTemplate()
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;
            System.IO.Stream stream = assembly.GetManifestResourceStream(mdOrganizer.Services.DeviceServices.BaseResource + "." + "Templates.defaultbook.md");
            using (var reader = new System.IO.StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        protected override async void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode != Result.Ok) return;
            string newPath = data?.Data?.ToString();
            switch (requestCode)
            {
                case REQUEST_CODE_OPEN_FILE:
                    mdOrganizer.Services.Settings.CurrentFile = newPath;
                    break;
                case REQUEST_CODE_CLONE_FILE:
                    await writeContent(newPath, mdOrganizer.Services.NoteNavigator.Document.ToString());
                    mdOrganizer.Services.Settings.CurrentFile = newPath;
                    break;
                case REQUEST_CODE_CREATE_FILE:
                    await writeContent(newPath, loadTemplate());
                    mdOrganizer.Services.Settings.CurrentFile = newPath;
                    break;
            }

        }

        protected override async void OnCreate(Bundle bundle)
        {
            base.SetTheme(Resource.Style.MyTheme_Base);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            LoadApplication(new App());
            if (Intent.Action.Equals(Android.Content.Intent.ActionSend))
            {
                string content = Intent.GetStringExtra(Android.Content.Intent.ExtraHtmlText);
                if (String.IsNullOrEmpty(content))
                    content = Intent.GetStringExtra(Android.Content.Intent.ExtraText);
                string title = Intent.GetStringExtra(Android.Content.Intent.ExtraSubject);
                if (content.StartsWith("http://") || content.StartsWith("https://"))
                    content = "[" + (String.IsNullOrEmpty(title) ? "link" : title) + "](" + content + ")";
                mdOrganizer.Pages.ViewerPage.AutoCreateInfo = new Pages.AutoCreateInfo()
                {
                    Caption = title,
                    Content = content
                };
//                if (Initialized)
                {
                    await mdOrganizer.Services.DeviceServices.ReadDocumentAsync();
                    if (mdOrganizer.Services.Settings.ManualCategory)
                    {
                        await mdOrganizer.Pages.MainPage.Navigator.PopToRootAsync();
                        mdOrganizer.Pages.ViewerPage.Viewer.RefreshWebView();
                    }
                }
            }

            Initialized = true;
        }

    }
}
