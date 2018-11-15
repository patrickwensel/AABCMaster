using Dymeng.Framework.Web.Mvc.Views;
using System;
using System.Linq;
using System.Web.Mvc;

namespace AABC.Web.Controllers
{
    [Authorize]
    public class ProviderPortalController : Dymeng.Framework.Web.Mvc.Controllers.ContentBaseController
    {
        private Repositories.IProviderPortalRepository repository;
        private App.ProviderPortal.ProviderPortalService _service;


        public ProviderPortalController()
        {
            repository = new Repositories.ProviderPortalRepository();
            _service = new App.ProviderPortal.ProviderPortalService();
        }


        public ActionResult GrantAppAccess(int providerID)
        {
            _service.GrantMobileAppAccess(providerID);
            return new HttpStatusCodeResult(200);
        }


        public ActionResult RevokeAppAccess(int providerID)
        {
            _service.RevokeMobileAppAccess(providerID);
            return new HttpStatusCodeResult(200);
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Index()
        {
            var localModel = new Models.ProviderPortal.UserVM
            {
                Base = new ViewModelBase(PushState, "/Admin/ProviderPortal", "Provider Portals")
            };
            return PartialView("Index", localModel);
        }


        [HttpGet]
        public ActionResult RegisterPopup(int providerID, string registerAction)
        {
            var data = repository.GetProvider(providerID);
            var model = new Models.ProviderPortal.RegisterVM
            {
                Password = System.Web.Security.Membership.GeneratePassword(8, 1),
                RegisterAction = registerAction,
                Email = data.Email,
                SendEmail = true,
                ProviderID = providerID,
                ProviderName = data.CommonName
            };
            if (data.NPI != null)
            {
                model.ProviderNumber = data.NPI;
            }
            else
            {
                if (data.ProviderNumber != null)
                {
                    model.ProviderNumber = data.ProviderNumber;
                }
                else
                {
                    model.ProviderNumber = repository.GenerateProviderNumber(providerID);
                }
            }
            return PartialView("RegisterPopup", model);
        }


        [HttpGet]
        public ActionResult IndexGrid()
        {
            var localModel = new Models.ProviderPortal.UserVM
            {
                Items = repository.GetUserItems(),
                Base = new ViewModelBase(PushState, "/Admin/ProviderPortal", "Provider Portals")
            };
            return PartialView("IndexGrid", localModel);
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult GridCallback()
        {
            var localModel = new Models.ProviderPortal.UserVM
            {
                Items = repository.GetUserItems(),
                Base = new ViewModelBase(PushState, "/Admin/ProviderPortal", "Provider Portals")
            };
            return PartialView("IndexGrid", localModel);
        }


        [HttpPost]
        public ActionResult UnregisterProvider(int providerID)
        {
            try
            {
                repository.UnregisterProvider(providerID);
                return Content("ok");
            }
            catch (Exception e)
            {
                Dymeng.Framework.Exceptions.Handle(e, Global.GetWebInfo());
                return Content("We're sorry, we ran into a problem registering this Provider.  Please contact your administrator or developer for details.");
            }
        }


        [HttpPost]
        public ActionResult RegisterProvider(int providerID, string registerAction, string providerNumber, string password, string email, bool sendEmail)
        {
            try
            {
                var model = new Models.ProviderPortal.RegisterVM
                {
                    RegisterAction = registerAction,
                    Email = email,
                    Password = password,
                    ProviderID = providerID,
                    ProviderNumber = providerNumber,
                    SendEmail = sendEmail
                };
                repository.RegisterProvider(model);
                if (sendEmail == true && !string.IsNullOrEmpty(email))
                {
                    var smtpInfo = AppService.Current.DataContext.SMTPAccounts.Where(x => x.AccountName == "Primary").SingleOrDefault();
                    if (smtpInfo == null) { throw new ArgumentNullException("Primary SMTP Account info has not been configured."); }
                    var smtpAccount = new Domain.Email.SMTPAccount()
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
                    string subject = "Applied Behavioral Provider Portal Account Created";
                    string message = "An account has been created for you on the Applied Behavioral Provider Portal. You can log in to your new account at {0}\n\nUser Name: {1}\nPassword: {2}";
                    message = String.Format(message, AppService.Current.Settings.ProviderPortalSite, providerNumber, password);
                    DomainServices.Email.SMTP.Send(smtpAccount, subject, message, email);
                }
                return Content("ok");
            }
            catch (Exception e)
            {
                Dymeng.Framework.Exceptions.Handle(e, Global.GetWebInfo());
                return Content("We're sorry, we ran into a problem registering this Provider.  Please contact your administrator or developer for details.");
            }
        }

    }
}