using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AABC.Mobile.Controls
{
	public partial class IconButton : StackLayout
	{
		public static readonly BindableProperty CommandProperty = BindableProperty.Create("Command", typeof(ICommand), typeof(IconButton), null, propertyChanged: (iconButton, o, n) => ((IconButton)iconButton).OnCommandChanged((ICommand)n));

		public static readonly BindableProperty TextProperty = BindableProperty.Create("Text", typeof(string), typeof(IconButton), null, propertyChanged: (iconButton, o, n) => ((IconButton)iconButton).OnTextChanged((string)n));

		public static readonly BindableProperty ImageSourceProperty = BindableProperty.Create("ImageSource", typeof(string), typeof(IconButton), null, propertyChanged: (iconButton, o, n) => ((IconButton)iconButton).OnImageSourceChanged((string)n));

		public static readonly BindableProperty DisabledProperty = BindableProperty.Create("Disabled", typeof(bool), typeof(IconButton), false, propertyChanged: (iconButton, o, n) => ((IconButton)iconButton).OnDisabledChanged((bool)n));

		public static readonly BindableProperty DisabledTextProperty = BindableProperty.Create("DisabledText", typeof(string), typeof(IconButton), null, propertyChanged: (iconButton, o, n) => ((IconButton)iconButton).OnDisabledTextChanged((string)n));

		public static readonly BindableProperty HighlightProperty = BindableProperty.Create("Highlight", typeof(bool), typeof(IconButton), false, propertyChanged: (iconButton, o, n) => ((IconButton)iconButton).OnHighlightChanged((bool)n));

		void OnDisabledTextChanged(string disabledText)
		{
			DisabledLabel.Text = disabledText;
		}

		void OnHighlightChanged(bool n)
		{
			UpdateDisplay();

		}

		void OnDisabledChanged(bool disabled)
		{
			UpdateDisplay();
		}

		void OnImageSourceChanged(string newSource)
		{
			ButtonImage.Source = Xamarin.Forms.ImageSource.FromResource("AABC.Mobile.Resources.ButtonImages." + newSource);
		}

		void OnCommandChanged(ICommand newCommand)
		{
		}

		void OnTextChanged(string newText)
		{
			ButtonLabel.Text = newText;
		}

		public ICommand Command
		{
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public string ImageSource
		{
			get { return (string)GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}

		public bool Disabled
		{
			get { return (bool)GetValue(DisabledProperty); }
			set { SetValue(DisabledProperty, value); }
		}

		public string DisabledText
		{
			get { return (string)GetValue(DisabledTextProperty); }
			set { SetValue(DisabledTextProperty, value); }
		}

		public bool Highlight
		{
			get { return (bool)GetValue(HighlightProperty); }
			set { SetValue(HighlightProperty, value); }
		}


		public IconButton()
		{
			InitializeComponent();

			UpdateDisplay();
		}

		protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			if (propertyName == "BackgroundColor")
			{
				UpdateDisplay();
			}

			base.OnPropertyChanged(propertyName);
		}

		void UpdateDisplay()
		{
			DisabledLabel.IsVisible = Disabled;

			var globalResources = App.Current.Resources;

			BackgroundColor = Disabled ? (Color)globalResources["disabledBackground"] :
									Highlight ? (Color)globalResources["selectedBackground"] : Color.White;
			ButtonLabel.TextColor = Disabled ? (Color)globalResources["disabledText"] : (Color)globalResources["normalText"];
		}

		void TapGestureCommand_Tapped(object sender, EventArgs e)
		{
			if (!Disabled)
			{
				Command.Execute(null);
			}
		}
	}
}