using System.Web;

namespace AABC.Web
{


    public class Global
    {


        public static string GetWebInfo()
        {

            string info = "AABC.Manage,";

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



        public Domain.Admin.User User()
        {
            return WebUserService.GetUserByAspNetUsername(HttpContext.Current.User.Identity.Name);
        }

        private Data.Services.WebUserService WebUserService { get; set; }

        public Global()
        {
            InitWebUserService();
        }

        void InitWebUserService()
        {
            WebUserService = new Data.Services.WebUserService();
        }


    }
}