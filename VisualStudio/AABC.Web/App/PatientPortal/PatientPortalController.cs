using AABC.DomainServices.Patients;
using AABC.Web.App.PatientPortal.Models;
using System;
using System.Web.Mvc;
namespace AABC.Web.App.PatientPortal
{
    [Authorize]
    public class PatientPortalController : Controller
    {



        [HttpPost]
        public ActionResult ResetPassword(int loginID) {

            string pass = _service.ResetPassword(loginID);

            return Content(pass);
        }


        [HttpGet]
        public ActionResult AddUpdateLoginPopup(string Email) {
            ExistingLoginListItem model = _service.GetLogin(Email);
            if (model == null)
            {
                model = new ExistingLoginListItem() { Active = true, Email = "", FirstName = "", LastName = "" };
            }
            return PartialView("AddUpdateLoginPopup", model);
        }

        [HttpPost]
        public ActionResult AddUpdateLoginPopup(string email, string firstName, string lastName, bool active) {

            ExistingLoginListItem model = _service.GetLogin(email);
            if (model == null)
            {
                string password = _service.AddLogin(email, firstName, lastName, active);
                return Content(password);
            }else
            {
                _service.UpdateLogin(email, firstName, lastName, active);
                return new EmptyResult();
            }


        }

        [HttpPost]
        [Route("PatientPortal/Login/Patients/Remove")]
        public ActionResult LoginPatientsRemove(int loginID, int patientID) {

            if (_service.RemovePatientFromLogin(loginID, patientID))
            {
                return Content("ok");
            }
            else
            {
                return Content("Unable to remove this patient from this login, because this login has approved hours for this patient.");
            }

        }

        [HttpPost]
        [Route("PatientPortal/Login/Patients/Add")]
        public ActionResult LoginPatientsAdd(int loginID, int patientID) {

            _service.AddPatientToLogin(loginID, patientID);
            return Content("ok");
        }


        [HttpGet]
        public ActionResult AddRemovePatientsPopup(int loginID) {

            var model = _service.GetAddRemovePatientsPopup(loginID);

            return PartialView("AddRemovePatientsPopup", model);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult PatientsListGridCallback() {
            var model = _service.GetPatientListItems();
            return PartialView("PatientsListGridPartial", model);
        }

        
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index() {
            return PartialView("Index");
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult CurrentPatientsGridCallback() {
            var loginID = int.Parse(Request.Params["loginID"]);
            var model = _service.GetCurrentPatients(loginID);
            return PartialView("CurrentPatientsGridPartial", model);
        }


        [HttpGet]
        public ActionResult Existing() {
            var model = _service.GetExistingLoginsVM();
            return PartialView("ExistingLoginsGrid", model);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ExistingPortalLoginsGridCallback() {
            var model = _service.GetExistingLoginsVM();
            return PartialView("ExistingLoginsGrid", model);
        }

        [HttpGet]
        public ActionResult Patients() {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("PatientPortal/Activation/ToggleStatus")]
        public ActionResult ActivationToggleStatus(int id) {
            var model = _service.GetActivationToggleStatus(id);
            return Content(Newtonsoft.Json.JsonConvert.SerializeObject(model));
        }
        
        [HttpPost]
        [Route("PatientPortal/Login/{id:int}/Activation/Toggle")]
        public ActionResult LoginActivationToggle(int id) {

            _service.ToggleActivation(id);
            return Content("ok");
        }

        public ActionResult SignInSummary(DateTime? StartDate, DateTime? EndDate)
        {
            var signIns = svcSignIn.GetSignInSummary(StartDate, EndDate);
            return PartialView("SignInGrid", signIns);
        }





        private PatientPortalService _service;
        private SignInService svcSignIn;
        public PatientPortalController() {
            _service = new PatientPortalService();
            svcSignIn = new SignInService(AppService.Current.DataContextV2);
        }

    }
}