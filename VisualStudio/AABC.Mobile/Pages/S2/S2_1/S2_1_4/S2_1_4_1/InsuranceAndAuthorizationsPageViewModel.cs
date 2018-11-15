using AABC.Mobile.AppServices.Extensions;
using AABC.Mobile.AppServices.Interfaces;
using AABC.Mobile.Pages.BaseViewModels;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AABC.Mobile.Pages.S2_1_4
{
	[AddINotifyPropertyChangedInterface]
	public class InsuranceAndAuthorizationsPageViewModel : SelectedCaseViewModel
	{
		readonly INavigationService _navigationService;

		public InsuranceAndAuthorizationsPageViewModel(INavigationService navigationService, IApplicationState applicationState) : base(applicationState)
		{
			_navigationService = navigationService;
		}

	}
}
