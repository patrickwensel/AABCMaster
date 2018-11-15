using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AABC.Mobile.Controls
{
	/// <summary>
	/// Enum RoundedCorners
	/// </summary>
	public enum RoundedCorners
	{
		/// <summary>
		/// The left
		/// </summary>
		left,
		/// <summary>
		/// The right
		/// </summary>
		right,
		/// <summary>
		/// All
		/// </summary>
		all,
		/// <summary>
		/// The none
		/// </summary>
		none
	}

	/// <summary>
	/// Class AdvancedFrame.
	/// </summary>
	/// <seealso cref="Xamarin.Forms.Frame" />
	public class AdvancedFrame : Frame
	{
		/// <summary>
		/// The inner background property
		/// </summary>
		public static readonly BindableProperty InnerBackgroundProperty = BindableProperty.Create("InnerBackground", typeof(Color), typeof(AdvancedFrame), default(Color));

		/// <summary>
		/// The border width property
		/// </summary>
		public static readonly BindableProperty BorderWidthProperty = BindableProperty.Create("BorderWidth", typeof(float), typeof(AdvancedFrame), 2.0f);

		/// <summary>
		/// Gets or sets the inner background.
		/// </summary>
		/// <value>The inner background.</value>
		public Color InnerBackground
		{
			get { return (Color)GetValue(InnerBackgroundProperty); }
			set { SetValue(InnerBackgroundProperty, value); }
		}

		/// <summary>
		/// Gets or sets the width of the border.
		/// </summary>
		/// <value>The width of the border.</value>
		public float BorderWidth
		{
			get { return (float)GetValue(BorderWidthProperty); }
			set { SetValue(BorderWidthProperty, value); }
		}

		/// <summary>
		/// Gets or sets the corners.
		/// </summary>
		/// <value>The corners.</value>
		public RoundedCorners Corners { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="AdvancedFrame"/> class.
		/// </summary>
		public AdvancedFrame()
		{
			BackgroundColor = Color.Transparent;
			HasShadow = false;
			Corners = RoundedCorners.none;
			this.Padding = new Thickness(0, 0, 0, 0);
		}
	}
}
