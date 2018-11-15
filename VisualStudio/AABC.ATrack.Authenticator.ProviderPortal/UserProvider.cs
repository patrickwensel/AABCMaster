using ATrack.Authenticator;
using System;
using System.Data.SqlClient;
using System.Web.Helpers;

namespace AABC.ATrack.Authenticator.ProviderPortal
{
    public class UserProvider : IUserProvider
    {
        private readonly string ProviderPortalConnectionString;

        public UserProvider(string providerPortalConnectionString)
        {
            ProviderPortalConnectionString = providerPortalConnectionString;
        }

        public virtual UserInfo GetUserInfo(string username, string password)
        {
            UserData r = null;
            using (var conn = new SqlConnection(ProviderPortalConnectionString))
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = @"SELECT TOP 1 UP.UserId, M.Password
                                    FROM UserProfile AS UP INNER JOIN webpages_Membership AS M ON M.UserId = UP.UserId
                                    WHERE UP.UserName = @userName";
                cmd.Parameters.AddWithValue("@userName", username);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        r = new UserData
                        {
                            UserId = Convert.ToString(reader["UserId"]),
                            Password = Convert.ToString(reader["Password"])
                        };
                        cmd.Cancel(); //execute before closing the reader
                        reader.Close();
                    }
                }
                conn.Close();
            }
            if (r == null || !Crypto.VerifyHashedPassword(r.Password, password))
            {
                return null;
            }
            return new UserInfo
            {
                UserId = r.UserId,
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
