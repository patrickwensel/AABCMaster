using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.LegacyConfiguration;
using StackExchange.Redis.Extensions.Newtonsoft;
using System.Web;

namespace AABC.Web
{
    public class AppService
    {

        private ICacheClient _cacheClient;

        public Types.Settings Settings { get; set; } = new Types.Settings();
        public ICacheClient CacheClient
        {
            get
            {
                if (_cacheClient == null)
                {
                    var redisConfiguration = RedisCachingSectionHandler.GetConfig();
                    _cacheClient = new StackExchangeRedisCacheClient(new NewtonsoftSerializer(), redisConfiguration);
                }

                return _cacheClient;
            }
        }


        public int UserID
        {
            get
            {
                return Global.Default.User().ID.Value;
            }
        }


        public Domain.Admin.User User
        {
            get
            {
                return Global.Default.User();
            }
        }


        public Data.V2.CoreContext DataContextV2
        {
            get
            {
                if (!HttpContext.Current.Items.Contains("DataContextV2"))
                {
                    HttpContext.Current.Items.Add(
                        "DataContextV2",
                        new Data.V2.CoreContext());
                }
                return HttpContext.Current.Items["DataContextV2"] as Data.V2.CoreContext;
            }
        }


        public Data.Models.CoreEntityModel DataContext
        {
            get
            {
                if (!HttpContext.Current.Items.Contains("DataContext"))
                {
                    HttpContext.Current.Items.Add(
                        "DataContext",
                        new Data.Models.CoreEntityModel());
                }
                return HttpContext.Current.Items["DataContext"] as Data.Models.CoreEntityModel;
            }
        }



        public class Types
        {
            public class Settings
            {
                public enum BCBAHoursNotesModes
                {
                    Normal = 0,
                    Extended = 1
                }

                public string ApplicationInsightsInstrumentationKey
                {
                    get
                    {
                        return System.Configuration.ConfigurationManager.AppSettings["ApplicationInsightsInstrumentationKey"];
                    }
                }

                public BCBAHoursNotesModes BCBAHoursNotesMode
                {
                    get
                    {
                        var x = System.Configuration.ConfigurationManager.AppSettings["BCBAHoursNotesMode"];
                        if (x == "Extended")
                        {
                            return BCBAHoursNotesModes.Extended;
                        }
                        else
                        {
                            return BCBAHoursNotesModes.Normal;
                        }
                    }
                }


                public string PatientPortalSite
                {
                    get
                    {
                        return System.Configuration.ConfigurationManager.AppSettings["PatientPortalSite"];
                    }
                }

                public string ProviderPortalSite
                {
                    get
                    {
                        return System.Configuration.ConfigurationManager.AppSettings["ProviderPortalSite"];
                    }
                }

                public string TempDirectory
                {
                    get
                    {
                        return System.Configuration.ConfigurationManager.AppSettings["TempDirectory"];
                    }
                }

                public string UploadDirectory
                {
                    get
                    {
                        return System.Configuration.ConfigurationManager.AppSettings["UploadDirectory"];
                    }
                }

                public string ExcelImportMethod
                {
                    get
                    {
                        try
                        {
                            return System.Configuration.ConfigurationManager.AppSettings["ExcelImportMethod"];
                        }
                        catch
                        {
                            return "ACE";
                        }
                    }
                }

                public string ProviderPortalDocuSignFinalizationsRoot
                {
                    get
                    {
                        return System.Configuration.ConfigurationManager.AppSettings["ProviderPortal.DocuSign.Finalizations.Root"];
                    }
                }
            }
        }


        private static AppService _appService;

        public static AppService Current
        {
            get
            {
                if (_appService == null)
                {
                    _appService = new AppService();
                }
                return _appService;
            }
        }

    }
}