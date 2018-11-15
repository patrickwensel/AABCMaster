using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using UIKit;

namespace AABC.Mobile.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			AppCenter.Start("b2f55bd2-d3c2-478c-9e67-909a9946f6f0", typeof(Analytics), typeof(Crashes));

			global::Xamarin.Forms.Forms.Init();
			LoadApplication(new App(new PlatformInitializer()));

			var selectedColor = UIColor.FromRGB(0x40, 0x85, 0xc0);

			// Color of the selected tab icon:
			UITabBar.Appearance.SelectedImageTintColor = selectedColor;

			// Color of the tabbar background:
			UITabBar.Appearance.BarTintColor = UIColor.FromRGB(0xf9, 0xf9, 0xf9);


			UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.BlackOpaque;

			// Color of the selected tab text color:
			UITabBarItem.Appearance.SetTitleTextAttributes(
				new UITextAttributes()
				{
					TextColor = selectedColor
				},
				UIControlState.Selected);

			// Color of the unselected tab icon & text:
			UITabBarItem.Appearance.SetTitleTextAttributes(
				new UITextAttributes()
				{
					TextColor = UIColor.FromRGB(0x8e, 0x8e, 0x93)
				},
				UIControlState.Normal);

			return base.FinishedLaunching(app, options);
		}
	}
}
