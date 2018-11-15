using System.Net;
using System.Web.Http.ExceptionHandling;

namespace AABC.Mobile.Api.Handlers
{
	/// <summary>
	/// Class GlobalExceptionHandler.
	/// </summary>
	/// <remarks>
	///	For more information on Exception handling and logging in WebAPI please look 
	/// <see href="https://www.asp.net/web-api/overview/error-handling/web-api-global-error-handling">at this document</see>
	/// </remarks>
	/// <seealso cref="System.Web.Http.ExceptionHandling.ExceptionHandler" />
	public class GlobalExceptionHandler : ExceptionHandler
	{

#if !DEBUG

		/// <summary>
		/// When overridden in a derived class, handles the exception synchronously.
		/// </summary>
		/// <param name="context">The exception handler context.</param>
		public override void Handle(ExceptionHandlerContext context)
		{
			// don't log (we will have already logged in the already with the GlobalExceptionLogger);
			context.Result = new ErrorResultMessage(context.Request, HttpStatusCode.InternalServerError, context.Exception.Message);
		}

#endif

	}
}