using AABC.Mobile.AppServices.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Mobile.Pages.S1
{
	[AddINotifyPropertyChangedInterface]
	public class SettingsPageViewModel
	{
		ISettingsService _settingsService;

		public bool ActiveSessionEnabled { get; private set; }
		public int TimeoutMinutes { get; private set; }

		public SettingsPageViewModel(ISettingsService settingsService)
		{
			_settingsService = settingsService;

			ActiveSessionEnabled = _settingsService.Functionality("ActiveSession.Abandon.Enabled");
			TimeoutMinutes = _settingsService.Setting<int>("ActiveSession.Abandon.TimeoutMinutes");
		}
	}
}
