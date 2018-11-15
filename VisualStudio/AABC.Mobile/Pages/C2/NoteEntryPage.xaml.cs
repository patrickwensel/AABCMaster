using Xamarin.Forms;

namespace AABC.Mobile.Pages.C2
{
	public partial class NoteEntryPage
	{
		public NoteEntryPage()
		{
			InitializeComponent();
		}

		protected async override void OnDisappearing()
		{
			var noteEntryPageViewModel = BindingContext as NoteEntryPageViewModel;

			if (!noteEntryPageViewModel.DontSaveOnClose)
			{
				// save on the back command
				await noteEntryPageViewModel.SaveData();
			}

			base.OnDisappearing();
		}
	}
}
