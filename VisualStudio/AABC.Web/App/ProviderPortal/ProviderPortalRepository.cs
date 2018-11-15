using AABC.Data.V2;
using AABC.Domain.ProviderPortal;
using AABC.Domain.Providers;
using Dymeng.Framework.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace AABC.Web.Repositories
{
    public interface IProviderPortalRepository
    {
        IEnumerable<Models.ProviderPortal.UserItem> GetUserItems();
        Provider GetProvider(int providerID);
        string GenerateProviderNumber(int providerID);
        void RegisterProvider(Models.ProviderPortal.RegisterVM model);
        void UnregisterProvider(int providerID);
    }


    public class ProviderPortalRepository : IProviderPortalRepository
    {
        private readonly CoreContext Context;
        private readonly Data.Services.ProviderPortalService service;
        private readonly string connectionString;


        public ProviderPortalRepository()
        {
            Context = AppService.Current.DataContextV2;
            service = new Data.Services.ProviderPortalService();
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString;
        }


        public void UnregisterProvider(int providerID)
        {

            var removalService = new DomainServices.Providers.RemovalService(this.Context);

            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["ProviderPortalConnection"].ConnectionString;

            removalService.RemoveProviderUser(providerID, connStr);
            
        }


        public void RegisterProvider(Models.ProviderPortal.RegisterVM model)
        {
            var ppu = new ProviderPortalUser();
            var hashedPass = System.Web.Helpers.Crypto.HashPassword(model.Password);
            int aspNetId = 0;
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["ProviderPortalConnection"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO dbo.UserProfile (UserName) VALUES (@UserName);";
                cmd.Parameters.AddWithValue("@UserName", model.ProviderNumber);
                aspNetId = cmd.InsertToIdentity();
                cmd.CommandText = "INSERT INTO dbo.webpages_Membership (" +
                    "UserID, CreateDate, IsConfirmed, PasswordFailuresSinceLastSuccess, Password, PasswordChangedDate, PasswordSalt" +
                    ") VALUES (" +
                    "@UserID, @CreateDate, @IsConfirmed, @PasswordFailuresSinceLastSuccess, @Password, @PasswordChangedDate, @PasswordSalt);";
                cmd.Parameters.AddWithValue("@UserID", aspNetId);
                cmd.Parameters.AddWithValue("@CreateDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@IsConfirmed", 1);
                cmd.Parameters.AddWithValue("@PasswordFailuresSinceLastSuccess", 0);
                cmd.Parameters.AddWithValue("@Password", hashedPass);
                cmd.Parameters.AddWithValue("@PasswordChangedDate", DateTime.Now);
                cmd.Parameters.AddWithValue("@PasswordSalt", "");
                cmd.ExecuteNonQueryToInt();
            }

            ppu.AspNetUserID = aspNetId;
            ppu.ProviderID = model.ProviderID;
            ppu.ProviderUserNumber = model.ProviderNumber;

            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "DELETE FROM dbo.ProviderPortalUsers WHERE AspNetUserID = @AspID; INSERT INTO dbo.ProviderPortalUsers (AspNetUserID, ProviderID, ProviderUserNumber) VALUES (" +
                    "@AspID, @ProviderID, @UserNumber);";
                cmd.Parameters.AddWithValue("@AspID", aspNetId);
                cmd.Parameters.AddWithValue("@ProviderID", model.ProviderID);
                cmd.Parameters.AddWithValue("@UserNumber", model.ProviderNumber);
                cmd.InsertToIdentity();
            }
        }


        public string GenerateProviderNumber(int providerID)
        {
            var providerService = new Data.Services.ProviderService();
            Random rnd = new Random();
            int number = rnd.Next(10000, 19999);
            bool isTaken = providerService.ProviderNumberExists(number.ToString());
            while (isTaken)
            {
                rnd = new Random();
                number = rnd.Next(10000, 19999);
                isTaken = providerService.ProviderNumberExists(number.ToString());
            }
            SaveProviderNumber(providerID, number.ToString());
            return number.ToString();
        }


        private void SaveProviderNumber(int providerID, string number)
        {
            using (SqlConnection conn = new SqlConnection(this.connectionString))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "UPDATE dbo.Providers SET ProviderNumber = @Num WHERE ID = @ID;";
                cmd.Parameters.AddWithValue("@ID", providerID);
                cmd.Parameters.AddWithValue("@Num", number);
                cmd.ExecuteNonQueryToInt();
            }
        }


        public Provider GetProvider(int providerID)
        {
            var providerService = new Data.Services.ProviderService();
            return providerService.GetProvider(providerID);
        }


        public IEnumerable<Models.ProviderPortal.UserItem> GetUserItems()
        {
            var items = Context.Database.SqlQuery<Models.ProviderPortal.UserItem>(@"
                            SELECT        
	                            P.ID, 
	                            P.ProviderFirstName AS FirstName, 
	                            P.ProviderLastName AS LastName, 
	                            P.ProviderPrimaryEmail AS Email, 
	                            P.ProviderPrimaryPhone AS Phone,
	                            PPU.ProviderUserNumber AS ProviderNumber,
	                            CAST(CASE WHEN PPU.ProviderUserNumber IS NOT NULL THEN 1 ELSE 0 END AS BIT) AS IsActive,
	                            CAST(COALESCE(PPU.ProviderHasAppAccess,0) AS BIT) AS HasAppAccess
                            FROM Providers AS P
                            LEFT JOIN ProviderPortalUsers AS PPU ON PPU.ProviderID = P.ID
                            ORDER BY ProviderFirstName, ProviderLastName
                        ").ToList();

            return items;
        }


    }
}