using Xamarin.Forms;

namespace AABC.Mobile.Pages.S2_1_4
{
	public partial class InsuranceAndAuthorizationsPage
	{
		public InsuranceAndAuthorizationsPage()
		{
			InitializeComponent();

			this.Appearing += InsuranceAndAuthorizationsPage_Appearing;
		}

		void InsuranceAndAuthorizationsPage_Appearing(object sender, System.EventArgs e)
		{
			// set the actual height of the InformationList
			InsuranceAndAuthorizationsPageViewModel viewModel = this.BindingContext as InsuranceAndAuthorizationsPageViewModel;

			InformationList.HeightRequest = InformationList.RowHeight * viewModel.SelectedCase.ActiveAuthorizations.Count;
		}
	}
}
