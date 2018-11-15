using Dymeng.Framework.Data.SqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AABC.Web.App.PatientPortal
{
    public class PatientPortalExtendedServices
    {


        public static string CreateLogin(string email, string firstName, string lastName, bool active) {


            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand()) {


                cmd.Connection = conn;

                cmd.CommandText = "INSERT INTO dbo.PatientPortalLogins " +
                    "(LoginEmail, LoginFirstName, LoginLastName, LoginPassword, Active) " +
                    "VALUES (@Email, @FirstName, @LastName, @Password, @Active);";

                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@Password", "unused");
                cmd.Parameters.AddWithValue("@Active", active);
                
                int id = cmd.InsertToIdentity();

                cmd.Parameters.Clear();
                string pass = GeneratePassword();
                cmd.CommandText = "INSERT INTO dbo.PatientPortalWebMembership (ID, MemberPassword) VALUES (@ID, @Password);";

                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@Password", pass);

                cmd.ExecuteNonQueryToInt();

                return pass;



            }
            
                    


        }


        public static void UpdateLogin(string email, string firstName, string lastName, bool active)
        {


            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                conn.Open();
                cmd.Connection = conn;

                cmd.CommandText = "Update dbo.PatientPortalLogins " +
                    "set LoginFirstName = @FirstName, LoginLastName = @LastName, Active = @Active where LoginEmail = @Email";

                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@Active", active);

                cmd.ExecuteNonQuery();

                conn.Close();
            }




        }

        static Random random = new Random();

        public static string GeneratePassword() {

            const string chars = "abcdefghjkmnpqrstuvABCDEFGHJKMNPQRSTUVWXYZ23456789";
            return new string(Enumerable.Repeat(chars, 8)
              .Select(s => s[random.Next(s.Length)]).ToArray());

        }
        
    }
}