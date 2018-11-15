using AABC.Mobile.AppServices.Interfaces;
using Plugin.SecureStorage.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Services
{
	public class SecureAppStorage : ISecureAppStorage
	{
		readonly ISecureStorage _secureStorage;

		public SecureAppStorage(ISecureStorage secureStorage)
		{
			_secureStorage = secureStorage;
		}

		public string AppInstanceId
		{
			get
			{
				return _secureStorage.GetValue("AppId", null);
			}

			set
			{
				_secureStorage.SetValue("AppId", value);
			}
		}


		public string Username
		{
			get
			{
				return _secureStorage.GetValue("Username", null);
			}

			set
			{
				_secureStorage.SetValue("Username", value);
			}
		}


		public string HashedPassword
		{
			get
			{
				return _secureStorage.GetValue("HashedPassword", null);
			}

			set
			{
				_secureStorage.SetValue("HashedPassword", value);
			}
		}

	}
}
