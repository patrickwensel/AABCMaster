using AABC.Mobile.AppServices.Extensions;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.Controls;
using AABC.Mobile.Interfaces;
using AABC.Mobile.Pages.BaseViewModels;
using AABC.Mobile.SharedEntities.Entities;
using Prism.Commands;
using Prism.Navigation;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AABC.Mobile.Pages.C1
{
	[AddINotifyPropertyChangedInterface]
	class SsgSelectionPageViewModel : INavigatingAware
	{
		readonly INavigationService _navigationService;
		readonly IApplicationState _applicationState;
		readonly IDatabaseService _databaseService;

		IWizardController _wizardController;
		int _pageNumber;
		Case _selectedCase;


		public ICommand ContinueCommand { get; set; }
		public ICommand CancelCommand { get; set; }


		public List<Case> Cases { get; set; }


		public List<int> SelectedCaseIds { get; set; }

		public bool DisplayNoAdditionalAttendeesSelected { get; set; }

		public ICommand SelectedUsersChangedCommand { get; set; }

		public SsgSelectionPageViewModel(INavigationService navigationService, IApplicationState applicationState, IDatabaseService databaseService)
		{
			_navigationService = navigationService;
			_applicationState = applicationState;
			_databaseService = databaseService;

			// show the error message if no users are selected
			SelectedUsersChangedCommand = new DelegateCommand<SelectedUsersChangedEventArgs>((e) => DisplayNoAdditionalAttendeesSelected = !e.SelectedCaseIds.Any());
		}

		public async void OnNavigatingTo(NavigationParameters parameters)
		{
			_wizardController = parameters.GetValue<IWizardController>("IWizardController");
			_pageNumber = parameters.GetValue<int>("PageNumber");
			_selectedCase = parameters.GetValue<Case>("SelectedCase");
			SelectedCaseIds = parameters.GetValue<List<int>>("SelectedCaseIds");

			var cases = await _databaseService.GetAllCases(_applicationState.CurrentUser.Username);
			Cases = cases.OrderBy(c => c.ID != _selectedCase.ID)
						.ThenBy(c => c.Patient.PatientLastName)
						.ToList();


			ContinueCommand = new DelegateCommand(() => OnContinueCommand().IgnoreResult());
			CancelCommand = new DelegateCommand(() => OnCancelCommand().IgnoreResult());

		}

		async Task OnContinueCommand()
		{
			if (!SelectedCaseIds.Any())
			{
				// no other attendees have been selected, so show the error and don't navigate away
				DisplayNoAdditionalAttendeesSelected = true;
				return;
			}
			await _wizardController.NextPage(_pageNumber);
		}

		async Task OnCancelCommand()
		{
			await _navigationService.GoBackAsync();
		}

	}
}
