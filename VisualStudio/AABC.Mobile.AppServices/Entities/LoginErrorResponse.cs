using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Entities
{
	/// <summary>
	/// Class LoginErrorResponse.
	/// </summary>
	public class LoginErrorResponse
	{
		/// <summary>
		/// Gets or sets the error.
		/// </summary>
		/// <value>The error.</value>
		[JsonProperty("error")]
		public string Error { get; set; }

		/// <summary>
		/// Gets or sets the error description.
		/// </summary>
		/// <value>The error description.</value>
		[JsonProperty("error_description")]
		public string ErrorDescription { get; set; }

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>A <see cref="System.String" /> that represents this instance.</returns>
		public override string ToString()
		{
			return ErrorDescription ?? Error ?? "Unknown error";
		}
	}
}
