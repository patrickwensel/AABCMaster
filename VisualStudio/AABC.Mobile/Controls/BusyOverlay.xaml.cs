using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AABC.Mobile.Controls
{
	/// <summary>
	/// Class BusyOverlay.
	/// </summary>
	/// <seealso cref="Xamarin.Forms.AbsoluteLayout" />
	public partial class BusyOverlay
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BusyOverlay"/> class.
		/// </summary>
		public BusyOverlay()
		{
			InitializeComponent();

			ActivityIndicator.BindingContext = this;
		}
	}
}