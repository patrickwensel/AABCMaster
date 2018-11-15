using AABC.Mobile.AppServices.Entities;
using AABC.Mobile.AppServices.Extensions;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.Interfaces;
using AABC.Mobile.Pages.BaseViewModels;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AABC.Mobile.Pages.S2_1_1_3
{
	class ParentSignoffPageViewModel : SelectedCaseViewModel, INavigatingAware
	{
		readonly INavigationService _navigationService;
		readonly IApplicationState _applicationState;
		readonly IDatabaseService _databaseService;
		readonly ISessionUpdateService _sessionUpdateService;

		public ICommand ContinueCommand { get; set; }

		public byte[] SignatureBytes { get; set; }

		IWizardController _wizardController;
		int _pageNumber;

		public ParentSignoffPageViewModel(INavigationService navigationService, IApplicationState applicationState, IDatabaseService databaseService, ISessionUpdateService sessionUpdateService) : base(applicationState)
		{
			_navigationService = navigationService;
			_applicationState = applicationState;
			_databaseService = databaseService;
			_sessionUpdateService = sessionUpdateService;

			ContinueCommand = new DelegateCommand(() => OnContinueCommand().IgnoreResult());
		}

		async Task OnContinueCommand()
		{
			await _databaseService.WriteSignature(SessionInProgress, SignatureType.Parent, SignatureBytes);

			await _wizardController.NextPage(_pageNumber);
		}

		public void OnNavigatingTo(NavigationParameters parameters)
		{
			_wizardController = parameters.GetValue<IWizardController>("IWizardController");
			_pageNumber = parameters.GetValue<int>("PageNumber");

		}
	}
}
