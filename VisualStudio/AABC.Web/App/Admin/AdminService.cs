using System;
using System.Linq;

namespace AABC.Web.App.Admin
{
    public class AdminService
    {


        public bool ResetPassword(int userID, out string newPassword)
        {

            string password = GeneratePassword();
            string username = _legacyUserRepository.GetUser(userID).UserName;

            try
            {
                WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection("DefaultConnection", "UserProfile", "UserId", "UserName", autoCreateTables: true);
            }
            catch (Exception)
            {
                // skip "initializedatabaseconnection can only be called once...
                // TODO: find out exact exception so we're not burying everything
            }

            string token = WebMatrix.WebData.WebSecurity.GeneratePasswordResetToken(username);

            try
            {
                WebMatrix.WebData.WebSecurity.ResetPassword(token, password);
                newPassword = password;
                return true;
            }
            catch (Exception)
            {
                newPassword = null;
                return false;
            }
        }


        private static Random random = new Random();
        private string GeneratePassword()
        {

            int length = 8;

            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        private Account.UserRepository _legacyUserRepository;
        private readonly Data.V2.CoreContext _context;

        public AdminService()
        {
            _legacyUserRepository = new App.Account.UserRepository();
            _context = AppService.Current.DataContextV2;
        }


    }
}