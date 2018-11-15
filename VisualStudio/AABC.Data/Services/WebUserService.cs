using AABC.Domain.Admin;
using Dymeng.Framework.Data.SqlClient;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AABC.Data.Services
{
    public class WebUserService
    {

        const string USER_CACHE_KEY_PREFIX = "UserCacheKey_aspnetName_";
        
        public static void RegenPermissions(int userID,string connString) {

            using (SqlConnection conn = new SqlConnection(connString))
            using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GenerateWebUserPermissions";
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.ExecuteNonQueryToInt();

            }

        }

        public static void RegenOptions(int userID, string connString) {
            
            using (SqlConnection conn = new SqlConnection(connString)) 
                using (SqlCommand cmd = new SqlCommand()) {

                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GenerateWebUserOptions";
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.ExecuteNonQueryToInt();

            }
            
        }

        public static void InvalidateUserCache(string aspNetName) {
            CacheService.Invalidate(USER_CACHE_KEY_PREFIX + aspNetName);
        }

        public User GetUserByAspNetUsername(string aspNetName) {

            string cacheKey = USER_CACHE_KEY_PREFIX + aspNetName;

            var user = CacheService.Get(cacheKey) as User;

            if (user != null) {

                return user;

            } else {

                user = setupUser(aspNetName);

                if (user == null) {
                    return null;
                } else {
                    CacheService.Add(cacheKey, user);
                    return user;
                }
            }
            
        }
        

        private User setupUser(string username) {

            var context = new Models.CoreEntityModel();

            var entity = context.WebUsers.Where(x => x.UserName == username).FirstOrDefault();

            if (entity == null) {
                return null;
            }
            
            var user = Mappings.WebUserMappings.User(entity);
            user.Permissions = Mappings.WebUserMappings.Permissions(context.WebUserPermissions.Where(x => x.WebUserID == user.ID).ToList());
            user.Options = Mappings.WebUserMappings.Options(context.WebUserOptions.Where(x => x.WebUserID == user.ID).ToList());

            return user;
        }




        

    }
}
