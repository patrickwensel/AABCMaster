using AABC.Mobile.SharedEntities.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AABC.Mobile.Converters
{
	class GenderImageConverter : IValueConverter
	{
		public string MaleImage { get; set; }

		public string FemaleImage { get; set; }

		public string UnknownImage { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var gender = (Gender)value;

			switch (gender)
			{
				case Gender.Male: return Xamarin.Forms.ImageSource.FromResource("AABC.Mobile.Resources." + MaleImage);
				case Gender.Female: return Xamarin.Forms.ImageSource.FromResource("AABC.Mobile.Resources." + FemaleImage);
				default: return Xamarin.Forms.ImageSource.FromResource("AABC.Mobile.Resources." + UnknownImage);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new InvalidOperationException();
		}
	}
}
