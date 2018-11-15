using AABC.DomainServices.Payments;
using AABC.PatientPortal.App.Payments.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace AABC.PatientPortal.App.Payments
{
    [Authorize]
    public class PaymentsController : Dymeng.Web.Mvc.ControllerBase
    {
        private PaymentsService _service;
        private PaymentManager _paymentManager;


        public PaymentsController()
        {
            _service = new PaymentsService();
            _paymentManager = new PaymentManager(AppService.Current.Data.Context, PaymentManagerConfiguration.Create());
        }


        public ActionResult Index()
        {
            var model = new PaymentVM
            {
                Configuration = _paymentManager.Configuration,
                ActivePlan = _service.GetActivePlanByUser(AppService.Current.User.CurrentUser.ID),
                Patients = _service.GetPatients(AppService.Current.User.CurrentUser.ID)
            };
            return View("Index", model);
        }


        public ActionResult Scheduled()
        {
            return View();
        }


        public ActionResult History()
        {
            return View();
        }


        [HttpGet]
        public JsonResult GetScheduledPayments()
        {
            var data = _paymentManager.PaymentRepository.GetScheduledPaymentsByLoginId(AppService.Current.User.CurrentUser.ID, DateTime.Today);
            var results = data.Select(m => Mapper.ToPaymentWithScheduledDTO(m));
            return this.CamelCaseJson(results, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetCharges()
        {
            var data = _paymentManager.PaymentChargeRepository.GetChargesByLoginId(AppService.Current.User.CurrentUser.ID);
            var results = data.Select(m => Mapper.ToPaymentChargeDTO(m));
            return this.CamelCaseJson(results, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult SavePayment(PaymentCreationParameters payment)
        {
            var user = AppService.Current.User.CurrentUser;
            var patientLoginInfo = new PatientLoginInfo
            {
                ID = user.ID,
                LastName = user.LastName,
                FirstName = user.FirstName,
                Email = user.Email
            };
            var result = _paymentManager.SavePayment(patientLoginInfo, payment, null);
            return this.CamelCaseJson(result);
        }

        [HttpGet]
        [AllowAnonymous]
        //todo: anonymous and get????
        public JsonResult PaySchedules()
        {
            var results = _paymentManager.PayUnpaidSchedules(DateTime.Today);
            return Json(results, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult StopScheduled(int id)
        {
            var result = _paymentManager.StopScheduledPayment(id);
            return this.CamelCaseJson(result, JsonRequestBehavior.AllowGet);
        }



    }
}