using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AABC.Mobile.Api.Handlers
{
	/// <summary>
	/// Class LogMessage.
	/// </summary>
	public class LogMessage
	{
		/// <summary>
		/// Gets or sets the status code.
		/// </summary>
		/// <value>The status code.</value>
		public int StatusCode { get; set; }

		/// <summary>
		/// Gets or sets the request method.
		/// </summary>
		/// <value>The request method.</value>
		public string RequestMethod { get; set; }

		/// <summary>
		/// Gets or sets the request URI.
		/// </summary>
		/// <value>The request URI.</value>
		public string RequestUri { get; set; }

		/// <summary>
		/// Gets or sets the elapsed milliseconds.
		/// </summary>
		/// <value>The elapsed milliseconds.</value>
		public long ElapsedMilliseconds { get; set; }

		/// <summary>
		/// Gets or sets the message that is output with the log.
		/// </summary>
		/// <value>The message.</value>
		public string Message { get; set; }

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
		public override string ToString()
		{
			return string.Format("{0} {1} {2} {3} {4}",
						StatusCode,
						RequestMethod,
						RequestUri,
						ElapsedMilliseconds,
						Message);
		}
	}
}