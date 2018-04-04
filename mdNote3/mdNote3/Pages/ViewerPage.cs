using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using mdOrganizer.Services;
using mdOrganizer.UI;

namespace mdOrganizer.Pages
{
    public class AutoCreateInfo
    {
        public string Caption;
        public string Content;
    }

    public class ViewerPage : ContentPage
    {
        private static ViewerPage _viewer = null;
        public static ViewerPage Viewer
        {
            get
            {
                if (_viewer == null) _viewer = new ViewerPage();
                return _viewer;
            }
        }

        public static AutoCreateInfo AutoCreateInfo = null;

        public ViewerPage()
        {
            initToolbar();

            searchBar = new SearchBar()
            {
                Placeholder = Strings.Loc.ViewerPage_SearchBar,
                BackgroundColor = UI.Styles.EntryBackground,
                SearchCommand = new Command(
                (o) =>
                {
                    RefreshWebView();
                })
            };

            webView = new AdvWebView();
            webView.Navigating += WebView_Navigating;
            webView.Source = "about:blank";
            htmlSource = new HtmlWebViewSource();
            this.Padding = 0;

            this.Focused += ViewerPage_Focused;

            Grid grid = new Grid()
            {
                RowDefinitions = {
                    new RowDefinition() {
                        Height = GridLength.Auto
                    },
                    new RowDefinition(){
                        Height = GridLength.Star
                    }
                },
                Children = {
                    new StackLayout()
                    {
                        Padding = new Thickness(10),
                        BackgroundColor = (Color)App.Current.Resources["Primary"],
                        Children = {
                            searchBar
                        }
                    }
                    , webView}
            };

            Grid.SetRow(searchBar, 0);
            Grid.SetRow(webView, 1);

            Content = grid;
        }

        private void AutoCreate()
        {
            if (AutoCreateInfo != null) 
                Add(AutoCreateInfo.Caption, AutoCreateInfo.Content);
        }

        #region UI Controls
        private SearchBar searchBar;
        private AdvWebView webView;
        private HtmlWebViewSource htmlSource;
        private Func<string, Task<string>> _evaluateJavascript;
        public Func<string, Task<string>> EvaluateJavascript
        {
            get { return _evaluateJavascript; }
            set { _evaluateJavascript = value; }
        }

        private void WebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            if (e.Url.Equals("about:blank")) return;
            e.Cancel = true;
            if (e.Url.StartsWith(htmlSource.BaseUrl))
            {
                string command = e.Url.Substring(htmlSource.BaseUrl.Length);
                if (command.StartsWith("parent_"))
                {
                    NoteNavigator.GoBack(int.Parse(command.Substring(7)));
                }
                if (command.StartsWith("goto_"))
                {
                    if (command.StartsWith("goto_parent"))
                        NoteNavigator.GoBack();
                    else
                        NoteNavigator.CurrentPath.Add(int.Parse(command.Substring(5)));
                }
                if (command.StartsWith("edit_"))
                {
                    if (command.StartsWith("edit_current"))
                        Edit(NoteNavigator.CurrentNote);
                    else
                        Edit(NoteNavigator.CurrentNote.Childs[int.Parse(command.Substring(5))]);
                    return;
                }
                if (command.StartsWith("delete_"))
                {
                    if (command.StartsWith("delete_current"))
                        Delete(NoteNavigator.CurrentNote);
                    else
                        Delete(NoteNavigator.CurrentNote.Childs[int.Parse(command.Substring(7))]);
                    return;
                }
                if (command.StartsWith("share_"))
                {
                    if (command.StartsWith("share_current"))
                        Share(NoteNavigator.CurrentNote);
                    else
                        Share(NoteNavigator.CurrentNote.Childs[int.Parse(command.Substring(6))]);
                    return;
                }
                if (command.StartsWith("add"))
                {
                    Add();
                }
                if (command.StartsWith("back"))
                    NoteNavigator.GoBack();
                RefreshWebView();
            }
            else
                Device.OpenUri(new Uri(e.Url));
        }
        private void ViewerPage_Focused(object sender, FocusEventArgs e)
        {
            webView.Focus();
        }

        private void initToolbar()
        {
            if (!Device.RuntimePlatform.Equals(Device.Android))
            {
                ToolbarItem addBtn = new ToolbarItem()
                {
                    Icon = "plus_white.png",
                    Text = Strings.Loc.ViewerPage_Add,
                    Command = new Command(new Action(Add)),
                    Order = ToolbarItemOrder.Primary
                };
                ToolbarItems.Add(addBtn);
            }

            ToolbarItem listBtn = new ToolbarItem();
            listBtn.Icon = Settings.PreviewMode ? "list_white.png" : "preview_white.png";
            listBtn.Text = Strings.Loc.ViewerPage_ViewMode;
            listBtn.Command = new Command(() =>
            {
                Settings.PreviewMode = !Settings.PreviewMode;
                listBtn.Icon = Settings.PreviewMode ? "list_white.png" : "preview_white.png";
                RefreshWebView();
            });
            listBtn.Order = ToolbarItemOrder.Primary;
            ToolbarItems.Add(listBtn);

            ToolbarItem refreshBtn = new ToolbarItem()
            {
                Icon = "refresh_white.png",
                Text = Strings.Loc.ViewerPage_Refresh,
                Command = new Command(async (o) =>
                {
                    await DeviceServices.ReadDocumentAsync();
                }),
                Order = ToolbarItemOrder.Secondary
            };
            ToolbarItems.Add(refreshBtn);

            ToolbarItem zoominBtn = new ToolbarItem()
            {
                Icon = "zoomin_white.png",
                Text = Strings.Loc.ViewerPage_ZoomIn,
                Command = new Command((o) =>
                {
                    Settings.BaseFontSize++;
                    RefreshWebView();
                }),
                Order = ToolbarItemOrder.Secondary
            };
            ToolbarItems.Add(zoominBtn);

            ToolbarItem zoomoutBtn = new ToolbarItem()
            {

                Icon = "zoomout_white.png",
                Text = Strings.Loc.ViewerPage_ZoomOut,
                Command = new Command((o) =>
                {
                    Settings.BaseFontSize--;
                    RefreshWebView();
                }),
                Order = ToolbarItemOrder.Secondary
            };
            ToolbarItems.Add(zoomoutBtn);
        }
        #endregion

        public void RefreshWebView()
        {
            if (NoteNavigator.Document.Root == null)
            {
                webView.Source = "about:blank";
                return;
            }

            NoteNavigator.Document.FindString(searchBar.Text);
            string htmlForm = NoteNavigator.NoteHtmlForm;
            if ((!String.IsNullOrEmpty(htmlForm)) && (!String.IsNullOrEmpty(searchBar.Text)))
                htmlForm = htmlForm.Replace(searchBar.Text, "<span class='marked'>" + searchBar.Text + "</span>");

            htmlSource.BaseUrl = DeviceServices.BaseUrl;
            htmlSource.Html = htmlForm;
            Title = NoteNavigator.FileName;

            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                webView.Source = null;
                webView.Focus();
                webView.Source = htmlSource;
                if (!Settings.ManualCategory)
                    AutoCreate();
                else
                {
                    if (AutoCreateInfo != null)
                        Title = Strings.Loc.ViewerPage_ChooseCategoryTitle;
                }
            });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            RefreshWebView();
        }

        protected override bool OnBackButtonPressed()
        {
            if (NoteNavigator.CanGoBack)
            {
                NoteNavigator.GoBack();
                RefreshWebView();
                return true;
            }
            else
                return base.OnBackButtonPressed();
        }

        public void Edit(Markdown.Item item)
        {
            EditorPage.Editor.EditNote(item, RefreshWebView);
            MainPage.Navigator.PushAsync(EditorPage.Editor, true);
        }

        public async void Delete(Markdown.Item item)
        {
            if (item.Parent == null)
            {
                if (!await DisplayAlert(Strings.Loc.ViewerPage_ClearTitle, Strings.Loc.ViewerPage_ClearConfirm, Strings.Loc.ViewerPage_Yes, Strings.Loc.ViewerPage_No)) return;
                NoteNavigator.Document.Root.Childs.Clear();
                await DeviceServices.SaveDocumentAsync();
            }
            else
            {
                if (!await DisplayAlert(Strings.Loc.ViewerPage_DeleteTitle, String.Format(Strings.Loc.ViewerPage_DeleteConfirm, item.Title), Strings.Loc.ViewerPage_Yes, Strings.Loc.ViewerPage_No)) return;
                Markdown.Item newCurrent = NoteNavigator.CurrentNote;
                if (NoteNavigator.CurrentNote == item)
                    newCurrent = item.Parent;
                NoteNavigator.Document.RemoveContent(item);
                NoteNavigator.BuildPathToNote(newCurrent);
                await DeviceServices.SaveDocumentAsync();
            }
            RefreshWebView();
        }

        public void Share(Markdown.Item item)
        {
            Plugin.Share.CrossShare.Current.Share(new Plugin.Share.Abstractions.ShareMessage
            {
                Title = item.Parent != null ? item.Title : NoteNavigator.FileName,
                Text = item.HtmlForm
            });
        }

        public void Add()
        {
            //TODO Убрать магические константы
            if (AutoCreateInfo != null)
                AutoCreate();
            else
            {
                EditorPage.Editor.AddNote(NoteNavigator.CurrentNote, RefreshWebView);
                MainPage.Navigator.PushAsync(EditorPage.Editor, true);
            }
        }

        private void AutoClose()
        {
            DeviceServices.ExitAsync();
        }

        public void Add(string defaultCaption, string defaultContent)
        {
            AutoCreateInfo = null;
            EditorPage.Editor.AddNote(Settings.ManualCategory ? NoteNavigator.CurrentNote : null, AutoClose, defaultCaption, defaultContent, Strings.Loc.UnsortedCategory);
            MainPage.Navigator.PushAsync(EditorPage.Editor, true);
        }

        public void EditNote()
        {
            Edit(NoteNavigator.CurrentNote);
        }
    }
}