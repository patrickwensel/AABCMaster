using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AABC.Mobile.Converters
{
	public class BooleanNegationConverter : IValueConverter
	{
		/// <summary>
		/// Implement this method to convert <paramref name="value" /> to <paramref name="targetType" /> by using <paramref name="parameter" /> and <paramref name="culture" />.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="parameter">The parameter.</param>
		/// <param name="culture">The culture.</param>
		/// <returns>System.Object.</returns>
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return !(bool)value;
		}
		/// <summary>
		/// Implement this method to convert <paramref name="value" /> back from <paramref name="targetType" /> by using <paramref name="parameter" /> and <paramref name="culture" />.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="parameter">The parameter.</param>
		/// <param name="culture">The culture.</param>
		/// <returns>System.Object.</returns>
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return !(bool)value;
		}
	}
}
