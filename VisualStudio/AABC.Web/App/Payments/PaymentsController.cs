using AABC.DomainServices.Payments;
using AABC.Web.App.Payments.Models;
using Dymeng.Framework.Web.Mvc.Controllers;
using System;
using System.Linq;
using System.Web.Mvc;

namespace AABC.Web.App.Payments
{
    public class PaymentsController : ContentBaseController
    {
        private readonly PaymentsService PaymentService;
        private readonly PaymentManager PaymentManager;

        public PaymentsController()
        {
            PaymentService = new PaymentsService(AppService.Current.DataContextV2);
            PaymentManager = new PaymentManager(AppService.Current.DataContextV2, PaymentManagerConfiguration.Create());
        }

        [HttpGet]
        public JsonResult GetPayment(int caseID)
        {
            var patientId = PaymentService.GetPatientId(caseID);
            var model = new PaymentVM();
            model.Configuration = PaymentManager.Configuration;
            model.PatientLogins = PaymentService.GetPatientLogins(patientId);
            model.Data.PatientId = patientId;
            return this.CamelCaseJson(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetScheduledPayments(int caseId)
        {
            var patientId = PaymentService.GetPatientId(caseId);
            var data = PaymentManager.PaymentRepository.GetScheduledPaymentsByPatientId(patientId, DateTime.Today);
            var results = data.Select(m => Mapper.ToPaymentWithScheduledDTO(m));
            return this.CamelCaseJson(results, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetCharges(int caseId)
        {
            var patientId = PaymentService.GetPatientId(caseId);
            var data = PaymentManager.PaymentChargeRepository.GetChargesByPatientId(patientId);
            var results = data.Select(m => Mapper.ToPaymentChargeDTO(m));
            return this.CamelCaseJson(results, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult SavePayment(ExtendedPaymentCreationParameters payment)
        {
            var patientLogin = PaymentService.GetPatientLogin(payment.PatientLoginId);
            if (patientLogin == null)
            {
                throw new Exception("Payment without an existing patient login cannot be processed.");
            }
            var patientLoginInfo = new PatientLoginInfo
            {
                ID = patientLogin.ID,
                LastName = patientLogin.LastName,
                FirstName = patientLogin.FirstName,
                Email = patientLogin.Email
            };
            var result = PaymentManager.SavePayment(patientLoginInfo, payment, AppService.Current.User.ID.Value);
            return this.CamelCaseJson(result);
        }


        [HttpPost]
        public ActionResult StopScheduled(int id)
        {
            var result = PaymentManager.StopScheduledPayment(id);
            return this.CamelCaseJson(result, JsonRequestBehavior.AllowGet);
        }
    }
}