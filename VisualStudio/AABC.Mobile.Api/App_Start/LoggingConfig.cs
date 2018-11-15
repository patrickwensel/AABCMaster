using AABC.Mobile.Api.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.ExceptionHandling;

namespace AABC.Mobile.Api.App_Start
{
	/// <summary>
	/// Class LoggingConfig.
	/// </summary>
	public class LoggingConfig
	{
		/// <summary>
		/// Registers the handlers.
		/// </summary>
		public static void RegisterHandlers(ServicesContainer servicesContainer)
		{
			servicesContainer.Replace(typeof(IExceptionLogger), new GlobalExceptionLogger());
			servicesContainer.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());
		}
	}
}