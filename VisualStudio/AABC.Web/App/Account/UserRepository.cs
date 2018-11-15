using AABC.Domain.Admin;
using AABC.Web.App.Account.Models;
using AABC.Web.Repositories;
using Dymeng.Framework;
using Dymeng.Framework.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;



namespace AABC.Web.App.Account
{

    public interface IUserRepository
    {

        // USERS
        UserVM GetUser(int id);
        UserVM GetUser(string userName);
        UserVM GetUser();

        UserListVM GetUserListItems();
        List<PermissionListItemVM> GetUserPermissionList(int? id);
        List<OptionListItemVM> GetUserOptionList(int? id);

        void DeleteUser(int id);
        void SaveUser(UserVM user);
        void InsertPermissionsList(int? id);
        void InsertOptionsList(int? id);
        void UserPermissionIsAllowedUpdate(int permissionId, bool isAllowed);

        List<Domain.OfficeStaff.OfficeStaff> GetOfficeStaffList();
        Domain.OfficeStaff.OfficeStaff GetOfficeStaffMember(int id);
        List<Domain.OfficeStaff.OfficeStaff> GetOfficeStaffUsers();
    }

    public class UserRepository : IUserRepository
    {

        public void UserPermissionIsAllowedUpdate(int permissionId, bool isAllowed)
        {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;

                cmd.CommandText = "UPDATE dbo.WebUserPermissions SET isAllowed = @isAllowed WHERE ID = @wupID;";

                cmd.Parameters.AddWithValue("@wupID", permissionId);
                cmd.Parameters.AddWithValue("@isAllowed", isAllowed);

                cmd.ExecuteNonQueryToInt();
            }
        }

        public List<Domain.OfficeStaff.OfficeStaff> GetOfficeStaffList()
        {
            return new OfficeStaffRepository().GetOfficeStaffList();
        }

        public Domain.OfficeStaff.OfficeStaff GetOfficeStaffMember(int id)
        {
            return new OfficeStaffRepository().GetOfficeStaff(id);
        }

        public UserVM GetUser(int id)
        {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;

                string sql = "";
                sql += "SELECT ";
                sql += "  ID, DateCreated, UserName, WebUserFirstName, WebUserLastName, WebUserEmail, StaffID ";
                sql += "FROM dbo.WebUsers ";
                sql += "WHERE ID = @ID;";

                cmd.CommandText = sql;

                cmd.Parameters.AddWithValue("@ID", id);

                try
                {

                    var r = cmd.GetRow();
                    var m = new UserVM
                    {
                        ID = r.ToInt("ID"),
                        DateCreated = r.ToDateTime("DateCreated"),
                        UserName = r.ToStringValue("UserName"),
                        FirstName = r.ToStringValue("WebUserFirstName"),
                        LastName = r.ToStringValue("WebUserLastName"),
                        Email = r.ToStringValue("WebUserEmail"),
                        StaffMemberID = r.ToIntOrNull("StaffID")
                    };
                    return m;

                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }
            }
        }

        public UserVM GetUser(string userName)
        {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;

                string sql = "";
                sql += "SELECT ";
                sql += "  ID, DateCreated, UserName, WebUserFirstName, WebUserLastName, WebUserEmail ";
                sql += "FROM dbo.WebUsers ";
                sql += "WHERE userName = @UserName;";

                cmd.CommandText = sql;

                cmd.Parameters.AddWithValue("@UserName", userName);

                try
                {
                    var r = cmd.GetRow();
                    var m = new UserVM
                    {
                        ID = r.ToInt("ID"),
                        DateCreated = r.ToDateTime("DateCreated"),
                        UserName = r.ToStringValue("UserName"),
                        FirstName = r.ToStringValue("WebUserFirstName"),
                        LastName = r.ToStringValue("WebUserLastName"),
                        Email = r.ToStringValue("WebUserEmail")
                    };

                    return m;
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }
            }
        }

        public UserVM GetUser()
        {
            return new UserVM();
        }

        public void DeleteUser(int id)
        {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;
                cmd.CommandText = "DELETE FROM dbo.WebUsers WHERE ID = @ID;";
                cmd.Parameters.AddWithValue("@ID", id);

                try
                {

                    cmd.ExecuteNonQueryToInt();

                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }
            }
        }

        public void SaveUser(UserVM user)
        {

            if (user.ID.GetValueOrDefault(0) > 0)
            {
                UpdatedExistingUser(user);
            }
            else
            {
                SaveNewUser(user);
            }
        }

        public void SaveUserEdit(int userID)
        {



        }

        void UpdatedExistingUser(UserVM u)
        {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;

                string sql = "";
                sql += "UPDATE dbo.WebUsers SET ";
                sql += "  StaffID = @StaffID, AspNetUserID = @AspNetUserID, ";
                sql += "  WebUserFirstName = @FirstName, WebUserLastName = @LastName, WebUserEmail = @Email ";
                sql += "WHERE ID = @ID;";

                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@ID", u.ID.Value);
                cmd.Parameters.AddWithValue("@AspNetUserID", u.AspNetUserID);
                cmd.Parameters.AddWithValue("@FirstName", u.FirstName);
                cmd.Parameters.AddWithValue("@LastName", u.LastName);
                cmd.Parameters.AddWithValue("@Email", u.Email);


                if (u.StaffMemberID.GetValueOrDefault(0) > 0)
                {
                    cmd.Parameters.AddWithNullableValue("@StaffID", u.StaffMemberID.Value);
                }
                else
                {
                    cmd.Parameters.AddWithNullableValue("@StaffID", null);
                }

                try
                {

                    cmd.ExecuteNonQueryToInt();

                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }
            }
        }

        void SaveNewUser(UserVM u)
        {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;

                string sql = "";
                sql += "INSERT INTO dbo.WebUsers (";
                sql += "  AspNetUserID, StaffID, UserName, WebUserFirstName, WebUserLastName, WebUserEmail ";
                sql += ") VALUES (";
                sql += "  @AspNetUserID, @StaffID, @Username, @FirstName, @LastName, @Email ";
                sql += ");";

                cmd.CommandText = sql;

                try
                {
                    cmd.Parameters.AddWithValue("@UserName", u.UserName);
                    cmd.Parameters.AddWithValue("@FirstName", u.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", u.LastName);
                    cmd.Parameters.AddWithValue("@Email", u.Email);
                    cmd.Parameters.AddWithValue("@AspNetUserID", u.AspNetUserID);

                    if (u.StaffMemberID.GetValueOrDefault(0) > 0)
                    {
                        cmd.Parameters.AddWithNullableValue("@StaffID", u.StaffMemberID.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithNullableValue("@StaffID", null);
                    }

                    u.ID = cmd.InsertToIdentity();

                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }
            }
        }

        public UserListVM GetUserListItems()
        {

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;

                cmd.CommandText = "SELECT " +
                                  "		ID, " +
                                  "		UserName, " +
                                  "		WebUserFirstName, " +
                                  "		WebUserLastName, " +
                                  "		WebUserEmail, " +
                                  "		StaffID " +
                                  "FROM dbo.WebUsers;";

                try
                {

                    DataSet set = cmd.GetDataSet();

                    DataTable userTable = set.Tables[0];

                    List<UserVM> items = new List<UserVM>();
                    UserListVM userList = new UserListVM();

                    foreach (DataRow r in userTable.Rows)
                    {

                        UserVM item = new UserVM
                        {
                            ID = r.ToIntOrNull("ID"),
                            UserName = r.ToStringValue("UserName"),
                            FirstName = r.ToStringValue("WebUserFirstName"),
                            LastName = r.ToStringValue("WebUserLastName"),
                            Email = r.ToStringValue("WebUserEmail")
                        };

                        if (r.ToIntOrNull("StaffID") != null)
                        {
                            item.StaffMember = this.GetOfficeStaffMember(r.ToInt("StaffID"));
                        }

                        items.Add(item);

                    }

                    userList.DetailList = items;

                    return userList;
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }
            }
        }

        public List<OptionListItemVM> GetUserOptionList(int? userID)
        {

            if (userID == null)
            {
                return new List<OptionListItemVM>();
            }

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;

                cmd.CommandText = "SELECT " +
                                  "			WebUsers.ID, " +
                                  "			WebOptions.WebOptionName, " +
                                  "			WebOptions.WebOptionDescription, " +
                                  "			WebUserOptions.WebOptionValue, " +
                                  "			WebUserOptions.isAllowed " +
                                  "FROM		WebUsers INNER JOIN " +
                                  "			WebUserOptions ON WebUsers.ID = WebUserOptions.WebUserID INNER JOIN " +
                                  "			WebOptions ON WebUserOptions.WebOptionID = WebOptions.ID " +
                                  "WHERE	WebUsers.ID = @UserID;";
                cmd.Parameters.AddWithValue("@UserID", userID);

                try
                {

                    DataTable table = cmd.GetTable();

                    List<OptionListItemVM> options = new List<OptionListItemVM>();

                    foreach (DataRow r in table.Rows)
                    {

                        var option = new OptionListItemVM
                        {
                            OptionName = r.ToStringValue("WebOptionName"),
                            Description = r.ToStringValue("WebOptionDescription"),
                            OptionValue = r.ToStringValue("WebOptionValue"),
                            isAllowed = r.ToBool("isAllowed")
                        };

                        options.Add(option);

                    }

                    return options;
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }
            }

        }

        public List<PermissionListItemVM> GetUserPermissionList(int? userID)
        {

            if (userID == null)
            {
                return new List<PermissionListItemVM>();
            }

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;

                cmd.CommandText = "SELECT " +
                                  "			WebUsers.ID, " +
                                  "         WebUserPermissions.ID AS WebUserPermissionID, " +
                                  "			WebPermissions.WebPermissionName, " +
                                  "			WebPermissions.WebPermissionDescription, " +
                                  "			WebUserPermissions.isAllowed " +
                                  "FROM		WebUsers INNER JOIN " +
                                  "			WebUserPermissions ON WebUsers.ID = WebUserPermissions.WebUserID INNER JOIN " +
                                  "			WebPermissions ON WebUserPermissions.WebPermissionID = WebPermissions.ID " +
                                  "WHERE	WebUsers.ID = @UserID;";
                cmd.Parameters.AddWithValue("@UserID", userID);

                try
                {

                    DataTable table = cmd.GetTable();

                    List<PermissionListItemVM> permissions = new List<PermissionListItemVM>();

                    foreach (DataRow r in table.Rows)
                    {

                        var permission = new PermissionListItemVM
                        {
                            PermissionName = r.ToStringValue("WebPermissionName"),
                            Description = r.ToStringValue("WebPermissionDescription"),
                            isAllowed = r.ToBool("isAllowed"),
                            UserPermissionID = r.ToInt("WebUserPermissionID")
                        };

                        permissions.Add(permission);

                    }

                    return permissions;
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }
            }
        }

        public void InsertPermissionsList(int? id)
        {

            var permissionList = Enum.GetValues(typeof(Permissions)).Cast<Permissions>().ToArray();

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO dbo.WebUserPermissions (WebUserID, WebPermissionID, isAllowed) VALUES (@UserID, @PermissionID, 0); ";
                cmd.Parameters.Add("@UserID", SqlDbType.Int);
                cmd.Parameters.Add("@PermissionID", SqlDbType.Int);

                foreach (int permissionID in permissionList)
                {

                    try
                    {

                        cmd.Parameters["@UserID"].Value = id;
                        cmd.Parameters["@PermissionID"].Value = permissionID;
                        cmd.ExecuteNonQueryToInt();

                    }
                    catch (Exception e)
                    {
                        Exceptions.Handle(e);
                        throw new DataException(e.Message, e);
                    }

                }

            }
        }


        public void InsertOptionsList(int? id)
        {

            var optionList = Enum.GetValues(typeof(Options)).Cast<Options>().ToArray();

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO dbo.WebUserOptions (WebUserID, WebOptionID, isAllowed) VALUES (@UserID, @OptionID, 0); ";
                cmd.Parameters.Add("@UserID", SqlDbType.Int);
                cmd.Parameters.Add("@OptionID", SqlDbType.Int);

                foreach (int optionID in optionList)
                {

                    try
                    {

                        cmd.Parameters["@UserID"].Value = id;
                        cmd.Parameters["@OptionID"].Value = optionID;
                        cmd.ExecuteNonQueryToInt();

                    }
                    catch (Exception e)
                    {
                        Exceptions.Handle(e);
                        throw new DataException(e.Message, e);
                    }

                }

            }

        }

        public List<Domain.OfficeStaff.OfficeStaff> GetOfficeStaffUsers()
        {

            var optionList = Enum.GetValues(typeof(Options)).Cast<Options>().ToArray();

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {

                cmd.Connection = conn;
                cmd.CommandText = "select Max(StaffId) as StaffId, Max(Username) as Username, Email, FirstName, LastName from ( "
                    + "select Id as StaffId, '' as UserName, StaffPrimaryEmail as Email, StaffFirstName as FirstName, StaffLastName as LastName from staff "
                    + "union "
                    + "select StaffId, UserName, WebUserEmail as Email, WebUserFirstName as FirstName, WebUserLastName as LastName from webusers "
                    + ") s "
                    + "group by Email, FirstName, LastName "
                    + "order by StaffId, UserName";

                List<Domain.OfficeStaff.OfficeStaff> r = new List<Domain.OfficeStaff.OfficeStaff>();
                try
                {
                    var dt = cmd.GetTable();
                    foreach (DataRow dr in dt.Rows)
                    {
                        var m = new Domain.OfficeStaff.OfficeStaff
                        {
                            ID = dr.ToIntOrNull("StaffId"),
                            UserName = dr.ToStringValue("UserName"),
                            FirstName = dr.ToStringValue("FirstName"),
                            LastName = dr.ToStringValue("LastName"),
                            Email = dr.ToStringValue("Email")
                        };
                        r.Add(m);
                    }

                    return r;
                }
                catch (Exception e)
                {
                    Exceptions.Handle(e);
                    throw new DataException(e.Message, e);
                }

            }

        }


        // GENERIC/SUPPORTING STUFF

        private readonly string connectionString;

        public UserRepository()
        {
            this.connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString;
        }

        public UserRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

    }
}