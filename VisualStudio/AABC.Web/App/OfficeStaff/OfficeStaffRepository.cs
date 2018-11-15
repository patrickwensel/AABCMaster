using Dymeng.Framework;
using Dymeng.Framework.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AABC.Web.Repositories
{

    public interface IOfficeStaffRepository
    {
        bool SaveStaff(Domain.OfficeStaff.OfficeStaff staff);
        Domain.OfficeStaff.OfficeStaff GetOfficeStaff(int id);
        List<Domain.OfficeStaff.OfficeStaff> GetOfficeStaffList();
    }

    public class OfficeStaffRepository : IOfficeStaffRepository
    {


        string connectionString = null;

        public OfficeStaffRepository() {
            this.connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString;
        }

        public OfficeStaffRepository(string connectionString) {
            this.connectionString = connectionString;
        }





        public List<Domain.OfficeStaff.OfficeStaff> GetOfficeStaffList() {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;

                cmd.CommandText = "SELECT ID, DateCreated, StaffActive, StaffFirstName, StaffLastName, " +
                    "StaffPrimaryPhone, StaffPrimaryEmail, StaffHireDate, StaffTerminatedDate " +
                    "FROM dbo.Staff;";

                try {

                    DataTable table = cmd.GetTable();

                    List<Domain.OfficeStaff.OfficeStaff> staffs = new List<Domain.OfficeStaff.OfficeStaff>();

                    foreach (DataRow r in table.Rows) {

                        Domain.OfficeStaff.OfficeStaff staff = new Domain.OfficeStaff.OfficeStaff();

                        staff.ID = r.ToInt("ID");
                        staff.DateCreated = r.ToDateTime("DateCreated");
                        staff.Active = r.ToBool("StaffActive");
                        staff.FirstName = r.ToStringValue("StaffFirstName");
                        staff.LastName = r.ToStringValue("StaffLastName");
                        staff.Phone = r.ToStringValue("StaffPrimaryPhone");
                        staff.Email = r.ToStringValue("StaffPrimaryEmail");
                        staff.HireDate = r.ToDateTimeOrNull("StaffHireDate");
                        staff.TerminationDate = r.ToDateTimeOrNull("StaffTerminatedDate");

                        staffs.Add(staff);

                    }

                    return staffs;


                }
                catch (Exception e) {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }
            }


        }


        public Domain.OfficeStaff.OfficeStaff GetOfficeStaff(int id) {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;

                cmd.CommandText =
                    "SELECT ID, StaffActive, StaffFirstName, StaffLastName, StaffPrimaryPhone, StaffPrimaryEmail, StaffHireDate, StaffTerminatedDate " +
                    "FROM dbo.Staff " +
                    "WHERE ID = @ID;";

                cmd.Parameters.AddWithValue("@ID", id);

                try {

                    DataTable table = cmd.GetTable();

                    if (table.Rows.Count != 1) {
                        throw new DataException("Item not found");
                    }

                    DataRow r = table.Rows[0];

                    Domain.OfficeStaff.OfficeStaff staff = new Domain.OfficeStaff.OfficeStaff();

                    staff.ID = r.ToInt("ID");
                    staff.Active = r.ToBool("StaffActive");
                    staff.FirstName = r.ToStringValue("StaffFirstName");
                    staff.LastName = r.ToStringValue("StaffLastName");
                    staff.Phone = r.ToStringValue("StaffPrimaryPhone");
                    staff.Email = r.ToStringValue("StaffPrimaryEmail");
                    staff.HireDate = r.ToDateTimeOrNull("StaffHireDate");
                    staff.TerminationDate = r.ToDateTimeOrNull("StaffTerminatedDate");

                    return staff;

                }
                catch (Exception e) {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }
            }

        }


        public bool SaveStaff(Domain.OfficeStaff.OfficeStaff staff) {

            if (staff.ID.HasValue) {
                return update(staff);
            } else {
                return saveNew(staff);
            }

        }


        private bool update(Domain.OfficeStaff.OfficeStaff staff) {


            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;

                cmd.CommandText =
                    "UPDATE dbo.Staff SET " +
                    "StaffActive = @Active, StaffFirstName = @FirstName, StaffLastName = @LastName, " +
                    "StaffPrimaryPhone = @Phone, StaffPrimaryEmail = @Email, StaffHireDate = @HireDate, StaffTerminatedDate = @TerminationDate " +
                    "WHERE ID = @ID;";

                cmd.Parameters.AddWithValue("@Active", staff.Active);
                cmd.Parameters.AddWithValue("@FirstName", staff.FirstName);
                cmd.Parameters.AddWithValue("@LastName", staff.LastName);
                cmd.Parameters.AddWithNullableValue("@Phone", staff.Phone);
                cmd.Parameters.AddWithNullableValue("@Email", staff.Email);
                cmd.Parameters.AddWithNullableValue("@HireDate", staff.HireDate);
                cmd.Parameters.AddWithNullableValue("@TerminationDate", staff.TerminationDate);

                cmd.Parameters.AddWithValue("@ID", staff.ID.Value);


                try {

                    int recsAffected = cmd.ExecuteNonQueryToInt();

                    if (recsAffected == 0) {
                        return false;
                    } else {
                        return true;
                    }

                }
                catch (Exception e) {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }

            }

        }

        private bool saveNew(Domain.OfficeStaff.OfficeStaff staff) {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;

                cmd.CommandText =
                    "INSERT INTO dbo.Staff (" +
                    "StaffActive, StaffFirstName, StaffLastName, StaffPrimaryPhone, " +
                    "StaffPrimaryEmail, StaffHireDate, StaffTerminatedDate" +
                    ") VALUES (" +
                    "@Active, @FirstName, @LastName, @Phone, @Email, @HireDate, @TerminationDate);";


                cmd.Parameters.AddWithValue("@Active", staff.Active);
                cmd.Parameters.AddWithValue("@FirstName", staff.FirstName);
                cmd.Parameters.AddWithValue("@LastName", staff.LastName);
                cmd.Parameters.AddWithNullableValue("@Phone", staff.Phone);
                cmd.Parameters.AddWithNullableValue("@Email", staff.Email);
                cmd.Parameters.AddWithNullableValue("@HireDate", staff.HireDate);
                cmd.Parameters.AddWithNullableValue("@TerminationDate", staff.TerminationDate);

                try {

                    int? id = cmd.InsertToIdentityOrNull();

                    if (id.HasValue) {
                        staff.ID = id;
                        return true;
                    } else {
                        return false;
                    }

                }
                catch (Exception e) {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }

            }
        }
    }
}