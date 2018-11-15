using AABC.Mobile.AppServices.Entities;
using AABC.Mobile.AppServices.Extensions;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.Entities;
using AABC.Mobile.Pages.BaseViewModels;
using AABC.Mobile.SharedEntities.Messages;
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

namespace AABC.Mobile.Pages.S2_1_2
{
	[AddINotifyPropertyChangedInterface]
	public class SessionStartPageViewModel: SelectedCaseViewModel, INavigationAware
	{
		readonly INavigationService _navigationService;
		readonly IDatabaseService _databaseService;
		readonly IApplicationState _applicationState;
		readonly IDataUpdateService _dataUpdateService;
		readonly IConnectivity _connectivity;
		bool _reloadCaseValidation;

		public ICommand BeginSessionCommand { get; set; }
		public ICommand CancelCommand { get; set; }
		public ICommand StartPreCheckClickedCommand { get; set; }

		public ICommand StartSessionCommand { get; set; }
		

		public bool StartPreCheckAvailable { get; set; }

		public List<CaseValidation> CaseValidations { get; set; }


		public SessionStartPageViewModel(INavigationService navigationService, IConnectivity connectivity, IDatabaseService databaseService, IApplicationState applicationState, IDataUpdateService dataUpdateService) : base(applicationState)
		{
			_navigationService = navigationService;
			_databaseService = databaseService;
			_applicationState = applicationState;
			_dataUpdateService = dataUpdateService;
			_connectivity = connectivity;

			BeginSessionCommand = new DelegateCommand(() => OnBeginSessionCommand().IgnoreResult());
			CancelCommand = new DelegateCommand(() => OnCancelCommand().IgnoreResult());
			StartPreCheckClickedCommand = new DelegateCommand(() => OnStartPreCheckClickedCommand().IgnoreResult());
			StartSessionCommand = new DelegateCommand<CaseValidation>((caseValidation) => StartSessionCommandCommand(caseValidation).IgnoreResult());
		}


		async Task OnBeginSessionCommand()
		{
			await _navigationService.GoBackAsync();
		}

		async Task OnCancelCommand()
		{
			await _navigationService.GoBackAsync();
		}

		async Task OnStartPreCheckClickedCommand()
		{
			_reloadCaseValidation = true;

			// start the wizard
			var preCheckWizardController = new PreCheckWizardController(SelectedCase, _navigationService, _dataUpdateService, _databaseService, _applicationState);
			await preCheckWizardController.NextPage(-1);
		}

		async Task StartSessionCommandCommand(CaseValidation caseValidation)
		{
			NavigationParameters navigationParameters = new NavigationParameters();
			navigationParameters.Add("CaseValidation", caseValidation);

			await _navigationService.NavigateAsync("ConfirmSessionStartPage", navigationParameters);
		}

		public async void OnNavigatingTo(NavigationParameters parameters)
		{
			StartPreCheckAvailable = _connectivity.IsConnected;

			CaseValidations = await _databaseService.GetAllValidCaseValidations(SelectedCase);
		}

		public void OnNavigatedFrom(NavigationParameters parameters)
		{
		}

		public async void OnNavigatedTo(NavigationParameters parameters)
		{
			StartPreCheckAvailable = _connectivity.IsConnected;

			var validateResponse = parameters.GetValue<ValidationResponse>("ValidationResponse");
			if (validateResponse != null)
			{
				await _navigationService.NavigateAsync("BaseInfoResultsPage", parameters);
			}
			else if (_reloadCaseValidation)
			{
				_reloadCaseValidation = false;
				CaseValidations = await _databaseService.GetAllValidCaseValidations(SelectedCase);
			}
		}
	}
}
