using AABC.Mobile.AppServices.Extensions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Unity;
using Prism.Ioc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using AABC.Mobile.AppServices.Interfaces;

namespace AABC.Mobile.Controls
{
	public class LoggedInPage : ContentPage
	{
		public ICommand LoginCommand { get; }

		public LoggedInPage()
		{
			LoginCommand = new DelegateCommand(() => OnLogout().IgnoreResult());

			ToolbarItems.Add(new ToolbarItem { Order = ToolbarItemOrder.Primary, Text = "Logout", Priority = 0, Command = LoginCommand });
		}

		async Task OnLogout()
		{
			if (await this.DisplayAlert("Logout", "Are you sure you want to log out?", "Logout", "Cancel"))
			{
				var accountService = ((PrismApplication)App.Current).Container.Resolve<IAccountService>();
				accountService.Logout();

				var navigationService = ((AABC.Mobile.App)App.Current).GetNavigationService();
				await navigationService.NavigateAsync("file:///LoginPage");
			}
		}
	}
}