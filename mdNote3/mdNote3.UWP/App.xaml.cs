using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace mdOrganizer.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private Windows.ApplicationModel.DataTransfer.ShareTarget.ShareOperation shareOpertion = null;
        private static bool Initialized = false;
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        private void InitXamarin(IActivatedEventArgs e)
        {
            if (Initialized) return;
            List<Assembly> assembliesToInclude = new System.Collections.Generic.List<System.Reflection.Assembly>();
            assembliesToInclude.Add(typeof(Xamarin.Forms.WebView).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(mdOrganizer.UI.AdvWebView).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(mdOrganizer.UWP.Base).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(mdOrganizer.UWP.AdvWebViewRender).GetTypeInfo().Assembly);
            assembliesToInclude.Add(typeof(Xamarin.Forms.DependencyService).GetTypeInfo().Assembly);

            Xamarin.Forms.Forms.Init(e, assembliesToInclude);

            Xamarin.Forms.DependencyService.Register<mdOrganizer.UWP.Base>();
            Xamarin.Forms.DependencyService.Register<mdOrganizer.UWP.FileSystem>();

            Initialized = true;
        }

        public void UniLaunch(Type type, IActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                InitXamarin(e);

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(type, e);
            }
            // Ensure the current window is active
            Window.Current.Activate();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            UniLaunch(typeof(MainPage), e);
        }

        private async Task FillCreateInfo(Windows.ApplicationModel.DataTransfer.ShareTarget.ShareOperation shareOperation)
        {
            string rawHtml = shareOperation.Data.Contains(StandardDataFormats.Html) ? HtmlFormatHelper.GetStaticFragment((await shareOperation.Data.GetHtmlFormatAsync()).ToString()) : "";
            string rawText = (shareOperation.Data.Contains(StandardDataFormats.Text)) ? (await shareOperation.Data.GetTextAsync()).ToString() : "";
            string rawLink = (shareOperation.Data.Contains(StandardDataFormats.WebLink)) ? (await shareOperation.Data.GetWebLinkAsync()).ToString() : "";

            bool htmlFormed = false;
            string Content = string.Empty;
            string Caption = string.Empty;

            if (!String.IsNullOrEmpty(rawHtml))
            {
                #region parsing html
                System.Text.StringBuilder contentBuilder = new System.Text.StringBuilder();

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(rawHtml);

                var allDivs = doc.DocumentNode.Descendants("div");
                var titleNodes = allDivs.Where(div => div.GetAttributeValue("class", "").Equals("snippet-info-title-div"));
                if (titleNodes.Count() > 0)
                {
                    Caption = titleNodes.First().Descendants("a").First().InnerHtml?.Trim();
                    contentBuilder.AppendLine("[" + Caption + "](" + rawLink + ")");
                }
                var imagesNodes = allDivs.Where(div => div.GetAttributeValue("class", "").Equals("snippet-images-div"));
                if (imagesNodes.Count() > 0)
                {
                    foreach (var imageNode in imagesNodes)
                    {
                        string imgUrl = imageNode.GetAttributeValue("src", "")?.Trim();
                        if (!String.IsNullOrEmpty(imgUrl))
                            contentBuilder.AppendLine("![](" + imgUrl + ")");
                    }
                }
                var descriptionNodes = allDivs.Where(div => div.GetAttributeValue("class", "").Equals("snippet-info-description-div"));
                if (descriptionNodes.Count() > 0)
                    contentBuilder.AppendLine(descriptionNodes.First().InnerHtml?.Trim());
                if (contentBuilder.Length > 0)
                {
                    htmlFormed = true;
                    Content = contentBuilder.ToString();
                }
                #endregion
            }

            if (!htmlFormed)
            {
                Content = rawText;
                if (!String.IsNullOrEmpty(rawLink))
                    Content = "[" + (String.IsNullOrEmpty(Content) ? "link" : Content) + "](" + rawLink + ")";
            }

            mdOrganizer.Pages.ViewerPage.AutoCreateInfo = new Pages.AutoCreateInfo()
            {
                Caption = Caption,
                Content = Content
            };
        }

        protected override async void OnShareTargetActivated(ShareTargetActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.ShareTarget)
            {
                args.ShareOperation.ReportStarted();
                await FillCreateInfo(args.ShareOperation);
                if (!Initialized)
                {
                    InitXamarin(args);
                    var rootFrame = new Frame();
                    rootFrame.Navigate(typeof(MainPage));
                    Window.Current.Content = rootFrame;
                    Window.Current.Activate();
                }
                else
                {
                    Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                    {
                        await mdOrganizer.Services.DeviceServices.ReadDocumentAsync();
                        if (mdOrganizer.Services.Settings.ManualCategory)
                        {
                            await mdOrganizer.Pages.MainPage.Navigator.PopToRootAsync();
                            mdOrganizer.Pages.ViewerPage.Viewer.RefreshWebView();
                        }
                    });
                    args.ShareOperation.ReportCompleted();
                }
            }

            /*                if (!Initialized)
                            {
                                UniLaunch(typeof(MainPage), args);
                            }
                            else
                            {
                                Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
                                  {
                                      await mdOrganizer.Services.DeviceServices.ReadDocumentAsync();
                                      if (mdOrganizer.Services.Settings.ManualCategory)
                                      {
                                          await mdOrganizer.Pages.MainPage.Navigator.PopToRootAsync();
                                          mdOrganizer.Pages.ViewerPage.Viewer.RefreshWebView();
                                      }
                                  });
                            }(
                            await FillCreateInfo(args.ShareOperation);
                            args.ShareOperation.ReportDataRetrieved() ;
                            mdOrganizer.Pages.MainPage.Navigator.Focus();
                            args.ShareOperation.ReportCompleted();*/
        }

        
        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }


        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
