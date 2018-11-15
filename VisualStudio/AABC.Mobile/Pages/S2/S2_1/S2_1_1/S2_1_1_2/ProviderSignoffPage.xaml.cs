using AABC.Mobile.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AABC.Mobile.Pages.S2_1_1_2
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ProviderSignoffPage : LandscapeContentPage
	{
		public ProviderSignoffPage()
		{
			InitializeComponent();

			padView.StrokeCompleted += PadView_StrokeCompleted;
		}

		async void PadView_StrokeCompleted(object sender, EventArgs e)
		{
			var imageStream = await padView.GetImageStreamAsync(SignaturePad.Forms.SignatureImageFormat.Png);

			using (var memory = new MemoryStream())
			{
				await imageStream.CopyToAsync(memory);

				var providerSignoffPageViewModel = BindingContext as ProviderSignoffPageViewModel;
				providerSignoffPageViewModel.SignatureBytes = memory.ToArray();
			}
		}
	}
}