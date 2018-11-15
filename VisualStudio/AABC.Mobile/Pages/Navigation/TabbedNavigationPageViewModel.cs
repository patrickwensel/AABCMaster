using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Mobile.Pages.Navigation
{
	[AddINotifyPropertyChangedInterface]
	public class TabbedNavigationPageViewModel : INavigationAware
	{
		readonly INavigationService _navigationService;

		public int CurrentTab { get; set; }

		public TabbedNavigationPageViewModel(INavigationService navigationService)
		{
			_navigationService = navigationService;
		}

		public void OnNavigatedFrom(NavigationParameters parameters)
		{
			
		}

		public void OnNavigatedTo(NavigationParameters parameters)
		{
			
		}

		public void OnNavigatingTo(NavigationParameters parameters)
		{
			var pageName = parameters.GetValue<string>("SelectedPage");

			switch (pageName)
			{
				case "Settings":
					CurrentTab = 0;
					break;

				case "Cases":
					CurrentTab = 1;
					break;

				case "About":
					CurrentTab = 2;
					break;

				default:
					CurrentTab = 1;
					break;
			}
		}
	}
}
