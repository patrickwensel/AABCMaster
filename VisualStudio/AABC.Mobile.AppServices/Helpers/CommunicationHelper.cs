using ModernHttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Helpers
{
	/// <summary>
	/// Class CommunicationHelper.
	/// </summary>
	/// <typeparam name="ResponseType">The type of the response type.</typeparam>
	/// <typeparam name="ErrorType">The type of the error type.</typeparam>
	public class CommunicationHelper<ResponseType, ErrorType>
						where ResponseType : new()
						where ErrorType : new()
	{

		/// <summary>
		/// Gets or sets the status code.
		/// </summary>
		/// <value>The status code.</value>
		public HttpStatusCode StatusCode { get; internal set; }

		/// <summary>
		/// Gets or sets the error response.
		/// </summary>
		/// <value>The error response.</value>
		public ErrorType ErrorResponse { get; internal set; }

		/// <summary>
		/// Gets or sets the timeout.
		/// </summary>
		/// <value>The timeout.</value>
		public TimeSpan? Timeout { get; set; }


		/// <summary>
		/// Posts the request.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="postValues">The post values.</param>
		/// <param name="authorisationHeader">The authorisation token.</param>
		/// <returns>Task&lt;ResponseType&gt;.</returns>
		public Task<ResponseType> PostRequest(Uri url, Dictionary<string, string> postValues, AuthenticationHeaderValue authorisationHeader = null)
		{
			return SendRequest(HttpMethod.Post, url, authorisationHeader, postValues == null
																			? null
																			: new FormUrlEncodedContent(postValues));
		}

		/// <summary>
		/// Posts the request.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="body">The body.</param>
		/// <param name="authorisationHeader">The authorisation token.</param>
		/// <returns>Task&lt;ResponseType&gt;.</returns>
		public Task<ResponseType> PostRequest(Uri url, string body, AuthenticationHeaderValue authorisationHeader = null)
		{
			return SendRequest(HttpMethod.Post, url, authorisationHeader, body == null
																			? null
																			: new StringContent(body, Encoding.UTF8, "application/json"));
		}

		/// <summary>
		/// Puts the request.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="body">The body.</param>
		/// <param name="authorisationHeader">The authorisation token.</param>
		/// <returns>Task&lt;ResponseType&gt;.</returns>
		public Task<ResponseType> PutRequest(Uri url, string body, AuthenticationHeaderValue authorisationHeader = null)
		{
			return SendRequest(HttpMethod.Put, url, authorisationHeader, body == null
																			? null
																			: new StringContent(body, Encoding.UTF8, "application/json"));
		}

		/// <summary>
		/// Deletes the request.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="body">The body.</param>
		/// <param name="authorisationHeader">The authorisation token.</param>
		/// <returns>Task&lt;ResponseType&gt;.</returns>
		public Task<ResponseType> DeleteRequest(Uri url, string body, AuthenticationHeaderValue authorisationHeader = null)
		{
			return SendRequest(HttpMethod.Delete, url, authorisationHeader, body == null
																			? null
																			: new StringContent(body, Encoding.UTF8, "application/json"));
		}


		/// <summary>
		/// Gets the request.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="authorisationHeader">The authorisation token.</param>
		/// <returns>Task&lt;ResponseType&gt;.</returns>
		public Task<ResponseType> GetRequest(Uri url, AuthenticationHeaderValue authorisationHeader = null)
		{
			return SendRequest(HttpMethod.Get, url, authorisationHeader, null);
		}


		/// <summary>
		/// Sends the request.
		/// </summary>
		/// <param name="verb">The verb.</param>
		/// <param name="url">The URL.</param>
		/// <param name="authorisationHeader">The authorisation token.</param>
		/// <param name="content">The content.</param>
		/// <returns>Task&lt;ResponseType&gt;.</returns>
		async Task<ResponseType> SendRequest(HttpMethod verb, Uri url, AuthenticationHeaderValue authorisationHeader, HttpContent content)
		{
			ErrorResponse = default(ErrorType);

			string responseString;
			using (var client = new HttpClient(new NativeMessageHandler() { Timeout = Timeout }))
			{
				// set the timeout if required
				if (Timeout.HasValue)
				{
					client.Timeout = Timeout.Value;
				}

				var request = new HttpRequestMessage()
				{
					Method = verb,
					RequestUri = url,
					Content = content
				};

				if (authorisationHeader != null)
				{
					request.Headers.Authorization = authorisationHeader;
				}

				using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
				{
					ResponseType returnValue;

					if (response.IsSuccessStatusCode)
					{
						responseString = await response.Content.ReadAsStringAsync();

						if (String.IsNullOrWhiteSpace(responseString))
						{
							// nothing returned, so return a default ResponseType and set the status code
							returnValue = default(ResponseType);
						}
						else
						{
							// set the required values
							returnValue = JsonConvert.DeserializeObject<ResponseType>(responseString);
						}
					}
					else
					{
						responseString = await response.Content.ReadAsStringAsync();
						try
						{
							ErrorResponse = JsonConvert.DeserializeObject<ErrorType>(responseString);
							returnValue = default(ResponseType);
						}
						catch (JsonException ex)
						{
							throw new InvalidOperationException("Unable to parse: " + responseString, ex);
						}
					}

					// set the status code
					StatusCode = response.StatusCode;

					return returnValue;
				}
			}
		}
	}
}
