using AABC.Mobile.AppServices.Extensions;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.Entities;
using AABC.Mobile.SharedEntities.Entities;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AABC.Mobile.Pages.S2
{
	[AddINotifyPropertyChangedInterface]
	public class CasesPageViewModel : INavigationAware
	{
		public class DisplayCase
		{
			public bool ActiveSession { get; set; }
			public Case Case { get; set; }
		}


		readonly INavigationService _navigationService;
		readonly IDatabaseService _databaseService;
		readonly IApplicationState _applicationState;
		private string _subPage;
		private NavigationParameters _parameters;

		public List<DisplayCase> DisplayCases { get; set; }

		public CasesPageViewModel(INavigationService navigationService, IDatabaseService databaseService, IApplicationState applicatonState)
		{
			_navigationService = navigationService;
			_databaseService = databaseService;
			_applicationState = applicatonState;
		}

		public async Task OnSelectedCase(Case displayCase)
		{
			_applicationState.SelectedCase = displayCase;

			await _navigationService.NavigateAsync("SelectedCasePage");
		}

		public async Task OnAppearing()
		{
			var cases = await _databaseService.GetAllCases(_applicationState.CurrentUser.Username);

			var currentCaseId = _applicationState.SessionInProgress?.CaseID ?? -1;
			DisplayCases = cases
							.Select(c => new DisplayCase { Case = c, ActiveSession = (c.ID == currentCaseId) } )
							.OrderBy(c => !c.ActiveSession)
							.ThenBy(c => c.Case.Patient.PatientLastName)
							.ToList();

			if (!String.IsNullOrEmpty(_subPage))
			{
				var subPage = _subPage;
				_subPage = null;
				await _navigationService.NavigateAsync(subPage, _parameters);
				_parameters = null;
			}

		}

		public void OnNavigatedFrom(NavigationParameters parameters)
		{
		}

		public void OnNavigatedTo(NavigationParameters parameters)
		{
		}

		public void OnNavigatingTo(NavigationParameters parameters)
		{
			_subPage = parameters.GetValue<string>("SubPage");
			_parameters = parameters;
		}
	}
}
