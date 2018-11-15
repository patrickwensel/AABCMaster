using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ExceptionHandling;
using System.Diagnostics;
using System.Net;

using log4net;

namespace AABC.Mobile.Api.Handlers
{
#pragma warning disable CS1584
	/// <summary>
	/// Class GlobalExceptionLogger.
	/// </summary>
	/// <seealso cref="System.Web.Http.ExceptionHandling.ExceptionLogger" />
	/// <remarks>For more information on Exception handling and logging in WebAPI please look at this document
	/// <see cref="https://www.asp.net/web-api/overview/error-handling/web-api-global-error-handling" /></remarks>
#pragma warning restore CS1584
	public class GlobalExceptionLogger : ExceptionLogger
	{
		/// <summary>
		/// The log timer key
		/// </summary>
		const string _logTimerKey = "LogTimer";

		// create a log interface for this class
		/// <summary>
		/// The logger
		/// </summary>
		private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// When overridden in a derived class, logs the exception synchronously.
		/// </summary>
		/// <param name="context">The exception logger context.</param>
		public override void Log(ExceptionLoggerContext context)
		{
			// get the timer and stop it if there is one
			object obTimerObject;
			var timer = context.Request.Properties.TryGetValue(_logTimerKey, out obTimerObject) ? (Stopwatch)obTimerObject : null;
			timer?.Stop();

			// create the message and log it
			var logMessage = new LogMessage
			{
				StatusCode = (int)HttpStatusCode.InternalServerError,
				RequestMethod = context.Request.Method?.Method,
				RequestUri = context.Request.RequestUri?.OriginalString,
				ElapsedMilliseconds = timer?.ElapsedMilliseconds ?? 0L,
				Message = context.Exception.ToString()
			};

			Logger.Error(logMessage.ToString());
		}
	}
}