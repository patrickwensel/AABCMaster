using AABC.Mobile.AppServices.Entities;
using AABC.Mobile.AppServices.Helpers;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.SharedEntities.Interfaces;
using AABC.Mobile.SharedEntities.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AABC.Mobile.AppServices.Services
{
	public class UpdateService : IUpdateService
	{
		readonly IPropertiesService _propertiesService;
		readonly IVersionService _versionService;
		readonly UrlHelper _urlHelper;
		readonly ISettingsService _settingsService;

		public UpdateService(IPropertiesService propertiesService, IVersionService versionService, ISettingsService settingsService)
		{
			_propertiesService = propertiesService;
			_versionService = versionService;
			_settingsService = settingsService;

			_urlHelper = new UrlHelper(propertiesService, "AccountService.");
		}

		public async Task<CurrentVersionResponse> UpdateRequired()
		{
			var timeoutSeconds = _settingsService.Setting<int?>("Communication.TimeoutSeconds");
			var timeout = (timeoutSeconds.HasValue)
									? (TimeSpan?)TimeSpan.FromSeconds(timeoutSeconds.Value)
									: null;

			var communicationHelper = new CommunicationHelper<CurrentVersionResponse, ErrorResponse> { Timeout = timeout };
			
			Dictionary<string, string> queryString = new Dictionary<string, string>();
			queryString["appVersion"] = _versionService.VersionNumber();
			queryString["noCache"] = DateTime.UtcNow.Ticks.ToString();

			var response = await communicationHelper.GetRequest(_urlHelper.BuildUrl("api/Cases/CurrentVersion", queryString));

			return response;
		}

		public Task RedirectToUpdate()
		{
#warning TODO: Add package information here

			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
					Device.OpenUri(new Uri("itms://itunes.apple.com"));
					break;

				case Device.Android:
					Device.OpenUri(new Uri("https://play.google.com/store/apps/details?id=[ID_HERE]"));
					break;
			}

			return Task.FromResult(false);
		}
	}
}
