using AABC.Data.V2;
using AABC.Domain2.Providers;
using System.Configuration;

namespace AABC.DomainServices.Sessions
{
    public class SessionReportService
    {
        private readonly CoreContext Context;

        public SessionReportService(CoreContext context)
        {
            Context = context;
        }


        public SessionReportConfiguration GetSessionReportConfig(ProviderTypeIDs providerTypeID, int serviceID)
        {
            return SessionReportConfiguration.CreateSample();
            //var config = Context.SessionReportConfigurations.SingleOrDefault(s => s.ProviderTypeID == (int)providerTypeID && s.ServiceID == serviceID);
            //if (config == null)
            //{
            //    throw new System.Exception("Configuration does not exist.");
            //}
            //return JsonConvert.DeserializeObject<SessionReportConfiguration>(config.Configuration);
        }


        public static bool IsOnAideLegacyMode(Domain2.Hours.Hours h)
        {
            return IsOnAideLegacyMode(h.Provider, h);
        }


        public static bool IsOnAideLegacyMode(Provider p)
        {
            return IsOnAideLegacyMode(p, null);
        }


        private static bool IsOnAideLegacyMode(Provider p, Domain2.Hours.Hours h)
        {
            if (p.ProviderTypeID == (int)ProviderTypeIDs.BoardCertifiedBehavioralAnalyst)
            {
                return true;
            }
            else if (h != null && h.Report == null)
            {
                return true;
            }
            else
            {
                return bool.Parse(ConfigurationManager.AppSettings["NotesLegacy"]);
            }
        }
    }
}
