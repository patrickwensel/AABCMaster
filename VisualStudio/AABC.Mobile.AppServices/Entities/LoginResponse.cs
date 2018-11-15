using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Entities
{
	public class LoginResponse
	{
		/// <summary>
		/// Gets or sets the access token.
		/// </summary>
		/// <value>The access token.</value>
		[JsonProperty("access_token")]
		public string AccessToken { get; set; }

		/// <summary>
		/// Gets or sets the type of the token.
		/// </summary>
		/// <value>The type of the token.</value>
		[JsonProperty("token_type")]
		public string TokenType { get; set; }

		/// <summary>
		/// Gets or sets the expires in seconds.
		/// </summary>
		/// <value>The expires in seconds.</value>
		[JsonProperty("expires_in")]
		public int? ExpiresInSeconds { get; set; }
	}
}
