using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Drawing;
using UIKit;
using System.ComponentModel;
using AABC.Mobile.Controls;
using AABC.Mobile.iOS.Renderers;

[assembly: ExportRenderer(typeof(AdvancedFrame), typeof(AdvancedFrameRenderer))]

namespace AABC.Mobile.iOS.Renderers
{
	public class AdvancedFrameRenderer : VisualElementRenderer<AdvancedFrame>
	{
		protected override void OnElementChanged(ElementChangedEventArgs<AdvancedFrame> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement != null)
				SetupLayer();
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged(sender, e);

			if (e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName ||
				e.PropertyName == Xamarin.Forms.Frame.OutlineColorProperty.PropertyName ||
				e.PropertyName == Xamarin.Forms.Frame.HasShadowProperty.PropertyName ||
				e.PropertyName == Xamarin.Forms.Frame.CornerRadiusProperty.PropertyName)
				SetupLayer();
		}

		void SetupLayer()
		{
			float cornerRadius = Element.CornerRadius;

			if (cornerRadius == -1f)
				cornerRadius = 5f; // default corner radius

			Layer.CornerRadius = cornerRadius;

			if (Element.InnerBackground == Color.Default)
				Layer.BackgroundColor = UIColor.White.CGColor;
			else
				Layer.BackgroundColor = Element.InnerBackground.ToCGColor();

			if (Element.HasShadow)
			{
				Layer.ShadowRadius = 5;
				Layer.ShadowColor = UIColor.Black.CGColor;
				Layer.ShadowOpacity = 0.8f;
				Layer.ShadowOffset = new SizeF();
			}
			else
				Layer.ShadowOpacity = 0;

			if (Element.OutlineColor == Color.Default)
				Layer.BorderColor = UIColor.Clear.CGColor;
			else
			{
				Layer.BorderColor = Element.OutlineColor.ToCGColor();
				Layer.BorderWidth = (nfloat)Math.Ceiling(Element.BorderWidth / 2);
			}

			Layer.RasterizationScale = UIScreen.MainScreen.Scale;
			Layer.ShouldRasterize = true;
		}
	}
}
