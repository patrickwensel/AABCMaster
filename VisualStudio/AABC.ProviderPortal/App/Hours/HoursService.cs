using AABC.Data.V2;
using AABC.Domain2.Integrations.Catalyst;
using AABC.DomainServices.HoursResolution;
using AABC.ProviderPortal.App.Hours.Models;
using AABC.ProviderPortal.Reports;
using DevExpress.Web.Mvc;
using Dymeng.DocuSign;
using Dymeng.DocuSign.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace AABC.ProviderPortal.App.Hours
{
    public class HoursService
    {
        private readonly CoreContext _context;

        public HoursService() : this(AppService.Current.Context) { }

        public HoursService(CoreContext context)
        {
            _context = context;
        }


        public bool IsFinalized(int caseID, int providerID, DateTime date)
        {
            var period = _context.Cases.Find(caseID).GetPeriod(date.Year, date.Month);
            return period.IsProviderFinalized(providerID);
        }


        public void DeleteHours(int hoursID)
        {
            HoursRemovalService.DeleteHours(hoursID, _context);
        }


        public void CommitPendingHours(int caseID, int providerID)
        {
            _context.Hours
                .Where(x => x.CaseID == caseID
                        && x.ProviderID == providerID
                        && x.Status == Domain2.Hours.HoursStatus.Pending)
                .ToList()
                .ForEach(x => x.Status = Domain2.Hours.HoursStatus.ComittedByProvider);
            _context.SaveChanges();
        }


        public IEnumerable<HoursDownloadItem> GetHoursDownloadItems(int providerID, int caseID, int month, int year)
        {
            var results = _context.Database.SqlQuery<HoursDownloadItem>(
                "[dbo].GetHoursForDownload @ProviderID, @CaseID, @Month, @Year",
                new SqlParameter("@ProviderID", providerID),
                new SqlParameter("@CaseID", caseID),
                new SqlParameter("@Month", month),
                new SqlParameter("@Year", year)
                );
            return results.ToList();
        }


        public GridViewSettings GetHoursDownloadSettings(IEnumerable<HoursDownloadItem> data, int caseID, DateTime targetDate)
        {
            var patientName = _context.Cases.Find(caseID).Patient.CommonName.Replace(' ', '_');
            var s = new GridViewSettings
            {
                Name = "gvDownloadCurrentHours"
            };
            s.Settings.ShowFilterRow = false;
            s.SettingsBehavior.AllowSort = false;
            s.SettingsBehavior.AllowGroup = false;
            s.SettingsBehavior.AllowFocusedRow = false;
            s.SettingsBehavior.AllowSelectSingleRowOnly = true;
            s.SettingsExport.ExportSelectedRowsOnly = false;
            s.SettingsExport.FileName = "Hours_" + patientName + "_" + targetDate.ToString("yyyy-MM") + ".xlsx";

            s.Columns.Add(col =>
            {
                col.Name = "StatusName";
                col.Caption = "Status";
            });

            s.Columns.Add(col =>
            {
                col.Name = "HoursDate";
                col.Caption = "Date";
            });

            s.Columns.Add(col =>
            {
                col.Name = "HoursTimeIn";
                col.Caption = "Time In";
            });

            s.Columns.Add(col =>
            {
                col.Name = "HoursTimeOut";
                col.Caption = "Time Out";
            });

            s.Columns.Add(col =>
            {
                col.Name = "HoursTotal";
                col.Caption = "Hours Total";
            });

            s.Columns.Add(col =>
            {
                col.Name = "ServiceCode";
                col.Caption = "Service Code";
            });

            s.Columns.Add(col =>
            {
                col.Name = "HoursNotes";
                col.Caption = "Notes";
            });

            s.Columns.Add(col =>
            {
                col.Name = "ExtendedNotes";
                col.Caption = "Extended Notes";
            });

            return s;
        }


        internal IEnumerable<TimesheetPreloadEntry> GetCatalystPreloads(int caseID, int providerID, DateTime cutoffDate)
        {
            var catalystHours = _context.CatalystPreloadEntries
                .Where(x => x.MappedCaseID == caseID
                            && x.MappedProviderID == providerID
                            && x.IsResolved == false
                            && x.Date >= cutoffDate)
                .ToList();

            // remove any finalized
            var finalizedMonths = _context.HoursFinalizations
                    .Where(x => x.Period.FirstDayOfMonth >= cutoffDate
                            && x.Period.CaseID == caseID
                            && x.ProviderID == providerID)
                    .Select(x => x.Period.FirstDayOfMonth)
                    .ToList();

            foreach (var fm in finalizedMonths)
            {
                var toRemove = catalystHours.Where(x => x.Date >= fm && x.Date < fm.AddMonths(1)).ToList();
                for (int i = 0; i < toRemove.Count; i++)
                {
                    catalystHours.Remove(toRemove[i]);
                }
            }

            return catalystHours;
        }


        public int GetCountOfHours(int caseID, int providerID, DateTime firstDayOfMonth)
        {
            var provider = _context.Providers.Find(providerID);
            var startDate = firstDayOfMonth;
            var endDate = firstDayOfMonth.AddMonths(1).AddDays(-1);
            return provider.Hours
                        .Where(
                            x => x.CaseID == caseID &&
                            x.ProviderID == providerID &&
                            x.Date >= startDate &&
                            x.Date <= endDate)
                        .Count();
        }


        public Envelope FinalizeMonthSignature(int caseID, DateTime firstDayOfMonth, string returnURL)
        {
            var startDate = firstDayOfMonth;
            var endDate = startDate.AddMonths(1).AddSeconds(-1);
            var provider = _context.Providers.Find(Global.Default.User().ProviderID.Value);
            var report = ReportService.GetPatientHoursReport(caseID, startDate, endDate, provider.ID);

            using (var memstream = new MemoryStream())
            {
                report.ExportToPdf(memstream);
                var dsClient = new DocuSignClient();
                var dsSettings = AppService.Current.Settings.DocuSignProviderFinalize;
                var config = new ClientConfig()
                {
                    AuthConfig = new Dymeng.DocuSign.AuthConfig()
                    {
                        Host = dsSettings.AuthHost,
                        IntegratorKey = dsSettings.AuthIntegratorKey,
                        OAuthBasePath = dsSettings.AuthOAuthBasePath,
                        PrivateKeyPath = dsSettings.AuthPrivateKeyPath,
                        UserID = dsSettings.AuthUserID
                    },
                    DocumentFilename = dsSettings.DocumentFilename,
                    EmailSubject = dsSettings.EmailSubject,
                    ReturnURL = returnURL,
                    SignerEmail = provider.Email ?? dsSettings.DefaultSignerEmail,
                    SignerID = provider.ID.ToString(),
                    SignerName = provider.FirstName + " " + provider.LastName,
                    DocumentBytes = memstream.ToArray()
                };
                return dsClient.GetSignatureRedirect(config);
            }
        }


    }
}