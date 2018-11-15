using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using WebMatrix.WebData;
using System.Data;
using System.Data.SqlClient;

namespace AABC.Mobile.Api.Providers
{
	public class CustomOAuthProvider : OAuthAuthorizationServerProvider
	{

        bool _demoMode;

        public CustomOAuthProvider(bool demoMode) {
            _demoMode = demoMode;
        }

		public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
		{
			context.Validated();
			return Task.FromResult<object>(null);
		}

		public override Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
		{
			var allowedOrigin = "*";

			context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

            // make sure the user is allowed to use the app
            if (!userAllowedAppAccess(context.UserName)) {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return Task.CompletedTask;
            }

			// make sure this username and password are correct
			if (WebSecurity.Login(context.UserName, context.Password))
			{
				var roles = System.Web.Security.Roles.GetRolesForUser(context.UserName);

				var userId = WebSecurity.GetUserId(context.UserName);

				var claims = new List<Claim>();
				claims.Add(new Claim(ClaimTypes.Sid, userId.ToString()));
				claims.Add(new Claim(ClaimTypes.Name, context.UserName));

				// add other claims here
				// claims.Add(new Claim(ClaimTypes.Email, context.UserName));
				// claims.Add(new Claim(ClaimTypes.Role, "MyRole"));
				ClaimsIdentity oAuthIdentity = new ClaimsIdentity(claims, "JWT");

				var ticket = new AuthenticationTicket(oAuthIdentity, null);

				context.Validated(ticket);
			}
			else
			{
				context.SetError("invalid_grant", "The user name or password is incorrect.");
			}

			return Task.CompletedTask;
		}




        private bool userAllowedAppAccess(string userName) {

            if (_demoMode) {
                return true;
            }

            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;
                cmd.CommandText = "SELECT ProviderHasAppAccess FROM dbo.ProviderPortalUsers WHERE ProviderUserNumber = @Username;";
                cmd.Parameters.AddWithValue("@Username", userName);

                conn.Open();
                var result = cmd.ExecuteScalar();
                conn.Close();

                if (result == null) {
                    return false;
                }

                bool allowed = (bool)result;

                return allowed;
            }
            
        }
	}
}