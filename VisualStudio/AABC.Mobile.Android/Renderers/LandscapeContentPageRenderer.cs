using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AABC.Mobile.Controls;
using AABC.Mobile.Droid.Renderers;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LandscapeContentPage), typeof(LandscapeContentPageRenderer))]

namespace AABC.Mobile.Droid.Renderers
{
	public class LandscapeContentPageRenderer : PageRenderer
	{
		public LandscapeContentPageRenderer(Context context): base(context)
		{
		}

		Android.Content.PM.ScreenOrientation _previousOrientation;
		bool _applied;

		protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
			{
				Element.Appearing += Page_Appearing;
				Element.Disappearing += Page_Disappearing;
			}
			else if (e.OldElement != null)
			{
				Element.Appearing -= Page_Appearing;
				Element.Disappearing -= Page_Disappearing;
			}
		}

		void Page_Appearing(object sender, EventArgs e)
		{
			if (!_applied)
			{
				var activity = (Activity)(Context);

				_previousOrientation = activity.RequestedOrientation;

				activity.RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;

				_applied = true;
			}
		}

		void Page_Disappearing(object sender, EventArgs e)
		{
			if (_applied)
			{
				var activity = (Activity)(Context);

				activity.RequestedOrientation = _previousOrientation;

				_applied = false;
			}
		}


	}
}