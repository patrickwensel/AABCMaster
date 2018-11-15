using AABC.Mobile.AppServices.Entities;
using AABC.Mobile.AppServices.Extensions;
using AABC.Mobile.AppServices.Interfaces;
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

namespace AABC.Mobile.Pages.S2_1_2
{
	class PreCheckWizardController : IWizardController
	{
		readonly INavigationService _navigationService;

		readonly IDataUpdateService _dataUpdateService;

		readonly IDatabaseService _databaseService;
		readonly IApplicationState _applicationState;

		EntryBaseInfoPageModel _entryBaseInfoPageModel = new EntryBaseInfoPageModel();

		List<int> _selectedCaseIds = new List<int>();

		public PreCheckWizardController(Case thisCase, INavigationService navigationService, IDataUpdateService dataUpdateService, IDatabaseService databaseService, IApplicationState applicationState)
		{
			_navigationService = navigationService;
			_dataUpdateService = dataUpdateService;
			_databaseService = databaseService;
			_applicationState = applicationState;

			var now = DateTime.Now;
			_entryBaseInfoPageModel.Case = thisCase;
			_entryBaseInfoPageModel.DateOfService = now.Date;
			_entryBaseInfoPageModel.StartTime = (now - now.Date).RoundUpTo(15);
			_entryBaseInfoPageModel.EndTime = _entryBaseInfoPageModel.StartTime + TimeSpan.FromMinutes(30);
		}

		public async Task Cancel()
		{
			// go back to the main screen
			await _navigationService.NavigateAsync("file:///TabbedNavigationPage?SelectedPage=Cases");
		}

		public async Task Complete()
		{
			// save the data

			// go back to the main screen
			await _navigationService.NavigateAsync("file:///TabbedNavigationPage?SelectedPage=Cases");
		}

		enum Page
		{
			Initial = -1,
			EntryBaseInfoPage,
			SsgSelectionPage,
			BaseInfoResultsPage,
			Complete
		}


		public async Task NextPage(int thisPageNumber)
		{
			Page thisPage = (Page)thisPageNumber;

			Page nextPage;

			ValidationResponse validationResponse = null;
			CaseValidation caseValidation = null;

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
			else if (thisPage != Page.Complete)
			{
				// validate the request
				caseValidation = GenerateCaseValidationRequest(_applicationState.CurrentUser.Username);
				validationResponse = await _dataUpdateService.ValidateRequest(caseValidation);
				if (validationResponse.Errors == null || !validationResponse.Errors.Any())
				{
					// the request has been validated
					caseValidation.State = CaseValidationState.Valid;
					caseValidation.ServerValidatedSessionID = validationResponse.ServerValidatedSessionID;
					caseValidation.Case = _applicationState.SelectedCase;

					await _databaseService.WriteCaseValidation(caseValidation);

					// save the questions
					await _databaseService.WriteCaseValidationQuestions(caseValidation.CaseValidationID, validationResponse.NoteQuestions);

					// and go to the final screen
					nextPage = Page.Complete;
				}
				else
				{
					// go to the error page
					nextPage = Page.BaseInfoResultsPage;
				}
			}
			else
			{
				// this shouldn't occur
				throw new InvalidOperationException("Unknown page state");
			}

			NavigationParameters parameters = new NavigationParameters();

			if (nextPage != Page.Complete)
			{
				parameters.Add("PageNumber", nextPage);
				parameters.Add("IWizardController", this);
			}

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
					parameters.Add("CaseValidation", caseValidation);
					parameters.Add("ValidationResponse", validationResponse);
					await _navigationService.NavigateAsync("BaseInfoResultsPage", parameters);
					break;

				case Page.Complete:
					parameters.Add("DisplayButtons", false);
					parameters.Add("Message", "Your session has been booked");

					parameters.Add("CaseValidation", caseValidation);
					parameters.Add("ValidationResponse", validationResponse);
					parameters.Add("SelectedPage", "Cases"); 
					parameters.Add("SubPage", "SelectedCasePage/BaseInfoResultsPage");


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
