using AABC.Mobile.AppServices.Extensions;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.Entities;
using AABC.Mobile.Pages.BaseViewModels;
using AABC.Mobile.Pages.S2_1_3;
using AABC.Mobile.SharedEntities.Entities;
using Plugin.Connectivity.Abstractions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AABC.Mobile.Pages.S2_1
{
	[AddINotifyPropertyChangedInterface]
	public class SelectedCasePageViewModel : SelectedCaseViewModel, INavigationAware
	{
		readonly INavigationService _navigationService;
		readonly IApplicationState _applicationState;
		readonly IDatabaseService _databaseService;
		readonly IDataUpdateService _dataUpdateService;
		readonly ISessionUpdateService _sessionUpdateService;
		readonly IConnectivity _connectivity;

		public bool ActiveSession { get; set; }

		public bool ActiveSessionDisabled { get; set; }
		public string ActiveSessionDisabledText { get; set; }

		public bool ManualTimeDisabled { get; set; }
		public string ManualTimeDisabledText { get; set; }

		public ICommand HomeSessionClicked { get; set; }
		public ICommand ManualTimeClicked { get; set; }
		public ICommand ViewDetailsClicked { get; set; }

		public SelectedCasePageViewModel(INavigationService navigationService, IApplicationState applicationState, IDatabaseService databaseService, IDataUpdateService dataUpdateService, ISessionUpdateService sessionUpdateService, IConnectivity connectivity) : base(applicationState)
		{
			_navigationService = navigationService;
			_applicationState = applicationState;
			_databaseService = databaseService;
			_dataUpdateService = dataUpdateService;
			_sessionUpdateService = sessionUpdateService;
			_connectivity = connectivity;

			HomeSessionClicked = new DelegateCommand(() => OnHomeSessionClicked().IgnoreResult());
			ManualTimeClicked = new DelegateCommand(() => OnManualTimeClicked().IgnoreResult());
			ViewDetailsClicked = new DelegateCommand(() => OnViewDetailsClicked().IgnoreResult());
		}

		async Task OnHomeSessionClicked()
		{
			if (ActiveSession)
			{
				// we have an active session so show the details
				await _navigationService.NavigateAsync("ActiveSessionPage");
			}
			else
			{
				// Show the sessions that we can start
				await _navigationService.NavigateAsync("SessionStartPage");
			}
		}

		async Task OnManualTimeClicked()
		{
			var manualEntryController = new ManualEntryWizardController(SelectedCase, _navigationService, _dataUpdateService, _databaseService, _sessionUpdateService, _applicationState);

			await manualEntryController.NextPage(-1);
		}

		async Task OnViewDetailsClicked()
		{
			await _navigationService.NavigateAsync("CaseDetailsPage");
		}

		public void OnNavigatingTo(NavigationParameters parameters)
		{
			SetButtonStates();
		}

		public void OnNavigatedFrom(NavigationParameters parameters)
		{
		}

		public void OnNavigatedTo(NavigationParameters parameters)
		{
			SetButtonStates();
		}

		private void SetButtonStates()
		{
			bool NoInsurance = (SelectedCase.ActiveInsurance == null);

			if (NoInsurance)
			{
				ManualTimeDisabled = true;
				ManualTimeDisabledText = "This case has no active insurance";

				ActiveSessionDisabled = true;
				ActiveSessionDisabledText = "This case has no active insurance";
			}
			else
			{
				if (!SelectedCase.AllowManualTime)
				{
					// we're not allowed to enter time manually
					ManualTimeDisabled = true;
					ManualTimeDisabledText = "Manual Time is not enabled for this case";
				}
				else if (!_connectivity.IsConnected)
				{
					// this is only available if we're connected
					ManualTimeDisabled = true;
					ManualTimeDisabledText = "Manual Time entry is not available when off-line";
				}
				else
				{
					ManualTimeDisabled = false;
				}

				// have we an active session
				ActiveSession = SessionInProgress != null;

				// have we an active session that isn't this session
				ActiveSessionDisabled = SessionInProgress != null && SelectedCase.ID != SessionInProgress.CaseID;
				if (ActiveSessionDisabled)
				{
					ActiveSessionDisabledText = "Another session is in progress";
				}
			}
		}
	}
}
