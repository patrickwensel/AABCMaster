using AABC.Mobile.AppServices.Entities;
using AABC.Mobile.AppServices.Exceptions;
using AABC.Mobile.AppServices.Helpers;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.SharedEntities.Entities;
using AABC.Mobile.SharedEntities.Interfaces;
using AABC.Mobile.SharedEntities.Messages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Services
{
	public class DataUpdateService : IDataUpdateService
	{
		readonly IPropertiesService _propertiesService;
		readonly IAccountService _accountService;
		readonly UrlHelper _urlHelper;
		readonly ISettingsService _settingsService;

		public DataUpdateService(IPropertiesService propertiesService, IAccountService accountService, ISettingsService settingsService)
		{
			_propertiesService = propertiesService;
			_accountService = accountService;
			_settingsService = settingsService;

			_urlHelper = new UrlHelper(propertiesService, "AccountService.");
		}

		public async Task<InitialDataResponse> GetInitialData()
		{
			var communicationHelper = new CommunicationHelper<InitialDataResponse, ErrorResponse> { Timeout = GetTimeout() };

			var authenticationHeader = await _accountService.GetAuthenticationHeader();

			var response = await communicationHelper.GetRequest(_urlHelper.BuildUrl("api/Cases"), authenticationHeader);


			if (response == null)
			{
				throw new CommunicationException(communicationHelper.ErrorResponse?.ToStringList());
			}

			return response;
		}

		public async Task<LocationsAndServicesResponse> GetLocationsAndServices(int caseId, DateTime date)
		{
			var communicationHelper = new CommunicationHelper<LocationsAndServicesResponse, ErrorResponse> { Timeout = GetTimeout() };

			var authenticationHeader = await _accountService.GetAuthenticationHeader();

			Dictionary<string, string> queryValues = new Dictionary<string, string>();
			queryValues["caseId"] = caseId.ToString(CultureInfo.InvariantCulture);
			queryValues["date"] = date.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

			var response = await communicationHelper.GetRequest(_urlHelper.BuildUrl("api/Cases/LocationsAndServices", queryValues), authenticationHeader);

			if (response == null)
			{
				throw new CommunicationException(communicationHelper.ErrorResponse?.ToStringList());
			}

			return response;
		}

		public async Task<ValidationResponse> ValidateRequest(ValidatedSession caseAuthorizationRequest, bool save = true)
		{
			try
			{
				var communicationHelper = new CommunicationHelper<ValidationResponse, ErrorResponse> { Timeout = GetTimeout() };

				var authenticationHeader = await _accountService.GetAuthenticationHeader();

				var validationRequest = new ValidationRequest { RequestedValidatedSession = caseAuthorizationRequest };

				var queryStringValues = new Dictionary<string, string>();
				queryStringValues["save"] = save.ToString();

				var response = await communicationHelper.PostRequest(_urlHelper.BuildUrl("api/Cases/Validate", queryStringValues), JsonConvert.SerializeObject(validationRequest), authenticationHeader);

				if (response == null)
				{
					throw new CommunicationException(communicationHelper.ErrorResponse?.ToStringList());
				}

				return response;
			}
			catch (Exception ex)
			{
				var validationResponse = new ValidationResponse() { Errors = new List<string>() };

#if DEBUG
				string errorMessage = "Unable to validate request : " + ex.GetType().Name + " " + ex.Message;
#else
                string errorMessage = "Unable to validate request.";
#endif
				validationResponse.Errors.Add(errorMessage);

				return validationResponse;
			}
		}


		public async Task<SessionUpdateResponse> PostSessionUpdateRequestToServer(SessionUpdateRequest sessionUpdateRequest)
		{
			var communicationHelper = new CommunicationHelper<SessionUpdateResponse, ErrorResponse> { Timeout = GetTimeout() };

			var authenticationHeader = await _accountService.GetAuthenticationHeader();

			var response = await communicationHelper.PostRequest(_urlHelper.BuildUrl("api/Cases/SessionUpdate"), JsonConvert.SerializeObject(sessionUpdateRequest), authenticationHeader);

			if (response == null)
			{
				throw new CommunicationException(communicationHelper.ErrorResponse?.ToStringList());
			}

			return response;
		}

		TimeSpan? GetTimeout()
		{
			// set the timeout
			var timeoutSeconds = _settingsService.Setting<int?>("Communication.TimeoutSeconds");
			return (timeoutSeconds.HasValue)
							? (TimeSpan?)TimeSpan.FromSeconds(timeoutSeconds.Value)
							: null;
		}
	}
}
