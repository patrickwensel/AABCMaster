using AABC.Mobile.AppServices.Entities;
using AABC.Mobile.AppServices.Extensions;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.Entities;
using AABC.Mobile.Interfaces;
using AABC.Mobile.Pages.BaseViewModels;
using AABC.Mobile.SharedEntities.Messages;
using Plugin.Connectivity.Abstractions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AABC.Mobile.Pages.C2
{
	[AddINotifyPropertyChangedInterface]
	public class NoteEntryPageViewModel: SelectedCaseViewModel, INavigatingAware
	{
		readonly INavigationService _navigationService;
		readonly IPageDialogService _pageDialogService;
		readonly IDatabaseService _databaseService;
		readonly IConnectivity _connectivity;
		readonly ISessionUpdateService _sessionUpdateService;
		readonly ISettingsService _settingsService;

		public List<DisplayQuestionAnswer> DisplayQuestionAnswers { get; set; }

		private bool _answerValidationRequired;

		public ICommand DoneCommand { get; set; }

		public ICommand CancelCommand { get; set; }

		// The data is automatically saved if this page is closed (via a call on the Page OnDisappearing() handler)
		// setting this flag stops this 
		public bool DontSaveOnClose { get; set; }

		IWizardController _wizardController;
		int _pageNumber;

		public NoteEntryPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IDatabaseService databaseService, IApplicationState applicationState, IConnectivity connectivity, ISessionUpdateService sessionUpdateService, ISettingsService settingsService) : base(applicationState)
		{
			_navigationService = navigationService;
			_pageDialogService = pageDialogService;
			_databaseService = databaseService;
			_connectivity = connectivity;
			_sessionUpdateService = sessionUpdateService;
			_settingsService = settingsService;

			DoneCommand = new DelegateCommand(() => OnDoneCommand().IgnoreResult());
			CancelCommand = new DelegateCommand(() => OnCancelCommand().IgnoreResult());
		}

		async Task OnDoneCommand()
		{
			var valid = await ValidateAnswers();
			if (!valid)
			{
				// TODO: report errors
			}
			else
			{ 
				if (_wizardController != null)
				{
					await _wizardController.NextPage(_pageNumber);
				}
				else
				{ 
					// save
					await SaveData();

					DontSaveOnClose = true;

					await _navigationService.GoBackAsync();
				}
			}
		}

		async Task OnCancelCommand()
		{
			if (await _pageDialogService.DisplayAlertAsync("Cancel", "Are you sure you want to cancel without saving?", "Yes", "No"))
			{
				DontSaveOnClose = true;

				if (_wizardController != null)
				{
					await _wizardController.Cancel();
				}
				else
				{
					await _navigationService.GoBackAsync();
				}
			}
		}

		public async Task SaveData()
		{
			foreach (var displayQuestionAnswer in DisplayQuestionAnswers)
			{
				// save the answer
				await _databaseService.SaveAnswer(displayQuestionAnswer.CaseValidationQuestion, displayQuestionAnswer.Answer);
			}
		}

		Task<bool> ValidateAnswers()
		{
			bool valid = true;

			if (_answerValidationRequired)
			{
				var answerValidationRegex = _settingsService.Setting<string>("NoteEntry.AnswerValidationRegex");

				if (!String.IsNullOrEmpty(answerValidationRegex))
				{
					var answerValidationMessage = _settingsService.Setting<string>("NoteEntry.AnswerValidationMessage") ?? "Please enter a valid answer";

					// validate the answer against the answer regex.
					foreach (var displayQuestionAnswer in DisplayQuestionAnswers)
					{
						if (!System.Text.RegularExpressions.Regex.IsMatch(displayQuestionAnswer.Answer, answerValidationRegex))
						{
							displayQuestionAnswer.DisplayError = true;
							displayQuestionAnswer.ErrorMessage = answerValidationMessage;
							valid = false;
						}
						else
						{
							displayQuestionAnswer.DisplayError = false;
						}
					}
				}
			}

			return Task.FromResult(valid);
		}

		Task<bool> ValidateAnswers(List<DisplayQuestionAnswer> displayQuestionAnswers)
		{
			// TODO: add call for validating the answer
			return Task.FromResult(true);
		}

		public void OnNavigatingTo(NavigationParameters parameters)
		{
			_wizardController = parameters.GetValue<IWizardController>("IWizardController");
			_pageNumber = parameters.GetValue<int>("PageNumber");
			DisplayQuestionAnswers = parameters.GetValue<List<DisplayQuestionAnswer>>("DisplayQuestionAnswer");
			_answerValidationRequired = parameters.GetValue<bool>("AnswerValidationRequired");
		}
	}
}
