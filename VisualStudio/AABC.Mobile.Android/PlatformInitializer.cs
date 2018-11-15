using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.Droid.Services;
using AABC.Mobile.Interfaces;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Prism;
using Prism.Ioc;

namespace AABC.Mobile.Droid
{
	class PlatformInitializer : IPlatformInitializer
	{
		Context _context;

		public PlatformInitializer(Context context)
		{
			_context = context;
		}

		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			containerRegistry.RegisterInstance<IVersionService>(new VersionService(_context));
			containerRegistry.RegisterInstance<INotificationService>(new NotificationService(_context));

		}
	}
}