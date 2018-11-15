using AABC.Data.V2;
using ATrack.Authenticator;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Helpers;

namespace AABC.ATrack.Authenticator.ProviderApp
{
    public class UserProvider : IUserProvider
    {
        private readonly string ProviderPortalConnectionString;
        private readonly CoreContext Context;

        public UserProvider(string providerPortalConnectionString, CoreContext context)
        {
            ProviderPortalConnectionString = providerPortalConnectionString;
            Context = context;
        }

        public UserInfo GetUserInfo(string username, string password)
        {
			var allUsers = Context.ProviderPortalUsers.Where(u => u.UserNumber == username).ToList();

			var user = Context.ProviderPortalUsers.FirstOrDefault(u => u.UserNumber == username && u.HasAppAccess);
			if (user == null)
			{
				// we don't have a user who has access
				return null;
			}

            string hashedPassword = null;
            using (var conn = new SqlConnection(ProviderPortalConnectionString))
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = @"SELECT Password FROM webpages_Membership WHERE UserId = @userId";
                cmd.Parameters.AddWithValue("@userId", user.AspNetUserID);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
	                    hashedPassword = Convert.ToString(reader["Password"]);

                        cmd.Cancel(); //execute before closing the reader
                        reader.Close();
                    }
                }
                conn.Close();
            }
            if (hashedPassword == null || !Crypto.VerifyHashedPassword(hashedPassword, password))
            {
                return null;
            }
            return new UserInfo
            {
                UserId = user.AspNetUserID.ToString(),
                UserName = username
            };
        }

        class UserData
        {
            public string UserId { get; set; }
            public string Password { get; set; }
        }

    }
}
