using AABC.Mobile.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AABC.Mobile.Pages.S2_1_1_3
{
	public partial class ParentSignoffPage : LandscapeContentPage
	{
		public ParentSignoffPage()
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

				var parentSignoffPageViewModel = BindingContext as ParentSignoffPageViewModel;
				parentSignoffPageViewModel.SignatureBytes = memory.ToArray();
			}
		}
	}
}