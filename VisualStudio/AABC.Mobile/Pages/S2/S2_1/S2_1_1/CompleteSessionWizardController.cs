using AABC.Mobile.AppServices.Entities;
using AABC.Mobile.AppServices.Extensions;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.Entities;
using AABC.Mobile.Interfaces;
using AABC.Mobile.Pages.C1;
using AABC.Mobile.SharedEntities.Entities;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.Pages.S2_1_1
{
	class CompleteSessionWizardController : IWizardController
	{
		readonly INavigationService _navigationService;

		readonly IDataUpdateService _dataUpdateService;

		readonly IDatabaseService _databaseService;

		readonly ISessionUpdateService _sessionUpdateService;

		readonly CaseValidation _sessionInProgress;

		readonly IApplicationState _applicationState;

		EntryBaseInfoPageModel _entryBaseInfoPageModel = new EntryBaseInfoPageModel();
		List<DisplayQuestionAnswer> _displayQuestionAnswers;

		public CompleteSessionWizardController(CaseValidation sessionInProgress, INavigationService navigationService, IDataUpdateService dataUpdateService, IDatabaseService databaseService, ISessionUpdateService sessionUpdateService, IApplicationState applicationState)
		{
			_navigationService = navigationService;
			_dataUpdateService = dataUpdateService;
			_databaseService = databaseService;
			_sessionUpdateService = sessionUpdateService;
			_applicationState = applicationState;

			_sessionInProgress = sessionInProgress;
		}

		enum Page
		{
			Initial = -1,
			CompleteSessionConfirmationPage,
			NoteEntryPage,
			ProviderSignoffPage,
			ParentSignoffPage,
			Complete
		}


		public async Task Cancel()
		{
			NavigationParameters parameters = new NavigationParameters();
			parameters.Add("SelectedPage", "Cases");
			parameters.Add("SubPage", "SelectedCasePage");
			await _navigationService.NavigateAsync("file:///TabbedNavigationPage", parameters, animated: false);
		}

		public async Task NextPage(int pageNumber)
		{
			Page thisPage = (Page)pageNumber;
			Page nextPage;

			// handle any events and move to the next page
			if (thisPage == Page.Initial)
			{
				// this is the initial page
				nextPage = Page.CompleteSessionConfirmationPage;
			}
			else if (thisPage == Page.CompleteSessionConfirmationPage)
			{
				nextPage = Page.NoteEntryPage;

				_displayQuestionAnswers = new List<DisplayQuestionAnswer>();
				var caseValidationQuestions = await _databaseService.GetCaseValidationQuestions(_sessionInProgress.CaseValidationID);
				foreach (var caseValidationQuestion in caseValidationQuestions)
				{
					// get the current answer if we have one
					string answer = await _databaseService.GetCurrentAnswer(caseValidationQuestion) ?? String.Empty;

					_displayQuestionAnswers.Add(new DisplayQuestionAnswer { CaseValidationQuestion = caseValidationQuestion, Answer = answer });
				}
			}
			else if (thisPage == Page.NoteEntryPage)
			{
				// save the answers
				foreach (var displayQuestionAnswer in _displayQuestionAnswers)
				{
					// save the answer
					await _databaseService.SaveAnswer(displayQuestionAnswer.CaseValidationQuestion, displayQuestionAnswer.Answer);
				}

				nextPage = Page.ProviderSignoffPage;
			}
			else if (thisPage == Page.ProviderSignoffPage)
			{
				nextPage = Page.ParentSignoffPage;
			}
			else if (thisPage == Page.ParentSignoffPage)
			{
				// write the completed session to the database
				_sessionInProgress.Duration = DateTime.Now - _sessionInProgress.StartDateTime;
				_sessionInProgress.State = CaseValidationState.CompletedAwaitingSendToServer;
				await _databaseService.WriteCaseValidation(_sessionInProgress);

				// we don't have a current session anymore
				_applicationState.SetSessionInProgress(null);

				// start updating the data, but don't wait for a response as this is done in the background
				_sessionUpdateService.SendDataToServer().IgnoreResult();

				nextPage = Page.Complete;
			}
			else
			{
				// this shouldn't occur
				throw new InvalidOperationException("Unknown page state");
			}

			NavigationParameters parameters = new NavigationParameters();
			parameters.Add("PageNumber", nextPage);
			parameters.Add("IWizardController", this);

			switch (nextPage)
			{
				case Page.CompleteSessionConfirmationPage:
					await _navigationService.NavigateAsync("CompleteSessionConfirmationPage", parameters);
					break;

				case Page.NoteEntryPage:
					parameters.Add("DisplayQuestionAnswer", _displayQuestionAnswers);
					parameters.Add("AnswerValidationRequired", true);

					await _navigationService.NavigateAsync("NoteEntryPage", parameters);
					break;

				case Page.ProviderSignoffPage:
					await _navigationService.NavigateAsync("ProviderSignoffPage", parameters, useModalNavigation:true);
					break;

				case Page.ParentSignoffPage:
					await _navigationService.NavigateAsync("ParentSignoffPage", parameters, useModalNavigation: true);
					break;

				case Page.Complete:
					parameters.Add("SelectedPage", "Cases");
					parameters.Add("SubPage", "SelectedCasePage/SessionCompletePage");
					await _navigationService.NavigateAsync("file:///TabbedNavigationPage", parameters, animated: false);
					break;
			}
		}
	}
}
