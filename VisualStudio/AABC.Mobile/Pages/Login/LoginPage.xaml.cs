using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AABC.Mobile.Pages.Login
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage
	{
		public LoginPage()
		{
			InitializeComponent();

			if (Device.RuntimePlatform == Device.iOS)
			{
				// make a bit of space on ios for the menu
				Padding = new Thickness(0, 40, 0, 0);
			}
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			// if we have a username, set the focus to the password entry
			var loginPageViewModel = BindingContext as LoginPageViewModel;
			if (!String.IsNullOrEmpty(loginPageViewModel.Username))
			{
				PasswordEntry.Focus();
			}
		}
	}
}