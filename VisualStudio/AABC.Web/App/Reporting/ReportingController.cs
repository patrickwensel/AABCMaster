using AABC.Web.App.Insurance;
using Dymeng.Framework.Web.Mvc.Views;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Mime;
using System.Web.Mvc;


namespace AABC.Web.App.Reporting
{
    public class ReportingController : Dymeng.Framework.Web.Mvc.Controllers.ContentBaseController
    {

        public ActionResult Reports()
        {
            ViewBag.Push = new ViewModelBase(PushState, "/Reporting/Reports", "Reporting");

            var model = new Models.ReportSelectorVM();

            model.AvailableDates = service.GetAvailableDates();
            model.DefaultDate = service.DefaultSelectedDate();
            model.AvailableInsurance = new Repositories.PatientRepository().GetInsuranceList();

            return GetView("ReportSelector", model);
        }


        public ActionResult Authorizations()
        {
            ViewBag.Push = new ViewModelBase(PushState, "/Reporting/Authorizations", "Authorizations");
            return GetView("AuthorizationReports");
        }



        [Route("Reporting/Authorizations/InvalidCaseAuths")]
        public ActionResult AuthsInvalidCaseAuths()
        {
            var data = service.GetInvalidCaseAuths();
            return PartialView("AuthsUnmatchedContainer", data);
        }


        [Route("Reporting/Authorizations/InvalidCaseAuthsGrid")]
        public ActionResult AuthsInvalidCaseAuthsGrid()
        {
            var data = service.GetInvalidCaseAuths();
            return PartialView("AuthsUnmatchedGrid", data);
        }

        [Route("Reporting/InvalidCaseAuthDetail")]
        public ActionResult InvalidCaseAuthDetail(string CaseAndAuthIDPair)
        {

            int caseID = int.Parse(CaseAndAuthIDPair.Split('|')[0]);
            int authID = int.Parse(CaseAndAuthIDPair.Split('|')[1]);

            var model = service.GetInvalidCaseDetail(caseID, authID);
            return PartialView("AuthsUnmatchedDetail", model);
        }

        [Route("Reporting/Authorizations/BreakdownByCase")]
        public ActionResult AuthsBreakdownsByCase()
        {
            return PartialView("AuthsBreakdownByCase");
        }


        [Route("Reporting/Authorizations/BreakdownByCaseGrid")]
        public ActionResult AuthsBreakdownsByCaseGrid(DateTime startDate, DateTime endDate, int caseID)
        {
            return PartialView("AuthsBreakdownByCaseGrid");
        }


        [Route("Reporting/Billing/BillingExport")]
        public ActionResult BillingBillingExport(DateTime period)
        {

            var data = service.GetBillingBillingExportData(period);
            var settings = service.GetBillingBillingExportGridViewSettings();
            return DevExpress.Web.Mvc.GridViewExtension.ExportToXlsx(settings, data);

        }

        [Route("Reporting/Billing/ProvidersWithoutHours")]
        public ActionResult BillingProvidersWithoutHours(DateTime period)
        {

            var data = service.GetBillingProvidersWithoutHoursData(period);
            var settings = service.GetBillingProvidersWithoutHoursSettings();
            return DevExpress.Web.Mvc.GridViewExtension.ExportToXlsx(settings, data);

        }


        [Route("Reporting/Billing/UnfinalizedProviders")]
        public ActionResult BillingUnfinalizedProviders(DateTime period)
        {

            var data = service.GetBillingUnfinalizedProvidersData(period);
            var settings = service.GetBillingUnfinalizedProvidersSettings();
            return DevExpress.Web.Mvc.GridViewExtension.ExportToXlsx(settings, data);

        }


        [Route("Reporting/Catalyst/ProvidersWithoutData")]
        public ActionResult CatalystProvidersWithoutData(DateTime period)
        {

            var data = service.GetCatalystProvidersWithoutDataData(period);
            var settings = service.GetCatalystProvidersWithoutDataSettings();
            return DevExpress.Web.Mvc.GridViewExtension.ExportToXlsx(settings, data);

        }

        [Route("Reporting/Payroll/PayrollExport")]
        public ActionResult PayrollPayrollExport(DateTime period)
        {

            var data = service.GetPayrollPayrollExportData(period);
            var settings = service.GetPayrollPayrollExportSettings();
            return DevExpress.Web.Mvc.GridViewExtension.ExportToXlsx(settings, data);

        }

        [Route("Reporting/Approvals/GuardianApprovalExists")]
        public ActionResult GurardianApprovalExists(DateTime period, int insuranceID)
        {
            DateTime startDate = period;
            DateTime endDate = period.AddMonths(1).AddDays(-1);

            bool exists = AppService.Current.DataContextV2.Hours
                .Where(x => x.Case.Patient.InsuranceID == insuranceID
                && x.Date > startDate
                && x.Date < endDate)
                .Select(x => x.ParentApproval)
                .Where(x => x != null).Any();

            return Json(exists);
        }

        [Route("Reporting/Approvals/GuardianApproval")]
        public ActionResult GuardianApprovalReport(DateTime period, int insuranceID)
        {

            DateTime startDate = period;
            DateTime endDate = period.AddMonths(1).AddDays(-1);

            var approvals = AppService.Current.DataContextV2.Hours
                .Where(x => x.Case.Patient.InsuranceID == insuranceID
                && x.Date > startDate
                && x.Date < endDate)

                .Select(x => x.ParentApproval).Distinct().ToList();


            //var approvals = approvalsTmp.GetRange(1, 1);
            //approvals.AddRange(approvalsTmp.GetRange(3, 1));

            string requestGUID = Guid.NewGuid().ToString();
            string tmpDir = AppService.Current.Settings.TempDirectory + "\\GuardianApprovalReports";
            string requestDir = tmpDir + "\\" + requestGUID;
            Directory.CreateDirectory(requestDir);

            //clean up directory
            Directory.GetFiles(tmpDir)
                .Select(f => new FileInfo(f))
                .Where(f => f.CreationTime < DateTime.Now.AddHours(-6))
                .ToList()
                .ForEach(f => f.Delete());

            Directory.GetDirectories(tmpDir)
                .Select(d => new DirectoryInfo(d))
                .Where(d => d.CreationTime < DateTime.Now.AddHours(-6))
                .ToList()
                .ForEach(f => f.Delete(true));


            foreach (var approval in approvals)
            {
                if (approval == null) { continue; }
                var report = Web.Reports.ReportService.GetParentApprovalReport(approval);
                var patient = approval.Hours.First().Case.Patient;

                using (var fileStream = System.IO.File.Create(
                    requestDir + "\\"
                    + patient.LastName + "_" + patient.FirstName
                    + "_" + approval.ID + ".pdf"))
                {
                    report.ExportToPdf(fileStream);
                }

            }

            ZipFile.CreateFromDirectory(requestDir, requestDir + ".zip", CompressionLevel.Fastest, false);
            return File(requestDir + ".zip", "application/zip", "HoursApproval_" + startDate.Year + "-" + startDate.Month + ".zip");


        }

        [Route("Reporting/Insurance/AuthRuleExport")]
        public ActionResult InsuranceAuthRule(int insuranceId)
        {
            var data = svcInsurance.GetAuthRules(insuranceId).OrderBy(i => i.InsuranceName).ThenBy(i => i.ProviderType).ThenBy(i => i.Service);
            var settings = service.GetInsuranceAuthRuleExportSettings();
            return DevExpress.Web.Mvc.GridViewExtension.ExportToXlsx(settings, data);

        }

        [Route("Reporting/Users/StaffComparison")]
        public ActionResult UsersStaffComparison()
        {
            var data = repoUser.GetOfficeStaffUsers();
            var settings = service.GetUserStaffComparisonExportSettings();
            return DevExpress.Web.Mvc.GridViewExtension.ExportToXlsx(settings, data);

        }


        [Route("Reporting/Dump")]
        public ActionResult DumpTables()
        {
            var name = $"DumpFile_{DateTime.Now.ToString("yyyy-MM-dd")}.xlsx";
            var tables = service.DumpTables();
            return File(tables, MediaTypeNames.Application.Octet, name);
        }

        [Route("Reporting/LatestNotesAndTasks")]
        public ActionResult LatestNotesAndTasks() {
            var name = $"LatestNotesAndTasks_{DateTime.Now.ToString("yyyy-MM-dd")}.xlsx";
            var file = service.GetLatestNotesAndTasksFile();
            return File(file, MediaTypeNames.Application.Octet, name);
        }

        [Route("Reporting/ProviderCaseloads")]
        public ActionResult ProviderCaseloads() {
            var name = $"ProviderCaseloads_{DateTime.Now.ToString("yyyy-MM-dd")}.xlsx";
            var file = service.GetProviderCaseloadsFile();
            return File(file, MediaTypeNames.Application.Octet, name);
        }

        [Route("Reporting/AuthorizationUtilization")]
        public ActionResult AuthorizationUtilization()
        {
            var name = $"AuthorizationUtilization_{DateTime.Now.ToString("yyyy-MM-dd")}.xlsx";
            var file = service.GetAuthorizationUtilizationFile();
            return File(file, MediaTypeNames.Application.Octet, name);
        }

        public ActionResult ProviderAppUtilizationReport()
        {
            ViewBag.Push = new ViewModelBase(PushState, "/Reporting/Reports", "Reporting");
            return GetView("ProviderAppUtilizationReport");
        }

        public JsonResult ProviderAppUtilizationReportData(DateTime startDate, DateTime endDate)
        {
            var cases = service.GetCasePercentageEnteredViaProviderApp(startDate, endDate);
            var providers = service.GetPortalAppPercentageEnteredViaProviderApp(startDate, endDate);

            return this.CamelCaseJson(new { cases, providers }, JsonRequestBehavior.AllowGet);
        }

        private ReportingService service;
        private InsuranceService svcInsurance;
        private App.Account.UserRepository repoUser;
        public ReportingController()
        {
            this.service = new ReportingService();
            this.svcInsurance = new InsuranceService();
            this.repoUser = new App.Account.UserRepository();
        }


    }
}