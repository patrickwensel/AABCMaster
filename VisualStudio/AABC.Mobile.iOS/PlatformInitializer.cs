using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.Interfaces;
using AABC.Mobile.iOS.Services;
using Foundation;
using Prism;
using Prism.Ioc;
using UIKit;

namespace AABC.Mobile.iOS
{
	class PlatformInitializer : IPlatformInitializer
	{
		public void RegisterTypes(IContainerRegistry containerRegistry)
		{
			containerRegistry.RegisterSingleton<IVersionService, VersionService>();
			containerRegistry.RegisterSingleton<INotificationService, NotificationService>();
		}
	}
}