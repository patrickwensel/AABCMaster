using AABC.Mobile.AppServices.Extensions;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.Interfaces;
using AABC.Mobile.Pages.BaseViewModels;
using Prism.Commands;
using Prism.Navigation;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AABC.Mobile.Pages.S2_1_1_1
{
	[AddINotifyPropertyChangedInterface]
	class CompleteSessionConfirmationPageViewModel : SelectedCaseViewModel, INavigatingAware
	{
		readonly INavigationService _navigationService;

		IWizardController _wizardController;
		int _pageNumber;

		public ICommand ConfirmAndCompleteCommand { get; set; }

		public ICommand CancelCommand { get; set; }
		
		public CompleteSessionConfirmationPageViewModel(INavigationService navigationService, IApplicationState applicationState) : base(applicationState)
		{
			_navigationService = navigationService;

			ConfirmAndCompleteCommand = new DelegateCommand(() => OnConfirmAndCompleteCommand().IgnoreResult());
			CancelCommand = new DelegateCommand(() => OnCancelCommand().IgnoreResult());

		}

		async Task OnCancelCommand()
		{
			await _navigationService.GoBackAsync();
		}

		async Task OnConfirmAndCompleteCommand()
		{
			await _wizardController.NextPage(_pageNumber);
		}

		public void OnNavigatingTo(NavigationParameters parameters)
		{
			_wizardController = parameters.GetValue<IWizardController>("IWizardController");
			_pageNumber = parameters.GetValue<int>("PageNumber");

		}
	}
}
