using AABC.Mobile.SharedEntities.Messages;
using System.Web.Http;

namespace AABC.Mobile.Api.Controllers
{
    [RoutePrefix("api/Version")]
    public class VersionController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ISettingsProvider _settingsRepository;


        public VersionController(ISettingsProvider settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        [Route("Current")]
        [Route("~/api/Cases/CurrentVersion")]
        [HttpGet]
        [AllowAnonymous]
        public CurrentVersionResponse GetCurrentVersion(string appVersion)
        {
            log.Info("GetCurrentVersion");
            string currentVersion = _settingsRepository.Version_Server.Value;
            return new CurrentVersionResponse { LatestVersion = currentVersion, UpdateRequired = false, UpdateAvailable = false };
        }

    }
}