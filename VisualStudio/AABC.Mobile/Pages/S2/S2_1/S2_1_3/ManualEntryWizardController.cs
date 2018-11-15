using AABC.Mobile.AppServices.Entities;
using AABC.Mobile.AppServices.Extensions;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.Entities;
using AABC.Mobile.Interfaces;
using AABC.Mobile.Pages.C1;
using AABC.Mobile.SharedEntities.Entities;
using AABC.Mobile.SharedEntities.Messages;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.Pages.S2_1_3
{
	class ManualEntryWizardController : IWizardController
	{
		readonly INavigationService _navigationService;

		readonly IDataUpdateService _dataUpdateService;

		readonly IDatabaseService _databaseService;

		readonly ISessionUpdateService _sessionUpdateService;
		readonly IApplicationState _applicationState;

		ValidationResponse _validationResponse;
		CaseValidation _caseValidation;

		EntryBaseInfoPageModel _entryBaseInfoPageModel = new EntryBaseInfoPageModel();

		List<int> _selectedCaseIds = new List<int>();
		List<DisplayQuestionAnswer> DisplayQuestionAnswers = new List<DisplayQuestionAnswer>();

		enum Page
		{
			Initial = -1,
			EntryBaseInfoPage,
			SsgSelectionPage,
			BaseInfoResultsPage,
			NoteEntryPage,
			Complete
		}

		public ManualEntryWizardController(Case thisCase, INavigationService navigationService, IDataUpdateService dataUpdateService, IDatabaseService databaseService, ISessionUpdateService sessionUpdateService, IApplicationState applicationState)
		{
			_navigationService = navigationService;
			_dataUpdateService = dataUpdateService;
			_databaseService = databaseService;
			_sessionUpdateService = sessionUpdateService;
			_applicationState = applicationState;

			var now = DateTime.Now;
			_entryBaseInfoPageModel.Case = thisCase;
			_entryBaseInfoPageModel.DateOfService = now.Date;
			_entryBaseInfoPageModel.StartTime = (now - now.Date).RoundUpTo(15);
			_entryBaseInfoPageModel.EndTime = _entryBaseInfoPageModel.StartTime + TimeSpan.FromMinutes(30);
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
				nextPage = Page.EntryBaseInfoPage;
			}
			else if (thisPage == Page.EntryBaseInfoPage && _entryBaseInfoPageModel.SelectedService.IsSsg)
			{
				// this requires ssg information
				nextPage = Page.SsgSelectionPage;
			}
			else if (thisPage == Page.SsgSelectionPage || (thisPage == Page.EntryBaseInfoPage && !_entryBaseInfoPageModel.SelectedService.IsSsg))
			{
				// validate the request
				_caseValidation = GenerateCaseValidationRequest(_applicationState.CurrentUser.Username);
				_validationResponse = await _dataUpdateService.ValidateRequest(_caseValidation, false);
				if (_validationResponse.Errors == null || !_validationResponse.Errors.Any())
				{
					// the request has been validated
					_caseValidation.State = CaseValidationState.Valid;
					_caseValidation.ServerValidatedSessionID = _validationResponse.ServerValidatedSessionID;
				}

				// go to the display error/warning page
				nextPage = Page.BaseInfoResultsPage;
			}
			else if (thisPage == Page.BaseInfoResultsPage)
			{
				// we've just displayed the errors
				if (_caseValidation.State == CaseValidationState.Valid)
				{
					// we're valid, so go to the next page
					DisplayQuestionAnswers = new List<DisplayQuestionAnswer>();
					var caseValidationQuestions = _validationResponse.NoteQuestions.Select(nq => new CaseValidationQuestion { Question = nq.Question }).ToList();
					foreach (var caseValidationQuestion in caseValidationQuestions)
					{
						DisplayQuestionAnswers.Add(new DisplayQuestionAnswer { CaseValidationQuestion = caseValidationQuestion, Answer = String.Empty });
					}

					// and go to the next page
					nextPage = Page.NoteEntryPage;
				}
				else
				{
					// not valid, so back to the start
					nextPage = Page.EntryBaseInfoPage;
				}
			}
			else if (thisPage == Page.NoteEntryPage)
			{
				// save the data to the database
				await _databaseService.WriteCaseValidation(_caseValidation);

				// save the questions
				await _databaseService.WriteCaseValidationQuestions(_caseValidation.CaseValidationID, _validationResponse.NoteQuestions);

				var caseValidationQuestions = await _databaseService.GetCaseValidationQuestions(_caseValidation.CaseValidationID);

				int index = 0;
				foreach (var displayQuestionAnswer in DisplayQuestionAnswers)
				{
					// update the question id
					displayQuestionAnswer.CaseValidationQuestion.CaseValidationQuestionID = caseValidationQuestions[index].CaseValidationQuestionID;

					// save the answer
					await _databaseService.SaveAnswer(displayQuestionAnswer.CaseValidationQuestion, displayQuestionAnswer.Answer);

					index++;
				}

				_caseValidation.State = CaseValidationState.CompletedAwaitingSendToServer;

				await _databaseService.WriteCaseValidation(_caseValidation);

				// start updating the data, but don't wait for a response as this is done in the background
				_sessionUpdateService.SendDataToServer().IgnoreResult();

				// go back to the main screen
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
				case Page.EntryBaseInfoPage:
					parameters.Add("EntryBaseInfoPageModel", _entryBaseInfoPageModel);
					await _navigationService.NavigateAsync("EntryBaseInfoPage", parameters);
					break;

				case Page.SsgSelectionPage:
					parameters.Add("SelectedCase", _entryBaseInfoPageModel.Case);
					parameters.Add("SelectedCaseIds", _selectedCaseIds);
					await _navigationService.NavigateAsync("SsgSelectionPage", parameters);
					break;

				case Page.BaseInfoResultsPage:
					parameters.Add("DisplayButtons", true);
					parameters.Add("Message", "Your session is valid");

					parameters.Add("CaseValidation", _caseValidation);
					parameters.Add("ValidationResponse", _validationResponse);
					await _navigationService.NavigateAsync("BaseInfoResultsPage", parameters);
					break;

				case Page.NoteEntryPage:
					parameters.Add("DisplayQuestionAnswer", DisplayQuestionAnswers);
					parameters.Add("AnswerValidationRequired", true);
					
					await _navigationService.NavigateAsync("NoteEntryPage", parameters);
					break;
					
				case Page.Complete:
					parameters.Add("SelectedPage", "Cases");
					parameters.Add("SubPage", "SelectedCasePage/SessionCompletePage");
					await _navigationService.NavigateAsync("file:///TabbedNavigationPage", parameters, animated: false);
					break;
			}
		}


		CaseValidation GenerateCaseValidationRequest(string userName)
		{
			// call the server to validate this time
			var duration = _entryBaseInfoPageModel.EndTime - _entryBaseInfoPageModel.StartTime;

			var caseValidation = new CaseValidation
			{
				CaseID = _entryBaseInfoPageModel.Case.ID,
				UserName = userName,
				DateOfService = _entryBaseInfoPageModel.DateOfService,
				StartTime = _entryBaseInfoPageModel.StartTime,
				Duration = duration,
				ServiceID = _entryBaseInfoPageModel.SelectedService.ID,
				ServiceDescription = _entryBaseInfoPageModel.SelectedService.Description,
				LocationID = _entryBaseInfoPageModel.SelectedLocation.ID,
				LocationDescription = _entryBaseInfoPageModel.SelectedLocation.Description,
				SsgCaseIds = _entryBaseInfoPageModel.SelectedService.IsSsg ? String.Join(", ", _selectedCaseIds) : null,
			};
			return caseValidation;
		}


	}
}
