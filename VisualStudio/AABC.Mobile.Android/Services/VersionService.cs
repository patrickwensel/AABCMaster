using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AABC.Mobile.AppServices.Interfaces;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace AABC.Mobile.Droid.Services
{
	class VersionService : IVersionService
	{
		Context _context;

		public VersionService(Context context)
		{
			_context = context;
		}

		public string VersionNumber()
		{
			var packageInfo = _context.PackageManager.GetPackageInfo(_context.PackageName, 0);

			return packageInfo.VersionName;
		}

		public string SystemVersion()
		{
			return Build.VERSION.Release;
		}

		public string SystemModel()
		{
			return Build.Manufacturer + ' ' + Build.Product;
		}

	}
}