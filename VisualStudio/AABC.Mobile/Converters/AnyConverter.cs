using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AABC.Mobile.Converters
{
	class AnyConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			// cast this into an enumerable interface
			var enumerable = value as System.Collections.IEnumerable;

			// if it's not valid, then we can't have any
			if (enumerable == null) return false;

			// if we can get the first value then we have some - otherwise we don't
			var enumerator = enumerable.GetEnumerator();
			return enumerator.MoveNext();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new InvalidOperationException();
		}
	}
}
