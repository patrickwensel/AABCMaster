using AABC.Mobile.AppServices.Extensions;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.Entities;
using AABC.Mobile.Pages.BaseViewModels;
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

namespace AABC.Mobile.Pages.S2_1_1
{
	[AddINotifyPropertyChangedInterface]
	public class ActiveSessionPageViewModel: SelectedCaseViewModel, INavigationAware
	{
		readonly INavigationService _navigationService;
		readonly IDataUpdateService _dataUpdateService;
		readonly IDatabaseService _databaseService;
		readonly ISessionUpdateService _sessionUpdateService;
		readonly IApplicationState _applicationState;

		public ICommand AddNotesClicked { get; set; }

		public ICommand CompleteSessionClicked { get; set; }

		public TimeSpan Duration { get; set; } = TimeSpan.FromMinutes(25);

		bool _timerRunning;

		public ActiveSessionPageViewModel(INavigationService navigationService, IDataUpdateService dataUpdateService, IDatabaseService databaseService, ISessionUpdateService sessionUpdateService, IApplicationState applicationState) : base(applicationState)
		{
			_navigationService = navigationService;
			_dataUpdateService = dataUpdateService;
			_databaseService = databaseService;
			_sessionUpdateService = sessionUpdateService;
			_applicationState = applicationState;

			AddNotesClicked = new DelegateCommand(() => OnAddNotesClicked().IgnoreResult());

			CompleteSessionClicked = new DelegateCommand(() => OnCompleteSessionClicked().IgnoreResult());

			Device.StartTimer(TimeSpan.FromSeconds(5), () => { UpdateDuration(); return _timerRunning; });
		}

		void UpdateDuration()
		{
			if (this.SessionInProgress != null)
			{
				Device.BeginInvokeOnMainThread(() => Duration = DateTime.Now - this.SessionInProgress.StartDateTime);
			}
		}

		async Task OnCompleteSessionClicked()
		{
			var completeSessionWizardController = new CompleteSessionWizardController(SessionInProgress, _navigationService, _dataUpdateService, _databaseService, _sessionUpdateService, _applicationState);

			await completeSessionWizardController.NextPage(-1);
		}

		async Task OnAddNotesClicked()
		{
			var displayQuestionAnswers = new List<DisplayQuestionAnswer>();
			var caseValidationQuestions = await _databaseService.GetCaseValidationQuestions(SessionInProgress.CaseValidationID);
			foreach (var caseValidationQuestion in caseValidationQuestions)
			{
				// get the current answer if we have one
				string answer = await _databaseService.GetCurrentAnswer(caseValidationQuestion) ?? String.Empty;

				displayQuestionAnswers.Add(new DisplayQuestionAnswer { CaseValidationQuestion = caseValidationQuestion, Answer = answer });
			}

			NavigationParameters parameters = new NavigationParameters();
			parameters.Add("DisplayQuestionAnswer", displayQuestionAnswers);
			parameters.Add("AnswerValidationRequired", false);

			await _navigationService.NavigateAsync("NoteEntryPage", parameters);
		}

		public void OnNavigatedFrom(NavigationParameters parameters)
		{
			_timerRunning = false;
		}

		public void OnNavigatedTo(NavigationParameters parameters)
		{
			_timerRunning = true;
			Device.StartTimer(TimeSpan.FromSeconds(5), () => { UpdateDuration(); return _timerRunning; });
		}

		public void OnNavigatingTo(NavigationParameters parameters)
		{
			UpdateDuration();
		}
	}
}
