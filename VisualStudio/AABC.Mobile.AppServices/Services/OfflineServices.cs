using AABC.Mobile.AppServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Services
{
	public class OfflineServices : IOfflineServices
	{
		readonly ISecureAppStorage _secureAppStorage;

		public OfflineServices(ISecureAppStorage secureAppStorage)
		{
			_secureAppStorage = secureAppStorage;
		}

		public bool ValidateCredentials(string username, string password)
		{
			// get the current hashed password
			var savedHashedPassword = _secureAppStorage.HashedPassword;

			if (savedHashedPassword == null)
			{
				return false;
			}
			else
			{
				var hashedPassword = GenerateHashedPassword(username, password);
				return (savedHashedPassword == hashedPassword);
			}
		}

		public void SaveCredentials(string username, string password)
		{
			_secureAppStorage.HashedPassword = GenerateHashedPassword(username, password);
		}


		string GenerateHashedPassword(string username, string password)
		{
			// get the current app id (or create one if required)
			string appId = GetOrGenerateAppId();

			// hash the appID, username and password together
			string dataToHash = appId + ":" + username + ":" + password;

			var hashAlgorithm = PCLCrypto.WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(PCLCrypto.HashAlgorithm.Sha256);
			var hashedData = hashAlgorithm.HashData(Encoding.Unicode.GetBytes(dataToHash));
			var hashedPassword = Convert.ToBase64String(hashedData);
			return hashedPassword;
		}

		string GetOrGenerateAppId()
		{
			var appId = _secureAppStorage.AppInstanceId;
			if (appId == null)
			{
				// create random 16 byte number
				byte[] cryptoRandomBuffer = new byte[16];
				PCLCrypto.NetFxCrypto.RandomNumberGenerator.GetBytes(cryptoRandomBuffer);

				// and convert to base 64
				appId = Convert.ToBase64String(cryptoRandomBuffer);
				_secureAppStorage.AppInstanceId = appId;
			}

			return appId;
		}
	}
}
