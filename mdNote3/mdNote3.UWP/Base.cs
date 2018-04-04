using Xamarin.Forms;

[assembly: Dependency(typeof(mdOrganizer.UWP.Base))]
namespace mdOrganizer.UWP
{
    class Base:mdOrganizer.Services.IBase
    {
        public string GetBaseUrl()
        {
            return "ms-appx-web:///";
        }
        public string GetBaseResource()
        {
            return "mdOrganizer.UWP";
        }
    }
}
