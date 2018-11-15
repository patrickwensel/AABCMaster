using AABC.Web.App.Insurance.Models;
using AABC.Web.Helpers;
using Dymeng.Framework.Web.Mvc.Views;
using System.Web.Mvc;

namespace AABC.Web.App.Insurance
{

    [Authorize]
    [AuthorizePermissions(Domain.Admin.Permissions.InsuranceEdit)]
    public class InsuranceController : Dymeng.Framework.Web.Mvc.Controllers.ContentBaseController
    {

        public ActionResult Index() {
            ViewBag.Push = new ViewModelBase(PushState, "/Insurance/Index", "Insurance");
            return GetView("Base");
        }

        public ActionResult InsuranceForm(int Id)
        {
            var model = _service.GetInsuranceEditItem(Id);
            return PartialView("InsuranceForm", model);
        }
        [HttpPost]
        public ActionResult InsuranceForm(InsuranceEditVM model)
        {
            _service.SaveInsurance(model);
            return PartialView("InsuranceForm", model);
        }

        public ActionResult List() {
            var model = _service.GetInsuranceListItems();
            return PartialView("List", model);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ListGrid() {
            var model = _service.GetInsuranceListItems();
            return PartialView("ListGrid", model);
        }
                
        public ActionResult Edit(int? id) {

            var model = _service.GetInsuranceEditItem(id);
            
            return PartialView("Edit", model);
        }

        public ActionResult Add(string name, int? copySourceId) {

            string message;
            if (_service.ValidateInsuranceAddition(name, out message)) {
                if (copySourceId.HasValue)
                {
                    _service.CopyInsurance(name, copySourceId.Value);
                }else
                {
                    _service.AddInsurance(name);
                }
                return Content("ok");
            } else {
                return Content(message);
            }
        }

        public ActionResult Delete(int id) {

            string message;
            if (_service.ValidateInsuranceDeletion(id, out message)) {
                _service.RemoveInsurance(id);
                return Content("ok");
            } else {
                return Content(message);
            }
            
        }


        public ActionResult CarrierDelete(int carrierID) {
            string result = _service.DeleteCarrier(carrierID);
            return Content(result);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult CarrierItems(int insuranceID) {
            var model = _service.GetCarrierItems(insuranceID);
            return PartialView("CarriersGrid", model);
        }

        [HttpPost]
        public ActionResult SaveCarrier(int? id, int insuranceID, string carrierName) {
            _service.SaveCarrier(id, insuranceID, carrierName);
            return Content("ok");
        }

        


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ServiceItems(int insuranceID) {
            var model = _service.GetServiceListItems(insuranceID);
            return PartialView("ServicesGrid", model);
        }

        [HttpGet]
        public ActionResult ServiceEditItem(int? insuranceServiceID) {

            var model = new ServiceEditVM();

            if (insuranceServiceID.HasValue) {
                model = _service.GetServiceEditVM(insuranceServiceID.Value);
            }

            model.ProviderTypesList = Helpers.CommonListItems.GetProviderTypes();
            model.ServicesList = Helpers.CommonListItems.GetServices();

            return PartialView("ServicesEdit", model);
        }

        [HttpPost]
        public ActionResult ServiceEdit(ServiceEditVM model) {
            _service.SaveService(model);
            return Content("ok");
        }

        [HttpPost]
        public ActionResult ServiceDelete(int insuranceServiceID) {
            _service.RemoveService(insuranceServiceID);
            return Content("ok");
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult AuthRuleItems(int insuranceID) {

            var model = _service.GetAuthRules(insuranceID);

            return PartialView("AuthRulesGrid", model);
        }

        [HttpGet]
        public ActionResult AuthRuleEditItem(int? ruleID) {

            var model = new AuthRuleEditVM();

            if (ruleID.HasValue) {
                model = _service.GetAuthRuleEditVM(ruleID.Value);
            }
            
            model.ProviderTypesList = Helpers.CommonListItems.GetProviderTypes();
            model.ServicesList = Helpers.CommonListItems.GetServices();
            model.AuthCodes = Helpers.CommonListItems.GetAuthCodes();

            return PartialView("AuthRuleEdit", model);
        }

        [HttpPost]
        public ActionResult AuthRuleEdit(AuthRuleEditVM model) {

            _service.SaveAuthRule(model);

            return Content("ok");
        }

        [HttpPost]
        public ActionResult AuthRuleDelete(int ruleID) {

            _service.RemoveAuthRule(ruleID);
            return Content("ok");

        }








        private InsuranceService _service;

        public InsuranceController() {
            _service = new InsuranceService();
        }

    }
}