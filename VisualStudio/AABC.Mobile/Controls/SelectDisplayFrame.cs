using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AABC.Mobile.Controls
{
	class SelectDisplayFrame : StackLayout
	{
		public static readonly BindableProperty SelectedProperty = BindableProperty.Create("Selected", typeof(bool), typeof(SelectDisplayFrame), false, propertyChanged: (frame, o, n) => ((SelectDisplayFrame)frame).UpdateSelected((bool)n));

		public bool Selected
		{
			get { return (bool)GetValue(SelectedProperty); }
			set { SetValue(SelectedProperty, value); }
		}

		public SelectDisplayFrame()
		{
			Padding = 0;
		}

		void UpdateSelected(bool selected)
		{
			Style = selected ? (Style)Application.Current.Resources["selectedFrame"] : null;
			Padding = selected ? 3 : 0;
		}
	}
}
