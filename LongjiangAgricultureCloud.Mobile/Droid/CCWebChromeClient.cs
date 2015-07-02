using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Webkit;

using Android.OS;
namespace LongjiangAgricultureCloud.Mobile.Droid
{
	public class CCWebChromeClient:WebChromeClient
	{
		Action<IValueCallback, Java.Lang.String, Java.Lang.String> callback;

		public CCWebChromeClient (Action<IValueCallback, Java.Lang.String, Java.Lang.String> callback)
		{
			this.callback = callback;
		}

		//For Android 4.1
		[Java.Interop.Export]
		public void openFileChooser (IValueCallback uploadMsg, Java.Lang.String acceptType, Java.Lang.String capture)
		{
			callback (uploadMsg, acceptType, capture);
		}

		[Java.Interop.Export]
		public void onGeolocationPermissionsShowPrompt(String origin, GeolocationPermissions.ICallback callback) {
			callback.Invoke(origin, true, false);
		}
	}
}

