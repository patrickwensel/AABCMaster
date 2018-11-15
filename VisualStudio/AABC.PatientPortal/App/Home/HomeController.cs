using System;
using System.Linq;
using System.Web.Mvc;

namespace AABC.PatientPortal.App.Home
{

    [Authorize]
    public class HomeController : Dymeng.Web.Mvc.ControllerBase
    {


        public ActionResult Index(int? currentPatientID, DateTime? currentPeriodDate) {

            var model = _service.GetHomeModel(currentPatientID);
            return View("Index", model);

        }


        [HttpPost]
        public ActionResult ApproveHours(int patientID, string groupID) {

            try {

                if (!AppService.Current.User.CurrentUser.Signatures.Any())
                {
                    return Content("signature required");
                }

                var firstDay = new DateTime(
                int.Parse(groupID.Substring(0, 4)),
                int.Parse(groupID.Substring(4, 2)),
                1);

                _service.ApproveHours(patientID, firstDay);

                return Content("ok");

            }
            catch {
                return Content("Unexpected error");
            }
        }

        [HttpPost]
        public ActionResult ReportHours(int hoursID, string comment)
        {

            try
            {
                _service.LogHours(hoursID, comment);
                _service.ReportHours(hoursID, comment);

                return Json("ok");
            }
            catch
            {
                return Json("An error occurred while reporting hours.");
            }
        }

        






        private HomeService _service;
        public HomeController() {
            _service = new HomeService();
        }

    }

}