using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AABC.Mobile.Droid.Renderers;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using DatePicker = Xamarin.Forms.DatePicker;
using TimePicker = Xamarin.Forms.TimePicker;

[assembly: ExportRenderer(typeof(Entry), typeof(CustomEntryRenderer))]

namespace AABC.Mobile.Droid.Renderers
{
	[Preserve(AllMembers = true)]
	public class CustomEntryRenderer : EntryRenderer
	{
		public CustomEntryRenderer(Context context): base(context)
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			if (Control != null)
			{
				var shape = new ShapeDrawable(new RectShape());
				shape.Paint.Color = global::Android.Graphics.Color.Transparent;
				shape.Paint.StrokeWidth = 0;
				shape.Paint.SetStyle(Paint.Style.Stroke);
				Control.Background = shape;
			}
		}
	}
}