using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Interfaces
{
	/// <summary>
	/// Interface IAccountService
	/// </summary>
	public interface IAccountService
	{
		/// <summary>
		/// Gets the authentication header.
		/// </summary>
		/// <returns>Task&lt;AuthenticationHeaderValue&gt;.</returns>
		Task<AuthenticationHeaderValue> GetAuthenticationHeader();

		/// <summary>
		/// Logins the specified user.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <param name="password">The password.</param>
		/// <returns>Task.</returns>
		Task Login(string userName, string password);

		/// <summary>
		/// Sets the cached credentials.
		/// </summary>
		/// <param name="username">The username.</param>
		/// <param name="password">The password.</param>
		void SetCachedCredentials(string username, string password);


		/// <summary>
		/// Logouts this user.
		/// </summary>
		void Logout();
	}
}
