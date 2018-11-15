using AABC.Mobile.AppServices.Extensions;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.Pages.BaseViewModels;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AABC.Mobile.Pages.S2_1_1_4
{
	class SessionCompletePageViewModel : SelectedCaseViewModel, INavigatingAware
	{
		readonly INavigationService _navigationService;

		public ICommand OKCommand { get; set; }

		
		public SessionCompletePageViewModel(INavigationService navigationService, IApplicationState applicationState) : base(applicationState)
		{
			_navigationService = navigationService;

			OKCommand = new DelegateCommand(() => OnOKCommand().IgnoreResult());
		}

		async Task OnOKCommand()
		{
			// redirect to the start page
			await _navigationService.NavigateAsync("file:///TabbedNavigationPage?SelectedPage=Cases");
		}

		public void OnNavigatingTo(NavigationParameters parameters)
		{
			
		}
	}
}
