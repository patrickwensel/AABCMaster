using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.Interfaces;
using Foundation;
using UIKit;

namespace AABC.Mobile.iOS.Services
{
	class VersionService : IVersionService
	{
		public string VersionNumber()
		{
			return NSBundle.MainBundle.InfoDictionary[new NSString("CFBundleShortVersionString")].ToString();
		}

		public string SystemVersion()
		{
			return UIDevice.CurrentDevice.SystemVersion;
		}

		public string SystemModel()
		{
			return UIDevice.CurrentDevice.Model;
		}
	}
}