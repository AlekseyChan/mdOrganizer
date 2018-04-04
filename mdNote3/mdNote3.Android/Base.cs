using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;

[assembly: Dependency(typeof(mdOrganizer.Droid.Base))]
namespace mdOrganizer.Droid
{
    public class Base : mdOrganizer.Services.IBase
    {
        public string GetBaseResource()
        {
            return "mdOrganizer.Droid";
        }

        public string GetBaseUrl()
        {
            return "file:///android_asset/";
        }
    }
}