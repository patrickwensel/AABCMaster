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
	class PatientNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var patient = value as Patient;

			if (value == null)
			{
				return String.Empty;
			}
			else
			{
				return patient.PatientFirstName + " " + patient.PatientLastName.Substring(0, 1);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new InvalidOperationException();
		}
	}
}
