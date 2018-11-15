using System.Web;

namespace AABC.ProviderPortal
{
    public class AppService
    {
        
        public Types.Settings Settings { get; set; } = new Types.Settings();
        
        private static AppService _instance;
        public static AppService Current {
            get {
                if (_instance == null) {
                    _instance = new AppService();
                }
                return _instance;
            }
        }

        public string ApplicationInsightsInstrumentationKey {
            get {
                return System.Configuration.ConfigurationManager.AppSettings["ApplicationInsightsInstrumentationKey"];
            }
        }
        
        public Data.V2.CoreContext Context {
            get {
                if (!HttpContext.Current.Items.Contains("DataV2Context")) {
                    HttpContext.Current.Items.Add("DataV2Context", new Data.V2.CoreContext());                        
                }
                return HttpContext.Current.Items["DataV2Context"] as Data.V2.CoreContext;
            }
        }

        public Domain2.Providers.Provider CurrentProvider {
            get {
                var id = Global.Default.GetUserProvider().ID;
                return this.Context.Providers.Find(id);
            }
        }

        public class Types
        {

            public class DocuSignProviderFinalizeConfig
            {
                public string AuthUserID { get { return System.Configuration.ConfigurationManager.AppSettings["DocuSign.Provider.Finalize.Auth.UserID"]; } }
                public string AuthOAuthBasePath { get { return System.Configuration.ConfigurationManager.AppSettings["DocuSign.Provider.Finalize.Auth.OAuthBasePath"]; } }
                public string AuthIntegratorKey { get { return System.Configuration.ConfigurationManager.AppSettings["DocuSign.Provider.Finalize.Auth.IntegratorKey"]; } }
                public string AuthPrivateKeyPath { get { return System.Configuration.ConfigurationManager.AppSettings["DocuSign.Provider.Finalize.Auth.PrivateKeyPath"]; } }
                public string AuthHost { get { return System.Configuration.ConfigurationManager.AppSettings["DocuSign.Provider.Finalize.Auth.Host"]; } }
                public string DefaultSignerEmail { get { return System.Configuration.ConfigurationManager.AppSettings["DocuSign.Provider.Finalize.DefaultSignerEmail"]; } }
                public string DocumentFilename { get { return System.Configuration.ConfigurationManager.AppSettings["DocuSign.Provider.Finalize.DocumentFilename"]; } }
                public string EmailSubject { get { return System.Configuration.ConfigurationManager.AppSettings["DocuSign.Provider.Finalize.EmailSubject"]; } }
            }

            public class Settings
            {

                

                public DocuSignProviderFinalizeConfig DocuSignProviderFinalize { get; set; } = new DocuSignProviderFinalizeConfig();


                public int CaseVisibilityAfterEndDateDays {
                    get {
                        int x = 15;
                        string s = System.Configuration.ConfigurationManager.AppSettings["CaseVisibilityAfterEndDateDays"];
                        int.TryParse(s, out x);
                        return x;                         
                    }
                }

                public string ProviderSignedHoursPath
                {
                    get {
                        return System.Configuration.ConfigurationManager.AppSettings["ProviderSignedHoursDirectory"];
                    }
                }

                public bool DocusignProviderEnabled { get {
                        var s = System.Configuration.ConfigurationManager.AppSettings["DocuSignProviderEnabled"];
                        return s == "true";
                    }
                }

                public bool DocusignParentEnabled {
                    get {
                        var s = System.Configuration.ConfigurationManager.AppSettings["DocuSignParentEnabled"];
                        return s == "true";
                    }
                }

                public bool FinalizeDeepValidationRequired {
                    get {
                        var s = System.Configuration.ConfigurationManager.AppSettings["FinalizeDeepValidationRequired"];
                        return s == "true";
                    }
                }

                public bool FinalizeDeepValidationHasDataRequired { get {
                        var s = System.Configuration.ConfigurationManager.AppSettings["FinalizeDeepValidationHasDataRequired"];
                        return s == "true";
                    }
                }
            }
        }


    }
}