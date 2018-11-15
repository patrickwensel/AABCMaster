using AABC.Mobile.AppServices.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AABC.Mobile.MarkupExtensions
{
	/// <summary>
	/// Class ImageResourceExtension.
	/// </summary>
	/// <seealso cref="Xamarin.Forms.Xaml.IMarkupExtension" />
	[ContentProperty("Source")]
	public class ImageResourceExtension : IMarkupExtension
	{
		/// <summary>
		/// Gets or sets the source.
		/// </summary>
		/// <value>The source.</value>
		public string Source { get; set; }


		/// <summary>
		/// Returns the object created from the markup extension.
		/// </summary>
		/// <param name="serviceProvider">To be added.</param>
		/// <returns>The object</returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public object ProvideValue(IServiceProvider serviceProvider)
		{
			if (Source == null)
				return null;

			// get the current app assembly
			return Xamarin.Forms.ImageSource.FromResource("AABC.Mobile.Resources." + Source);
		}
	}
}
