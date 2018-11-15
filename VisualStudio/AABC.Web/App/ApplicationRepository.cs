namespace AABC.Web.Repositories
{
    public class ApplicationRepository
    {
        static ApplicationRepository instance;

        public static ApplicationRepository Default {
            get
            {
                if (instance == null) {
                    instance = new ApplicationRepository();
                }
                return instance;
            }
        }

    }
}