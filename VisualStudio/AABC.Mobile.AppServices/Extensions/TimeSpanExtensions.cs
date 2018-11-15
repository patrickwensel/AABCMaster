using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Extensions
{
	public static class TimeSpanExtensions
	{
		public static TimeSpan RoundUpTo(this TimeSpan timeSpan, int n)
		{
			return TimeSpan.FromMinutes(n * Math.Ceiling(timeSpan.TotalMinutes / n));
		}
	}
}
