using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace AABC.Mobile.Api.Handlers
{
	/// <summary>
	/// Class ErrorResultMessage.
	/// </summary>
	/// <seealso cref="IHttpActionResult" />
	/// <seealso cref="System.Web.Http.IHttpActionResult" />
	public class ErrorResultMessage : IHttpActionResult
	{
		/// <summary>
		/// The error message
		/// </summary>
		readonly string _errorMessage;

		/// <summary>
		/// The request message
		/// </summary>
		readonly HttpRequestMessage _requestMessage;

		/// <summary>
		/// The status code
		/// </summary>
		readonly HttpStatusCode _statusCode;

		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorResultMessage" /> class.
		/// </summary>
		/// <param name="requestMessage">The request message.</param>
		/// <param name="statusCode">The status code.</param>
		/// <param name="errorMessage">The error message.</param>
		public ErrorResultMessage(HttpRequestMessage requestMessage, HttpStatusCode statusCode, string errorMessage)
		{
			_requestMessage = requestMessage;
			_statusCode = statusCode;
			_errorMessage = errorMessage;
		}

		/// <summary>
		/// Creates an <see cref="T:System.Net.Http.HttpResponseMessage" /> asynchronously.
		/// </summary>
		/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
		/// <returns>A task that, when completed, contains the <see cref="T:System.Net.Http.HttpResponseMessage" />.</returns>
		public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
		{
			return Task.FromResult(_requestMessage.CreateErrorResponse(_statusCode, _errorMessage));
		}
	}
}