using AABC.Mobile.Droid.Renderers;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.Runtime;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(TimePicker), typeof(CustomTimePickerRenderer))]

namespace AABC.Mobile.Droid.Renderers
{
	[Preserve(AllMembers = true)]
	public class CustomTimePickerRenderer : TimePickerRenderer
	{
		public CustomTimePickerRenderer(Context context) : base(context)
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
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