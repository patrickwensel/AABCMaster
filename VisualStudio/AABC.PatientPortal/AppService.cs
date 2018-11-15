using System.Linq;
using System.Web;

namespace AABC.PatientPortal
{
    public class AppService
    {

        public const string APP_NAME = "AABC.PatientPortal";

        private static AppService _current;

        public static AppService Current {
            get
            {
                if (_current == null) {
                    _current = new AppService();
                }
                return _current;
            }
        }

        public Types.Data Data;
        public Types.User User;
        public Types.Navigation Navigation;
        public Types.Settings Settings;

        public AppService() {
            Data = new Types.Data(this);
            User = new Types.User(this);
            Navigation = new Types.Navigation(this);
            Settings = new Types.Settings(this);
        }


        public class Types
        {


            public class Settings
            {
                private AppService _service;
                public Settings(AppService service) {
                    _service = service;                    
                }

                public bool ShowPaymentsNavItem {
                    get
                    {

                        if (!paymentsFeatureEnabled) {
                            return false;
                        }

                        if (allowPaymentsForAll) {
                            return true;
                        }

                        string s = System.Configuration.ConfigurationManager.AppSettings["AllowPaymentsLoginIDList"];
                        if (string.IsNullOrEmpty(s)) {
                            return false;
                        }

                        s = s.Trim();

                        string[] ids = s.Split(',');

                        foreach (string id in ids) {
                            if (int.Parse(id) == _service.User.CurrentUser.ID) {
                                return true;
                            }
                        }

                        return false;
                    }
                }

                private bool allowPaymentsForAll
                {
                    get
                    {
                        string value = System.Configuration.ConfigurationManager.AppSettings["AllowPaymentsForAll"];
                        if (value.ToLower() == "true")
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                
                private bool paymentsFeatureEnabled {
                    get
                    {
                        string value = System.Configuration.ConfigurationManager.AppSettings["PaymentsFeatureEnabled"];
                        if (value.ToLower() == "true") {
                            return true;
                        } else {
                            return false;
                        }
                    }
                }

                public bool AllowApproveAny {
                    get
                    {
                        string value = System.Configuration.ConfigurationManager.AppSettings["PatientPortalAllowApproveAny"];
                        if (value.ToLower() == "true") {
                            return true;
                        } else {
                            return false;
                        }
                    }
                }

                public int ViewableMonths {
                    get
                    {
                        int months = 6; // default
                        try {
                            months = int.Parse(System.Configuration.ConfigurationManager.AppSettings["PatientPortalVisibleMonths"]);
                        } catch {
                            // couldn't find an explicit setting, use default
                        }
                            
                        return months;
                    }
                }

            }


            public class Navigation
            {
                private AppService _service;
                private App.Shared.Navigation navService;

                public Navigation(AppService service) {
                    _service = service;
                    navService = new App.Shared.Navigation();
                }

                public bool IsMatched(string path, App.Shared.Navigation.NavigationKey key) {
                    var keys = navService.GetActiveKeys(path);
                    if (keys != null && keys.Contains(key)) {
                        return true;
                    } else {
                        return false;
                    }
                }



            }


            public class User
            {

                private AppService _service;

                public User(AppService service) {
                    _service = service;
                }


                public void ClearCache() {
                    CacheService.Current.User.Remove(CacheService.Types.UserCache.Key.User);
                }

                public int ID { get { return getLoginFromCache().ID; } }
                public string UserName { get { return getLoginFromCache().Email; } }
                public Domain2.PatientPortal.Login CurrentUser { get { return getLoginFromCache(); } }
                
                private Domain2.PatientPortal.Login getLoginFromDatabase() {
                    return _service.Data.Context.PatientPortalLogins
                        .Where(x => x.Email == HttpContext.Current.User.Identity.Name)
                        .Single();
                }

                private Domain2.PatientPortal.Login getLoginFromCache() {
                    var obj = CacheService.Current.User.Get(CacheService.Types.UserCache.Key.User);
                    if (obj != null) {
                        return obj as Domain2.PatientPortal.Login;
                    } else {
                        var user = getLoginFromDatabase();
                        CacheService.Current.User.Add(CacheService.Types.UserCache.Key.User, user);
                        return user;
                    }
                }
                
            }



            public class Data
            {
                private AppService _service;

                public Data(AppService service) {
                    _service = service;
                }


                public AABC.Data.V2.CoreContext Context {
                    get
                    {
                        if (!HttpContext.Current.Items.Contains("CoreContext")) {
                            HttpContext.Current.Items.Add("CoreContext", new AABC.Data.V2.CoreContext());
                        }
                        return HttpContext.Current.Items["CoreContext"] as AABC.Data.V2.CoreContext;
                    }
                }

                public AABC.Data.Models.CoreEntityModel OldContext
                {
                    get
                    {
                        if (!HttpContext.Current.Items.Contains("DataContext"))
                        {
                            HttpContext.Current.Items.Add(
                                "DataContext",
                                new AABC.Data.Models.CoreEntityModel());
                        }
                        return HttpContext.Current.Items["DataContext"] as AABC.Data.Models.CoreEntityModel;
                    }
                }

            }


        }


    }
}