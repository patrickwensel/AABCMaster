using AABC.Web.App.PatientPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.Web.App.PatientPortal
{
    public class PatientPortalService
    {

        
        internal ExistingVM GetExistingLoginsVM() {

            var model = new ExistingVM();
            model.Items = new List<ExistingLoginListItem>();

            var itemsData = _context.PatientPortalLogins.OrderBy(x => x.Email).ToList();

            foreach (var data in itemsData) {

                var item = new ExistingLoginListItem();

                item.ID = data.ID;
                item.Active = data.Active;
                item.Email = data.Email;
                item.FirstName = data.FirstName;
                item.LastName = data.LastName;
                item.Patients = new List<ExistingLoginPatientListItem>();
                foreach (var p in data.Patients) {
                    item.Patients.Add(new ExistingLoginPatientListItem()
                    {
                        ID = p.ID,
                        FirstName = p.FirstName,
                        LastName = p.LastName
                    });
                }
                model.Items.Add(item);
            }

            return model;

        }

        internal string ResetPassword(int loginID) {

            var membership = _context.PatientPortalLogins.Find(loginID).WebMembershipDetail;

            string pass = PatientPortalExtendedServices.GeneratePassword();

            membership.Password = pass;

            _context.SaveChanges();


            var smtpInfo = AppService.Current.DataContext.SMTPAccounts.Where(x => x.AccountName == "Primary").SingleOrDefault();
            if (smtpInfo == null) { throw new ArgumentNullException("Primary SMTP Account info has not been configured."); }
            var smtpAccount = new AABC.Domain.Email.SMTPAccount()
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

            string subject = "Applied Behavioral Parent Portal Account Password Reset";
            string message = "The password for your account on the Applied Behavioral Parent Portal has been reset. You can log in to your account at {0}\n\nUserName: {1}\nPassword: {2}";
            message = String.Format(message, AppService.Current.Settings.PatientPortalSite, membership.User.Email, pass);
            AABC.DomainServices.Email.SMTP.Send(smtpAccount, subject, message, membership.User.Email);


            return pass;

        }

        internal ExistingLoginListItem GetLogin(string Email)
        {
            var login = _context.PatientPortalLogins.Where(x => x.Email == Email).FirstOrDefault();
            if (login!=null)
            {
                return new ExistingLoginListItem() { Active = login.Active, Email = login.Email, FirstName = login.FirstName, LastName = login.LastName };
            }
            return null;
        }
        internal string AddLogin(string email, string firstName, string lastName, bool active) {

            if (_context.PatientPortalLogins.Where(x => x.Email == email).Count() > 0) {
                return null;
            }

            // EF6 add login is giving me errors about inserting IDs and I've spent enough time
            // trying to troubleshoot... will resort to manual creation here
            string password = PatientPortalExtendedServices.CreateLogin(email, firstName, lastName, active);

            var smtpInfo = AppService.Current.DataContext.SMTPAccounts.Where(x => x.AccountName == "Primary").SingleOrDefault();
            if (smtpInfo == null) { throw new ArgumentNullException("Primary SMTP Account info has not been configured."); }
            var smtpAccount = new AABC.Domain.Email.SMTPAccount()
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

            string subject = "Applied Behavioral Parent Portal Account Created";
            string message = "An account has been created for you on the Applied Behavioral Parent Portal. You can log in to your new account at {0}\n\nUserName: {1}\nPassword: {2}";
            message = String.Format(message, AppService.Current.Settings.PatientPortalSite, email, password);
            AABC.DomainServices.Email.SMTP.Send(smtpAccount, subject, message, email);

            return password;
        }

        internal void UpdateLogin(string email, string firstName, string lastName, bool active)
        {

            if (_context.PatientPortalLogins.Where(x => x.Email == email).Count() == 0)
            {
                throw new Exception("Not Found");
            }
            PatientPortalExtendedServices.UpdateLogin(email, firstName, lastName, active);

        }

        internal bool RemovePatientFromLogin(int loginID, int patientID) {

            var login = _context.PatientPortalLogins.Find(loginID);
            if (login.CasePeriodApprovals.Where(x => x.Period.Case.PatientID == patientID).Any())
            {
                return false;
            }

            var patient = login.Patients.Where(x => x.ID == patientID).FirstOrDefault();

            if (patient != null) {
                login.Patients.Remove(patient);
                _context.SaveChanges();
            }

            return true;
        }

        internal void AddPatientToLogin(int loginID, int patientID) {

            var login = _context.PatientPortalLogins.Find(loginID);
            var patient = _context.Patients.Find(patientID);

            login.Patients.Add(patient);

            _context.SaveChanges();
        }

        internal AddRemovePatientsVM GetAddRemovePatientsPopup(int loginID) {
            
            var model = new AddRemovePatientsVM();

            model.LoginID = loginID;
            model.CurrentPatients = GetCurrentPatients(loginID);
            model.PatientsList = GetPatientListItems();

            return model;
        }


        internal List<CurrentPatientListItem> GetCurrentPatients(int loginID) {

            
            var login = _context.PatientPortalLogins.Find(loginID);
            var items = new List<CurrentPatientListItem>();
            
            foreach (var patient in login.Patients) {
                items.Add(new CurrentPatientListItem()
                {
                    ID = patient.ID,
                    Name = patient.CommonName
                });
            }

            return items;

        }

        internal List<PatientListItem> GetPatientListItems() {
            var patients = from p in _context.Patients
                           orderby p.FirstName
                           where p.Cases.Any(c => c.Status != Domain2.Cases.CaseStatus.History)
                           select new PatientListItem
                           {
                               ID = p.ID,
                               Email = p.Email,
                               FirstName = p.FirstName,
                               LastName = p.LastName
                           };

            return patients.ToList();
        }

        internal ActivationToggleStatusVM GetActivationToggleStatus(int id) {

            var login = _context.PatientPortalLogins.Find(id);

            return new ActivationToggleStatusVM()
            {
                active = login.Active,
                email = login.Email
            };

        }

        internal void ToggleActivation(int id) {

            var login = _context.PatientPortalLogins.Find(id);
            login.Active = !login.Active;
            _context.SaveChanges();

        }

        Data.V2.CoreContext _context;

        public PatientPortalService() {
            _context = AppService.Current.DataContextV2;
        }
    }
}