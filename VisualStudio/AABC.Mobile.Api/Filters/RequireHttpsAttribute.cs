using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace AABC.Mobile.Api.Filters
{
	/// <summary>
	/// Class RequireHttpsAttribute.
	/// </summary>
	/// <seealso cref="System.Web.Http.Filters.AuthorizationFilterAttribute" />
	public class RequireHttpsAttribute : AuthorizationFilterAttribute
	{
		/// <summary>
		/// Gets or sets the port.
		/// </summary>
		/// <value>The port.</value>
		public int Port { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="RequireHttpsAttribute"/> class.
		/// </summary>
		public RequireHttpsAttribute()
		{
			Port = 443;
		}

		/// <summary>
		/// Calls when a process requests authorization.
		/// </summary>
		/// <param name="actionContext">The action context, which encapsulates information for using <see cref="T:System.Web.Http.Filters.AuthorizationFilterAttribute" />.</param>
		public override void OnAuthorization(HttpActionContext actionContext)
		{
			var request = actionContext.Request;

			if (request.RequestUri.Scheme != Uri.UriSchemeHttps)
			{
				var response = new HttpResponseMessage();

				if (request.Method == HttpMethod.Get || request.Method == HttpMethod.Head)
				{
					var uri = new UriBuilder(request.RequestUri);
					uri.Scheme = Uri.UriSchemeHttps;
					uri.Port = this.Port;

					response.StatusCode = HttpStatusCode.Found;
					response.Headers.Location = uri.Uri;
				}
				else
				{
					response.StatusCode = HttpStatusCode.Forbidden;
				}

				actionContext.Response = response;
			}
			else
			{
				base.OnAuthorization(actionContext);
			}
		}
	}
}