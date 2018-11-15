using System.Web.Mvc;

namespace AABC.PatientPortal.App.Patients
{
    [Authorize]
    public class PatientsController : Dymeng.Web.Mvc.ControllerBase
    {

        public ActionResult Index() {

            var model = new Models.PatientVM();
            return View("Index", model);
        }

        //public JsonResult GetPatientsForLogin()
        //{
        //    return Json(_service.GetByLogin(AppService.Current.User.ID), JsonRequestBehavior.AllowGet);
            
        //}
        private PatientsService _service;
        public PatientsController() {
            _service = new PatientsService();
        }
    }
}