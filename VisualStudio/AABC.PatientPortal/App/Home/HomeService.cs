using AABC.PatientPortal.App.Home.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.PatientPortal.App.Home
{

    public class HomeService
    {





        internal HomeVM GetHomeModel(int? childID) {

            var model = new HomeVM();

            model.HasSignature = AppService.Current.User.CurrentUser.Signatures.Any();

            model.Children = getChildren();

            model.ActiveChild = getActiveChild(model, childID);

            model.MonthlyGroups = getMonthlyGroups(model.ActiveChild);

            return model;
        }


        internal List<MonthlyGroupListItem> getMonthlyGroups(int childID) {

            var ret = new List<MonthlyGroupListItem>();

            var @case = _context.Cases.Where(x => x.PatientID == childID).FirstOrDefault();
            if (@case == null) {
                return ret;
            }

            if (@case.Periods.Count == 0) {
                return ret;
            }

            int viewableMonths = AppService.Current.Settings.ViewableMonths;

            var data = @case.Periods.OrderByDescending(x => x.StartDate).Take(viewableMonths).ToList();

            foreach (var d in data) {
                var item = new MonthlyGroupListItem();
                item.GroupID = d.FirstDayOfMonth.ToString("yyyyMMdd");
                item.MonthDate = d.FirstDayOfMonth;
                item.Hours = getMonthlyHours(d);
                ret.Add(item);
            }

            return ret;

        }


        internal List<MonthlyGroupHoursListItem> getMonthlyHours(Domain2.Cases.MonthlyPeriod period) {

            var ret = new List<MonthlyGroupHoursListItem>();

            var hours = period.Hours.OrderBy(x => x.Date).ThenBy(x => x.StartTime).ToList();

            foreach (var h in hours) {

                var item = new MonthlyGroupHoursListItem();

                item.Date = h.Date;
                item.ID = h.ID;
                item.IsApproved = h.ParentApprovalID.HasValue ? true : false;
                item.IsReported = h.ParentReported;
                item.ProviderFirstName = h.Provider.FirstName;
                item.ProviderID = h.ProviderID;
                item.ProviderLastName = h.Provider.LastName;
                item.Service = h.Service?.Name;
                item.ServiceID = h.ServiceID;
                item.TimeIn = new DateTime(h.Date.Year, h.Date.Month, h.Date.Day).AddMinutes(h.StartTime.Minutes).AddHours(h.StartTime.Hours);
                item.TimeOut = new DateTime(h.Date.Year, h.Date.Month, h.Date.Day).AddMinutes(h.EndTime.Minutes).AddHours(h.EndTime.Hours);

                ret.Add(item);
            }

            return ret;

        }


        internal int getActiveChild(HomeVM model, int? childID) {

            // default to first child in list
            var child = model.Children.FirstOrDefault();

            if (childID.HasValue) {
                // ensure it's in the child list
                var matchedChild = model.Children.Where(x => x.ID == childID.Value).FirstOrDefault();
                if (matchedChild != null) {
                    child = matchedChild;
                }
            }

            if (child == null) {
                throw new ArgumentOutOfRangeException("ChildID", "Child not found");
            }

            return child.ID;
        }

        internal List<ChildListItem> getChildren() {

            var currentUserID = AppService.Current.User.ID;
            var list = new List<ChildListItem>();
            var patients = _context.PatientPortalLogins.Find(currentUserID).Patients;

            foreach (var patient in patients) {

                list.Add(new ChildListItem()
                {
                    ID = patient.ID,
                    Name = patient.FirstName + ' ' + patient.LastName
                });
            }

            return list;
        }


        

        
        internal void ApproveHours(int patientID, DateTime firstDay) {

            var patient = _context.Patients.Find(patientID);
            var @case = patient.ActiveCase;
            var period = @case.GetPeriod(firstDay.Year, firstDay.Month);
            var approval = new Domain2.Cases.ParentApproval();
            period.ParentApprovals.Add(approval);

            approval.DateApproved = DateTime.Now;
            approval.ParentLoginID = AppService.Current.User.ID;

            _context.SaveChanges();

            var unapprovedHours = period.Hours.Where(x => x.ParentApprovalID == null);

            foreach (var h in unapprovedHours) {
                h.ParentApprovalID = approval.ID;
            }

            _context.SaveChanges();
            
        }

        internal void LogHours(int hoursID, string comment) {

            var logItem = new Domain2.Hours.ReportLogItem();

            logItem.HoursID = hoursID;
            logItem.LoginID = AppService.Current.User.ID;
            logItem.Message = comment;

            _context.HoursReportLogItems.Add(logItem);
            _context.SaveChanges();

        }

        internal void ReportHours(int hoursID, string comment)
        {
            var reportedHours = _context.Hours.Find(hoursID);
            reportedHours.ParentReported = true;

            _context.SaveChanges();

            var smtpInfo = _contextOld.SMTPAccounts.Where(x => x.AccountName == "Primary").SingleOrDefault();
            if(smtpInfo == null) { throw new ArgumentNullException("Primary SMTP Account info has not been configured."); }
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

            string subjectText = "Hours Reported - " + reportedHours.Case.Patient.CommonName;
            string messageFormat =
@"Hours Reported
Patient: {0}
{1}

{2} {3} - {4}
Provider: {5} {6}, {7}

Comments:
{8}";

            string messageText = String.Format(messageFormat,
                reportedHours.Case.Patient.CommonName,
                reportedHours.Case.Patient.Email,
                reportedHours.Date.ToString("D"),
                reportedHours.StartTime.ToString("hh\\:mm"),
                reportedHours.EndTime.ToString("hh\\:mm"),
                reportedHours.Provider.FirstName,
                reportedHours.Provider.LastName,
                reportedHours.Service.Name,
                comment);

            string mailToAddress = System.Configuration.ConfigurationManager.AppSettings["ReportHoursMailtoAddress"];

            AABC.DomainServices.Email.SMTP.Send(smtpAccount, subjectText, messageText, mailToAddress);

            

        }
        


        private Data.V2.CoreContext _context;
        private Data.Models.CoreEntityModel _contextOld;

        public HomeService() {
            _context = AppService.Current.Data.Context;
            _contextOld = AppService.Current.Data.OldContext;
        }

        public HomeService(Data.V2.CoreContext context, Data.Models.CoreEntityModel contextOld) {
            _context = context;
            _contextOld = contextOld;
        }

    }
}