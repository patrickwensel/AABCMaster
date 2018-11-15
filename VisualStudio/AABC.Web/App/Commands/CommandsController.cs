using AABC.Cache;
using System;
using System.Web.Mvc;

namespace AABC.Web.Controllers
{
    public class CommandsController : Controller
    {
        // GET: Commands
        public ActionResult Index()
        {
            return PartialView();
        }


        [HttpPost]
        public ActionResult ResolveExistingAuthBreakdowns(DateTime startDate, DateTime endDate)
        {
            try
            {
                var batchResolver = new DomainServices.Hours.BatchAuthResolver(new Data.V2.CoreContext());
                batchResolver.ResolveAllAuths(startDate, endDate);
                return Content("Resolution successful");
            }
            catch (Exception e)
            {
                return Content("We're sorry, we ran into an issue with that.  Please report error " + e.HResult.ToString() + " to your administrator.");
            }
        }


        [HttpPost]
        public ActionResult ClearCache()
        {
            var cacheManager = new CacheManager(AppService.Current.CacheClient);
            cacheManager.FlushAll();
            return this.CamelCaseJson(new { Success = true });
        }
    }
}