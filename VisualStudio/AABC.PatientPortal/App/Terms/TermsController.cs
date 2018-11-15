using AABC.PatientPortal.App.Terms.Models;
using System.Web.Mvc;

namespace AABC.PatientPortal.App.Terms
{
    public class TermsController : Dymeng.Web.Mvc.ControllerBase
    {
        private readonly TermsService _service;

        public TermsController()
        {
            _service = new TermsService();
        }

        public ActionResult Index()
        {
            var latestTerms = _service.GetLatestTerms();
            if (latestTerms == null)
            {
                return HttpNotFound();
            }
            var model = new TermsVM();
            model.Text = latestTerms.Text;
            model.LastUpdated = latestTerms.Created;
            model.IsAccepted = _service.UserHasAcceptedTerms(latestTerms);
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult Accept()
        {
            _service.AcceptTerms();
            return Redirect("/");
        }
    }
}