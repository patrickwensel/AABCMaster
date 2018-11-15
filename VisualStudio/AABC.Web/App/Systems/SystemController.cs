using AABC.Domain.Email;
using Dymeng.Framework.Web.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace AABC.Web.App.Systems
{
    public class SystemController : ContentBaseController
    {
        private readonly SystemService SystemService;

        public SystemController()
        {
            var apiKey = ConfigurationManager.AppSettings["PaymentGateways.Stripe.ApiKey"].ToString();
            var apiEndPoint = ConfigurationManager.AppSettings["PaymentGateways.Stripe.ApiEndpoint"].ToString();
            var smtpInfo = AppService.Current.DataContext.SMTPAccounts.Where(x => x.AccountName == "Primary").SingleOrDefault();
            if (smtpInfo == null) { throw new ArgumentNullException("Primary SMTP Account info has not been configured."); }
            var smtpAccount = new SMTPAccount()
            {
                Username = smtpInfo.AccountUsername,
                Password = smtpInfo.AccountPassword,
                Server = smtpInfo.AccountServer,
                Port = smtpInfo.AccountPort.Value,
                UseSSL = smtpInfo.AccountUseSSL.Value,
                AuthenticationMode = smtpInfo.AccountAuthMode.Value,
                FromAddress = smtpInfo.AccountDefaultFromAddress,
                ReplyToAddress = smtpInfo.AccountDefaultReplyAddress
            };
            var connectionString = ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString;
            var recipient = ConfigurationManager.AppSettings["Payments.RBTRecipientAddress"].ToString();
            SystemService = SystemService.Create(apiKey, apiEndPoint, smtpAccount, recipient, connectionString);
        }

        [HttpPost]
        [AllowCrossSiteJson]
        [AllowAnonymous]
        public JsonResult SubmitRBTCoursePayment(PaymentParameters model)
        {
            var description = ConfigurationManager.AppSettings["Payments.RBTCourseDescription"].ToString();
            var amount = int.Parse(ConfigurationManager.AppSettings["Payments.RBTCourseAmount"].ToString());
            var result = SystemService.ProcessPayment(model, description, amount);
            return Json(result);
        }
    }

    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var domains = new List<string> { "localhost", "appliedabc.com" };
            var host = filterContext.RequestContext.HttpContext.Request.UrlReferrer.Host;
            if (domains.Contains(host))
            {
                filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
            }
            base.OnActionExecuting(filterContext);
        }
    }

}