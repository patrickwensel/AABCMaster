using AABC.Mobile.Entities;
using AABC.Mobile.SharedEntities.Entities;
using Prism.Navigation;
using Xamarin.Forms;

namespace AABC.Mobile.Pages.S2
{
	public partial class CasesPage
	{
		public CasesPage()
		{
			InitializeComponent();
		}

		async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
		{
			var displayCase = e.Item as CasesPageViewModel.DisplayCase;

			var casesPageViewModel = BindingContext as CasesPageViewModel;

			await casesPageViewModel.OnSelectedCase(displayCase.Case);
		}

		protected override async void OnAppearing()
		{
			var casesPageViewModel = BindingContext as CasesPageViewModel;
			await casesPageViewModel.OnAppearing();

			// set the required row height
			Cases.HeightRequest = casesPageViewModel.DisplayCases.Count * Cases.RowHeight;

			base.OnAppearing();
		}
	}
}
