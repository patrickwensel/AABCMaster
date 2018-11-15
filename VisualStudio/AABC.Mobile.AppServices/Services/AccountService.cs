using AABC.Mobile.AppServices.Entities;
using AABC.Mobile.AppServices.Events;
using AABC.Mobile.AppServices.Exceptions;
using AABC.Mobile.AppServices.Helpers;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.SharedEntities.Interfaces;
using Newtonsoft.Json;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Services
{
	/// <summary>
	/// Class AccountService.
	/// </summary>
	/// <seealso cref="AABC.Mobile.AppServices.Interfaces.IAccountService" />
	public class AccountService : IAccountService
	{

		readonly IEventAggregator _eventAggregator;
		readonly IApplicationState _applicationState;
		readonly UrlHelper _urlHelper;
		readonly IPropertiesService _propertiesService;
		readonly ISettingsService _settingsService;

		string _userName;
		string _password;

		AuthenticationHeaderValue _cachedAuthenticationHeader;
		DateTime _tokenExpiryDateUtc;

		public AccountService(IEventAggregator eventAggregator, IApplicationState applicationState, IPropertiesService propertiesService, ISettingsService settingsService)
		{
			_eventAggregator = eventAggregator;
			_applicationState = applicationState;
			_propertiesService = propertiesService;
			_settingsService = settingsService;

			_urlHelper = new UrlHelper(propertiesService, "AccountService.");
		}

		public async Task Login(string userName, string password)
		{
			SetCachedCredentials(userName, password);

			var accessToken = await GetAuthenticationHeader();

			_applicationState.CurrentUser = new User { Username = userName };

			// the user has changed
			_eventAggregator.GetEvent<UserChangedEvent>().Publish(_applicationState.CurrentUser);
		}

		public void SetCachedCredentials(string userName, string password)
		{
			if (_userName != userName || _password != password)
			{
				ClearCachedDetails();

				// remember the new details
				_userName = userName;
				_password = password;
			}
		}


		public void Logout()
		{
			ClearCachedDetails();
		}


		/// <summary>
		/// Gets the access token.
		/// </summary>
		/// <returns>Task&lt;System.String&gt;.</returns>
		/// <exception cref="CommunicationException"></exception>
		public async Task<AuthenticationHeaderValue> GetAuthenticationHeader()
		{
			if (_userName == null)
			{
				// no point in looking if the username hasn't been set up
				return null;
			}

			if (_cachedAuthenticationHeader != null && DateTime.UtcNow < _tokenExpiryDateUtc)
			{
				// used the cached token;
				return _cachedAuthenticationHeader;
			}
			else
			{
				Dictionary<string, string> postValues = new Dictionary<string, string>();

				postValues["grant_type"] = "password";
				postValues["username"] = _userName;
				postValues["password"] = _password;

				var timeoutSeconds = _settingsService.Setting<int?>("Communication.TimeoutSeconds");
				var timeout = (timeoutSeconds.HasValue)
										? (TimeSpan?)TimeSpan.FromSeconds(timeoutSeconds.Value)
										: null;

				var communicationHelper = new CommunicationHelper<LoginResponse, LoginErrorResponse> { Timeout = timeout };

				var loginResponse = await communicationHelper.PostRequest(_urlHelper.BuildUrl("oauth2/token"), postValues);

				if (loginResponse == null)
				{
					throw new CommunicationException(communicationHelper.ErrorResponse?.ToString());
				}
				else
				{
					_cachedAuthenticationHeader = new AuthenticationHeaderValue("Bearer", loginResponse.AccessToken);

					// remember the expiry date, but give ourselves a bit of leeway with the time
					_tokenExpiryDateUtc = DateTime.UtcNow + TimeSpan.FromSeconds(loginResponse.ExpiresInSeconds.Value) - TimeSpan.FromMinutes(2);

					return _cachedAuthenticationHeader;
				}
			}
		}

		/// <summary>
		/// Clears the cached details.
		/// </summary>
		void ClearCachedDetails()
		{
			// clear the cached login details 
			_applicationState.CurrentUser = null;

			_password = null;
			_cachedAuthenticationHeader = null;
			_tokenExpiryDateUtc = DateTime.MinValue;

			if (_userName != null)
			{
				_userName = null;

				// inform everyone that the user has changed
				_eventAggregator.GetEvent<UserChangedEvent>().Publish(null);
			}
		}

	}
}
