using Dymeng.Framework;
using Dymeng.Framework.Data.SqlClient;
using System;
using System.Data;
using System.Data.SqlClient;


namespace AABC.Web.Models.OfficeStaff
{
    public class OfficeStaffHelpers
    {

        public static string GetStaffCommonName(int staffID) {

            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;
                cmd.CommandText = "SELECT StaffFirstName + ' ' + StaffLastName AS CommonName FROM dbo.Staff WHERE ID = @ID;";
                cmd.Parameters.AddWithValue("@ID", staffID);

                try {

                    DataRow row = cmd.GetRow();
                    if (row == null) {
                        return null;
                    } else {
                        return row.ToStringValue("CommonName");
                    }

                } catch (Exception e) {
                    Exceptions.Handle(e);
                    return null;
                }

            }

        }

        internal static void DeleteStaff(int staffID) {

            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;
                cmd.CommandText = "DELETE FROM dbo.Staff WHERE ID = @ID;";
                cmd.Parameters.AddWithValue("@ID", staffID);

                try {
                    cmd.ExecuteNonQueryToInt();
                }
                catch (Exception e) {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }

            }

        }
    }
}