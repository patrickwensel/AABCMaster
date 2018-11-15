using AABC.Mobile.AppServices.Entities;
using AABC.Mobile.AppServices.Extensions;
using AABC.Mobile.Interfaces;
using AABC.Mobile.SharedEntities.Messages;
using Prism.Commands;
using Prism.Navigation;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AABC.Mobile.Pages.C1_1
{
	[AddINotifyPropertyChangedInterface]
	class BaseInfoResultsPageViewModel : INavigatingAware
	{
		readonly INavigationService _navigationService;

		int _pageNumber;
		IWizardController _wizardController;

		public bool DisplayErrors { get; private set; }

		public string Message { get; private set; }

		public bool DisplayContinueButton { get; private set; }
		public bool DisplayCancelButton { get; private set; }

		public ValidationResponse ValidationResponse { get; set; }

		public ICommand ContinueCommand { get; set; }

		public ICommand CancelCommand { get; set; }

		public BaseInfoResultsPageViewModel(INavigationService navigationService)
		{
			_navigationService = navigationService;

			ContinueCommand = new DelegateCommand(() => OnContinueCommand().IgnoreResult());
			CancelCommand = new DelegateCommand(() => OnCancelCommand().IgnoreResult());
		}

		async Task OnContinueCommand()
		{
			// only go to the next page if we're not displaying errors - and we have a wizard so we can
			if (!DisplayErrors && _wizardController != null)
			{
				await _wizardController.NextPage(_pageNumber);
			}
			else
			{
				await _navigationService.GoBackAsync();
			}
		}

		async Task OnCancelCommand()
		{
			await _wizardController.Cancel();
		}


		public void OnNavigatingTo(NavigationParameters navigationParameters)
		{
			_wizardController = navigationParameters.GetValue<IWizardController>("IWizardController");
			_pageNumber = navigationParameters.GetValue<int>("PageNumber");

			var displayButtons = navigationParameters.GetValue<bool>("DisplayButtons");

			Message = navigationParameters.GetValue<string>("Message");

			ValidationResponse = navigationParameters.GetValue<ValidationResponse>("ValidationResponse");
			DisplayErrors = ValidationResponse.Errors.Any();

			DisplayContinueButton = !DisplayErrors;
			DisplayCancelButton = DisplayErrors;
		}
	}
}
