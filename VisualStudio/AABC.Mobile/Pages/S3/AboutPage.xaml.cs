using Xamarin.Forms;

namespace AABC.Mobile.Pages.S3
{
	public partial class AboutPage
	{
		public AboutPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			var aboutPageViewModel = BindingContext as AboutPageViewModel;

			aboutPageViewModel.OnNavigatingTo(null);
		}
	}
}
