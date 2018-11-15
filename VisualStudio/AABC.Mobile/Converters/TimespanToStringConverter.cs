using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AABC.Mobile.Converters
{
	/// <summary>
	/// Class MinutesToStringConverter.
	/// </summary>
	/// <seealso cref="Xamarin.Forms.IValueConverter" />
	class TimeSpanToStringConverter : IValueConverter
	{
		/// <summary>
		/// Implement this method to convert <paramref name="value" /> to <paramref name="targetType" /> by using <paramref name="parameter" /> and <paramref name="culture" />.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="parameter">The parameter.</param>
		/// <param name="culture">The culture.</param>
		/// <returns>To be added.</returns>
		/// <remarks>To be added.</remarks>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ConvertMinutesToString((TimeSpan)value);
		}

		/// <summary>
		/// Converts the minutes to string.
		/// </summary>
		/// <param name="timespan">The timespan.</param>
		/// <returns>System.String.</returns>
		public static string ConvertMinutesToString(TimeSpan timespan)
		{
			StringBuilder builder = new StringBuilder();
			string separator = String.Empty;
			if (timespan.Days > 0)
			{
				builder.Append(separator).Append(timespan.Days).Append("d");
				separator = " ";
			}
			if (timespan.Hours > 0)
			{
				builder.Append(separator).Append(timespan.Hours).Append("h");
				separator = " ";
			}
			if (timespan.Minutes >= 0)
			{
				builder.Append(separator).Append(timespan.Minutes).Append("m");
				separator = " ";
			}

			return builder.ToString();
		}

		/// <summary>
		/// Implement this method to convert <paramref name="value" /> back from <paramref name="targetType" /> by using <paramref name="parameter" /> and <paramref name="culture" />.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="parameter">The parameter.</param>
		/// <param name="culture">The culture.</param>
		/// <returns>To be added.</returns>
		/// <exception cref="System.InvalidOperationException"></exception>
		/// <remarks>To be added.</remarks>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new InvalidOperationException();
		}
	}
}
