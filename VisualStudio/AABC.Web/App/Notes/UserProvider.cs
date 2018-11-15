using AABC.Domain.Admin;
using AABC.DomainServices.Notes;

namespace AABC.Web.App.Notes
{
    public class UserProvider : IUserProvider
    {
        public User GetUser()
        {
            return Global.Default.User();
        }
    }
}