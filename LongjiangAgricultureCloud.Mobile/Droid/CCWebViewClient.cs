using System;
using System.Threading;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Webkit;
using Android.OS;

namespace LongjiangAgricultureCloud.Mobile.Droid
{
	public class CCWebViewClient : WebViewClient
	{
		MainActivity thisActivity;

		public CCWebViewClient(MainActivity activity)
		{
			thisActivity = activity;
		}

		public override void OnReceivedError (WebView view, ClientError errorCode, string description, string failingUrl)
		{
			if (view.CanGoBack ())
				view.GoBack ();
			else
				view.LoadUrl ("file:///android_asset/nonetwork.html");
		}

		public override bool ShouldOverrideUrlLoading (WebView view, string url)
		{
			view.LoadUrl (url);
			return true;
		}

		public override void OnLoadResource (WebView view, string url)
		{
			if (url.IndexOf ("tel:") >= 0) {
				view.GoBack ();
				Thread.Sleep (500);
				Intent intent = new Intent (Intent.ActionCall, Android.Net.Uri.Parse (url));
				thisActivity.StartActivity (intent);
			} else {
				base.OnLoadResource (view, url);
			}
		}


	}
}