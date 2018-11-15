using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using AABC.Mobile.Api.Handlers;

using log4net;

namespace AABC.Mobile.Api.Filters
{
	/// <summary>
	/// Class LogFilterAttribute.
	/// </summary>
	/// <seealso cref="System.Web.Http.Filters.ActionFilterAttribute" />
	public class LogFilterAttribute : ActionFilterAttribute
	{
		/// <summary>
		/// The log timer key
		/// </summary>
		const string _logTimerKey = "LogTimer";

		// create a log interface for this class
		/// <summary>
		/// The log
		/// </summary>
		private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		/// <summary>
		/// Occurs before the action method is invoked.
		/// </summary>
		/// <param name="actionContext">The action context.</param>
		public override void OnActionExecuting(HttpActionContext actionContext)
		{
			var timer = Stopwatch.StartNew();
			actionContext.Request.Properties[_logTimerKey] = timer;
		}


		/// <summary>
		/// Occurs after the action method is invoked.
		/// </summary>
		/// <param name="actionExecutedContext">The action executed context.</param>
		public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
		{
			if (actionExecutedContext.Response != null)
			{
				var timer = ((Stopwatch)actionExecutedContext.Request.Properties[_logTimerKey]);
				timer.Stop();

				var message = GetMessage(actionExecutedContext, timer.ElapsedMilliseconds);

				Log.Info(message);
			}
		}

		/// <summary>
		/// Gets the message.
		/// </summary>
		/// <param name="actionExecutedContext">The action executed context.</param>
		/// <param name="elapsedMilliseconds">The elapsed milliseconds.</param>
		/// <returns>System.String.</returns>
		private string GetMessage(HttpActionExecutedContext actionExecutedContext, long elapsedMilliseconds)
		{
			return new LogMessage
			{
				StatusCode = (int)actionExecutedContext.Response.StatusCode,
				RequestMethod = actionExecutedContext.Request.Method.Method,
				RequestUri = actionExecutedContext.Request.RequestUri.OriginalString,
				Message = actionExecutedContext.Response.StatusCode.ToString(),
				ElapsedMilliseconds = elapsedMilliseconds
			}.ToString();
		}
	}
}