using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Naxam.Controls.Platform.Droid;

namespace AABC.Mobile.Droid
{
	[Activity(Label = "AABC.Mobile", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/MainTheme", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		App _app;

		protected override void OnCreate(Bundle bundle)
		{
			AppCenter.Start("cd0caf09-075b-43e6-9798-7d0000d6fda3", typeof(Analytics), typeof(Crashes));

			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			InitialiseBottomTabbedRenderer();

			base.OnCreate(bundle);

			global::Xamarin.Forms.Forms.Init(this, bundle);

			_app = new App(new PlatformInitializer(ApplicationContext));

			LoadApplication(_app);
		}

		void InitialiseBottomTabbedRenderer()
		{
			BottomTabbedRenderer.FontSize = 15;
			BottomTabbedRenderer.IconSize = 30;
			BottomTabbedRenderer.ItemSpacing = 8;
			BottomTabbedRenderer.ItemPadding = new Xamarin.Forms.Thickness(8);
			BottomTabbedRenderer.BottomBarHeight = 70;
		}

		public override void OnBackPressed()
		{
			if (_app.IsBackAllowed())
			{
				base.OnBackPressed();
			}
		}
	}
}

