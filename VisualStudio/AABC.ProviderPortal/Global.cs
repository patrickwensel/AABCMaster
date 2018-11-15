using System.Web;

namespace AABC.ProviderPortal
{
    public class Global
    {


        public static string GetWebInfo()
        {

            string info = "ProvPort,";

            try
            {
                var req = HttpContext.Current.Request;
                info += req.HttpMethod + "," + req.RawUrl + "," + req.ServerVariables["SERVER_PROTOCOL"];
            }
            catch
            {
                // nothing
            }
            return info;

        }


        public static void ClearInstance()
        {
            instance = null;
        }

        static Global instance;

        public static Global Default
        {
            get
            {
                if (instance == null)
                {
                    instance = new Global();
                }
                return instance;
            }
        }



        public Domain.ProviderPortal.ProviderPortalUser User()
        {
            var currentLoginName = HttpContext.Current.User.Identity.Name;
            return PpService.GetPortalUserByAspNetUsername(currentLoginName);
        }

        private Data.Services.ProviderPortalService PpService { get; set; }

        public Global()
        {
            InitPPService();
        }

        private void InitPPService()
        {
            PpService = new Data.Services.ProviderPortalService();
        }

        internal Domain.Providers.Provider GetUserProvider()
        {
            var ppu = Global.Default.User();
            return PpService.GetProvider(ppu.ProviderID.Value);
        }
    }
}