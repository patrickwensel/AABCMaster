using AABC.Mobile.AppServices.Exceptions;
using AABC.Mobile.AppServices.Extensions;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.Interfaces;
using Plugin.Messaging;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace AABC.Mobile.Pages.S3
{
	[AddINotifyPropertyChangedInterface]
	public class AboutPageViewModel : INavigatingAware
	{
		IVersionService _versionService;

		ISettingsService _settingsService;

		IDataUpdateService _dataUpdateService;

		IEmailTask _emailTask;

		public ICommand CheckForUpdate { get; set; }

		public ICommand SendEmail { get; set; }

		public ICommand OpenUserGuide { get; set; }

		public string VersionNumber { get; private set; }

		public string ServerVersion { get; private set; }

		public bool Busy { get; private set; }

		public bool CanSendEmail { get; set; }

		public string UserGuideUrl { get; set; } = "http://downloads.appliedabc.com/apa_user_guide_latest.pdf";

		public AboutPageViewModel(IVersionService versionService, ISettingsService settingsService, IDataUpdateService dataUpdateService, IEmailTask emailTask)
		{
			_versionService = versionService;
			_settingsService = settingsService;
			_dataUpdateService = dataUpdateService;
			_emailTask = emailTask;

			GetLatestInformation();

			CheckForUpdate = new DelegateCommand(() => OnCheckForUpdate().IgnoreResult());
			SendEmail = new DelegateCommand(() => OnSendEmail());
			OpenUserGuide = new DelegateCommand(() => OnOpenUserGuide());

			CanSendEmail = _emailTask.CanSendEmail;
		}

		void OnOpenUserGuide()
		{
			Device.OpenUri(new Uri(UserGuideUrl));
		}

		async Task OnCheckForUpdate()
		{
			try
			{
				Busy = true;

				// check for an update
				var initialData = await _dataUpdateService.GetInitialData();
				await _settingsService.UpdateSettings(initialData.Settings);

				GetLatestInformation();

			}
			catch 
			{
				// ignore this exception
			}
			finally
			{
				Busy = false;
			}
		}

		void OnSendEmail()
		{
			if (_emailTask.CanSendEmail)
			{
				_emailTask.SendEmail("info@appliedabc.com", "Applied Behavioral Health Counseling Support");
			}
		}

		public void OnNavigatingTo(NavigationParameters parameters)
		{
			GetLatestInformation();
		}

		void GetLatestInformation()
		{
			VersionNumber = _versionService.VersionNumber();
			ServerVersion = _settingsService.Setting<string>("ServerVersion");
		}

	}
}
