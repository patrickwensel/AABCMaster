using Xamarin.Forms;

namespace AABC.Mobile.Pages.S2
{
	public partial class CaseDetailsPage
	{
		public CaseDetailsPage()
		{
			InitializeComponent();

			this.Appearing += CaseDetailsPage_Appearing;
		}

		void CaseDetailsPage_Appearing(object sender, System.EventArgs e)
		{
			// set the actual height of the InformationList
			CaseDetailsPageViewModel viewModel = this.BindingContext as CaseDetailsPageViewModel;

			InformationList.HeightRequest = InformationList.RowHeight * viewModel.InformationList.Count;
		}
	}
}
