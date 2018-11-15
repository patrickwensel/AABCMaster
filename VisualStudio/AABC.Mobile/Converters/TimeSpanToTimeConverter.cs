using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AABC.Mobile.Converters
{
	class TimeSpanToTimeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var timespan = (TimeSpan)value;

			DateTime timeToday = DateTime.Now.Date + timespan;

			return timeToday.ToString("h:mm") + timeToday.ToString("tt").ToLower();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new InvalidOperationException();
		}
	}
}
