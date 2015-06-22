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
	[Activity (Label = "龙江云农业", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		WebView WebView;
		public IValueCallback mUploadMessage;
		private static int FILECHOOSER_RESULTCODE = 1;

		public override bool OnKeyDown (Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back && WebView.CanGoBack ()) {
				WebView.GoBack ();// 返回前一个页面
				return true;
			} else
				return base.OnKeyDown (keyCode, e);
		}

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			WebView = FindViewById<WebView> (Resource.Id.webView);
			var wvc = new CCWebViewClient (this);
			var chrome = new CCWebChromeClient ((uploadMsg, acceptType, capture) => {
				mUploadMessage = uploadMsg;
				var i = new Intent (Intent.ActionGetContent);
				i.AddCategory (Intent.CategoryOpenable);
				i.SetType ("image/*");
				StartActivityForResult (Intent.CreateChooser (i, "选择图片"), FILECHOOSER_RESULTCODE);	
			});
			WebView.Settings.JavaScriptEnabled = true;
			WebView.Settings.CacheMode = CacheModes.Normal;
			WebView.Settings.AllowContentAccess = true;
			WebView.Settings.AllowFileAccess = true;
			WebView.Settings.AllowUniversalAccessFromFileURLs = true;
			WebView.Settings.AllowFileAccessFromFileURLs = true;
			WebView.Settings.JavaScriptCanOpenWindowsAutomatically = true;
			WebView.Settings.LoadWithOverviewMode = true;
			WebView.SetWebViewClient (wvc);
			WebView.SetWebChromeClient (chrome);
			WebView.LoadUrl ("http://221.208.208.32:7532/Mobile");
		}

		protected override void OnActivityResult (int requestCode, Result resultCode, Intent intent)
		{
			if (requestCode == FILECHOOSER_RESULTCODE) {
				if (null == mUploadMessage)
					return;
				Java.Lang.Object result = intent == null || resultCode != Result.Ok
					? null
					: intent.Data;
				mUploadMessage.OnReceiveValue (result);
				mUploadMessage = null;
			}
		}
	}
}


