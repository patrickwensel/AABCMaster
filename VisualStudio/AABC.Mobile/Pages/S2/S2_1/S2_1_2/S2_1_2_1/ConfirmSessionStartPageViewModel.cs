using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AABC.Mobile.AppServices.Entities;
using AABC.Mobile.AppServices.Extensions;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.SharedEntities.Entities;
using Prism.Commands;
using Prism.Navigation;
using PropertyChanged;

namespace AABC.Mobile.Pages.S2_1_2_1
{
	[AddINotifyPropertyChangedInterface]
	class ConfirmSessionStartPageViewModel : INavigatingAware
	{
		readonly INavigationService _navigationService;
		readonly IApplicationState _applicationState;
		readonly IDatabaseService _databaseService;

		public ICommand ConfirmAndStartCommand { get; set; }
		public ICommand CancelCommand { get; set; }

		public CaseValidation CaseValidation { get; set; }

		public string SessionText { get; set; }

		public bool SessionValid { get; set; }

		const string startSessionText = "I attest that I am currently at the patient’s home and ready to begin service immediately. I understand that by starting this service I will begin tracking my time, and that when the patient’s service is finished, I must complete this session, enter my notes and obtain signatures to stop my timer.";
		const string notTodayText = "This session is not for today, so you cannot start it";

		public ConfirmSessionStartPageViewModel(INavigationService navigationService, IApplicationState applicationState, IDatabaseService databaseService)
		{
			_navigationService = navigationService;
			_applicationState = applicationState;
			_databaseService = databaseService;

			ConfirmAndStartCommand = new DelegateCommand(() => OnConfirmAndStartCommand().IgnoreResult());
			CancelCommand = new DelegateCommand(() => OnCancelCommand().IgnoreResult());
		}

		async Task OnCancelCommand()
		{
			await _navigationService.GoBackAsync();
		}

		async Task OnConfirmAndStartCommand()
		{
			var now = DateTime.Now;
			CaseValidation.DateOfService = now.Date;
			CaseValidation.StartTime = now.TimeOfDay;
			CaseValidation.State = CaseValidationState.Active;

			// update the database with the start time
			await _databaseService.WriteCaseValidation(CaseValidation);

			// set this as the current session
			_applicationState.SetSessionInProgress(CaseValidation);

			// start the session
			await _navigationService.NavigateAsync("../../ActiveSessionPage");
		}

		public void OnNavigatingTo(NavigationParameters navigationParameters)
		{
			// get the session
			CaseValidation = navigationParameters.GetValue<CaseValidation>("CaseValidation");

			if (CaseValidation.DateOfService.Date == DateTime.Now.Date)
			{
				SessionText = startSessionText;
				SessionValid = true;
			}
			else
			{
				SessionText = notTodayText;
				SessionValid = false;
			}
		}
	}
}
