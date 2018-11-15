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
using Xamarin.Forms;

namespace AABC.Mobile.Pages.S2_1_1_2
{
	class ProviderSignoffPageViewModel : SelectedCaseViewModel, INavigatingAware
	{
		readonly INavigationService _navigationService;
		readonly IDatabaseService _databaseService;

		IWizardController _wizardController;
		int _pageNumber;

		public ICommand ContinueCommand { get; set; }


		public byte[] SignatureBytes { get; set; }

		public ProviderSignoffPageViewModel(INavigationService navigationService, IApplicationState applicationState, IDatabaseService databaseService) : base(applicationState)
		{
			_navigationService = navigationService;
			_databaseService = databaseService;

			ContinueCommand = new DelegateCommand(() => OnContinueCommand().IgnoreResult());
		}

		async Task OnContinueCommand()
		{
			await _databaseService.WriteSignature(SessionInProgress, SignatureType.Provider, SignatureBytes);

			await _wizardController.NextPage(_pageNumber);
		}

		public void OnNavigatingTo(NavigationParameters parameters)
		{
			_wizardController = parameters.GetValue<IWizardController>("IWizardController");
			_pageNumber = parameters.GetValue<int>("PageNumber");

		}
	}
}
