using System.Linq;

namespace AABC.Web.App.ProviderPortal
{
    public class ProviderPortalService
    {
        // port stuff from the Controller into here as we move along...
        private readonly Data.V2.CoreContext _context;


        public ProviderPortalService()
        {
            _context = AppService.Current.DataContextV2;
        }


        public void RevokeMobileAppAccess(int providerID)
        {
            var user = _context.ProviderPortalUsers.Where(x => x.ProviderID == providerID).Single();
            user.HasAppAccess = false;
            _context.SaveChanges();
        }


        public void GrantMobileAppAccess(int providerID)
        {
            var user = _context.ProviderPortalUsers.Where(x => x.ProviderID == providerID).Single();
            user.HasAppAccess = true;
            _context.SaveChanges();
        }

    }
}