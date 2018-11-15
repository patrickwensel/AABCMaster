using System;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace AABC.Mobile.Pages.Navigation
{
	public partial class TabbedNavigationPage
	{
		public static readonly BindableProperty SelectedTabIndexProperty = BindableProperty.Create("SelectedTabIndex", typeof(int), typeof(TabbedNavigationPage), 0, propertyChanged: (tabbedNavigationPage, o, n) => ((TabbedNavigationPage)tabbedNavigationPage).SelectedTabIndexChanged((int)n));

		public int SelectedTabIndex
		{
			get { return (int)GetValue(SelectedTabIndexProperty); }
			set { SetValue(SelectedTabIndexProperty, value); }
		}

		public TabbedNavigationPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
		}

		void SelectedTabIndexChanged(int n)
		{
			// Device.StartTimer(TimeSpan.FromMilliseconds(100), () => { this.CurrentPage = this.Children[n]; return false; });

			Device.BeginInvokeOnMainThread(() => { this.CurrentPage = this.Children[n]; });
		}
	}
}
