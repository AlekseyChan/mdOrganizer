using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;
using mdOrganizer.UI;

[assembly: ExportRenderer(typeof(AdvWebView), typeof(mdOrganizer.UWP.AdvWebViewRender))]
namespace mdOrganizer.UWP
{
    public class AdvWebViewRender : WebViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<WebView> e)
        {
            base.OnElementChanged(e);
            var webView = e.NewElement as AdvWebView;
            if (webView != null)
                webView.EvaluateJavascript = async (js) =>
                {
                    string res = await Control.InvokeScriptAsync("eval", new[] { js });
                    return res;
                };
        }
    }
}
