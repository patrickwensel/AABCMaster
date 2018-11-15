using AABC.Domain2.Providers;
using AABC.DomainServices.TableExporter;
using AABC.Web.App.Hours.Models;
using AABC.Web.App.Reporting.Models;
using AABC.Web.Models.Providers;
using DevExpress.Web.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace AABC.Web.App.Reporting
{
    class ReportingService
    {



        internal List<Data.V2.DTOs.AppliedAuthorizationAndInsuranceMismatchItem> GetInvalidCaseAuths()
        {

            return _context.GetAppliedAuthsAndInsuranceMismatches().ToList();
        }



        internal Models.AuthsUnmatchedDetailVM GetInvalidCaseDetail(int caseID, int authID)
        {

            var model = new Models.AuthsUnmatchedDetailVM();

            var c = _context.Cases.Find(caseID);

            model.Authorizations = c.GetActiveAuthorizations();
            model.MatchRules = c.Patient.Insurance?.AuthorizationMatchRules?.ToList();

            model.AuthCode = model.Authorizations.Where(x => x.AuthorizationCodeID == authID).Single().AuthorizationCode.Code;
            model.InsuranceName = c.Patient.Insurance?.Name;
            model.PatientName = c.Patient.CommonName;

            model.CaseID = c.ID;
            model.InsuranceID = c.Patient.InsuranceID;

            return model;
        }


        public List<AvailableDate> GetAvailableDates()
        {
            return hoursRepo.GetScrubAvailableDates();
        }

        public AvailableDate DefaultSelectedDate()
        {
            return new AvailableDate()
            {
                Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)
            };
        }







        internal object GetBillingProvidersWithoutHoursData(DateTime period)
        {
            throw new NotImplementedException();
        }

        internal GridViewSettings GetBillingProvidersWithoutHoursSettings()
        {
            throw new NotImplementedException();
        }



        internal List<UnfinalizedProviderExportItemVM> GetBillingUnfinalizedProvidersData(DateTime period)
        {
            return hoursRepo.GetUnfinalizedProviderExportItems(period);
        }

        internal GridViewSettings GetBillingUnfinalizedProvidersSettings()
        {

            var s = new GridViewSettings();

            s.Name = "gvDownloadUnfinalizedProviders";
            s.SettingsExport.FileName = "UnfinalizedProviders_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";

            var col = s.Columns.Add("colFirstName");
            col.FieldName = "FirstName";
            col.Caption = "First Name";

            col = s.Columns.Add("colLastName");
            col.FieldName = "LastName";
            col.Caption = "Last Name";

            col = s.Columns.Add("colEmail");
            col.FieldName = "Email";
            col.Caption = "Email";

            col = s.Columns.Add("colHoursCount");
            col.FieldName = "HoursCount";
            col.Caption = "Hours Count";

            col = s.Columns.Add("colHasFinalization");
            col.FieldName = "HasFinalization";
            col.Caption = "Has Finalization";

            return s;
        }


        internal List<Domain.Catalyst.NoDataByProviderAndCase> GetCatalystProvidersWithoutDataData(DateTime period)
        {
            return new Data.Services.CatalystService().GetNoDataByProviderAndCaseItems(period);
        }

        internal GridViewSettings GetCatalystProvidersWithoutDataSettings()
        {

            var s = new GridViewSettings();

            s.Name = "gvExportCatalystProvidersMissingHoursReport";
            s.SettingsExport.FileName = "NoCatalystData_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";

            var col = s.Columns.Add("colProviderFirstName");
            col.FieldName = "ProviderFirstName";
            col.Caption = "Provider First Name";

            col = s.Columns.Add("colProviderLastName");
            col.FieldName = "ProviderLastName";
            col.Caption = "Provider Last Name";

            col = s.Columns.Add("colPatientFirstName");
            col.FieldName = "PatientFirstName";
            col.Caption = "Patient First Name";

            col = s.Columns.Add("colPatientLastName");
            col.FieldName = "PatientLastName";
            col.Caption = "Patient Last Name";

            col = s.Columns.Add("colProviderPhone");
            col.FieldName = "ProviderPhone";
            col.Caption = "Provider Phone";

            col = s.Columns.Add("colProviderEmail");
            col.FieldName = "ProviderEmail";
            col.Caption = "Provider Email";

            col = s.Columns.Add("colDates");
            col.FieldName = "Dates";
            col.Caption = "Dates";

            return s;

        }



        internal List<PayrollGridItemVM> GetPayrollPayrollExportData(DateTime period)
        {
            return hoursRepo.GetPayablesByPeriod(period);
        }

        internal GridViewSettings GetPayrollPayrollExportSettings()
        {

            var s = new GridViewSettings();

            s.Name = "gvPayrollOverviewGrid";
            s.SettingsExport.FileName = "Payables_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";

            var col = s.Columns.Add("colFirstName");
            col.FieldName = "FirstName";
            col.Caption = "First Name";

            col = s.Columns.Add("colLastName");
            col.FieldName = "LastName";
            col.Caption = "Last Name";

            col = s.Columns.Add("colHours");
            col.FieldName = "Hours";
            col.Caption = "Hours";
            col.Width = 80;

            return s;

        }




        public List<MHBExportItemVM> GetBillingBillingExportData(DateTime period)
        {
            return hoursRepo.GetMHBExportItems(period, null);
        }

        public GridViewSettings GetBillingBillingExportGridViewSettings()
        {

            var s = new GridViewSettings();

            s.Name = "rptgvBillingBillingExport";
            s.SettingsExport.FileName = "MBH-Billing_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";

            var col = s.Columns.Add("colPatientFN");
            col.FieldName = "PatientFN";

            s.Columns.Add("PatientLN");
            s.Columns.Add("ProviderFN");
            s.Columns.Add("ProviderLN");

            col = s.Columns.Add("colPatientID");
            col.FieldName = "PatientID";
            col.Caption = "Patient ID";

            col = s.Columns.Add("colProviderID");
            col.FieldName = "ProviderID";
            col.Caption = "Provider ID";

            col = s.Columns.Add("AuthorizedProviderID");

            col = s.Columns.Add("colSupervisingBCBAID");
            col.FieldName = "SupervisingBCBAID";
            col.Caption = "Supervising BCBA ID";

            col = s.Columns.Add("colIsBCBATimesheet");
            col.FieldName = "IsBCBATimesheet";
            col.Caption = "Is BCBA Timesheet";

            col = s.Columns.Add("colDateOfService");
            col.FieldName = "DateOfService";
            col.Caption = "Date Of Service";

            col = s.Columns.Add("colStartTime");
            col.FieldName = "StartTime";
            col.Caption = "Start Time";
            col.PropertiesEdit.DisplayFormatString = "t";

            col = s.Columns.Add("colEndTime");
            col.FieldName = "EndTime";
            col.Caption = "End Time";
            col.PropertiesEdit.DisplayFormatString = "t";

            col = s.Columns.Add("colTotalTime");
            col.FieldName = "TotalTime";
            col.Caption = "Total Time";

            col = s.Columns.Add("colServiceCode");
            col.FieldName = "ServiceCode";
            col.Caption = "Service Code";

            col = s.Columns.Add("colPlaceOfService");
            col.FieldName = "PlaceOfService";
            col.Caption = "Place Of Service";

            col = s.Columns.Add("colPlaceOfServiceID");
            col.FieldName = "PlaceOfServiceID";
            col.Caption = "Place Of Service ID";

            return s;

        }


        internal GridViewSettings GetInsuranceAuthRuleExportSettings()
        {

            var s = new GridViewSettings();

            s.Name = "gvInsuranceAuthRuleGrid";
            s.SettingsExport.FileName = "InsuranceAuthRules.xlsx";

            var col = s.Columns.Add("colInsurance");
            col.FieldName = "InsuranceName";
            col.Caption = "Insurance";

            col = s.Columns.Add("colProviderType");
            col.FieldName = "ProviderType";
            col.Caption = "Provider";

            col = s.Columns.Add("colService");
            col.FieldName = "Service";
            col.Caption = "Service";

            col = s.Columns.Add("colInitialStats");
            col.FieldName = "InitialStats";
            col.Caption = "Initial";

            col = s.Columns.Add("colFinalStats");
            col.FieldName = "FinalStats";
            col.Caption = "Final";

            return s;

        }



        internal GridViewSettings GetUserStaffComparisonExportSettings()
        {

            var s = new GridViewSettings();

            s.Name = "gvUserStaffComparisonGrid";
            s.SettingsExport.FileName = "UserStaffComparison.xlsx";

            var col = s.Columns.Add("colStaffId");
            col.FieldName = "ID";
            col.Caption = "Staff Id";

            col = s.Columns.Add("colUsername");
            col.FieldName = "UserName";
            col.Caption = "Username";

            col = s.Columns.Add("colEmail");
            col.FieldName = "Email";
            col.Caption = "Email";

            col = s.Columns.Add("colFirstName");
            col.FieldName = "FirstName";
            col.Caption = "First Name";

            col = s.Columns.Add("colLastName");
            col.FieldName = "LastName";
            col.Caption = "Last Name";

            return s;

        }



        public byte[] GetProviderCaseloadsFile()
        {
            var exporter = new TableExporter(_context.Database.Connection.ConnectionString);
            return exporter.GetFile(new ExportDefinition
            {
                SelectStatement = "EXEC [webreports].[ProviderCaseloads]",
                FormatAsTable = true
            });
        }


        public byte[] GetLatestNotesAndTasksFile()
        {
            var exporter = new TableExporter(_context.Database.Connection.ConnectionString);
            return exporter.GetFile(new ExportDefinition
            {
                SelectStatement = "EXEC [webreports].[LatestTasksAndNotesByPatient]",
                FormatAsTable = true,
                Format = (ws, table) =>
                {
                    var r = ws.Cells[2, 4, table.Rows.Count + 1, 4];
                    r.Style.Numberformat.Format = "yyyy-mm-dd";
                }
            });
        }


        public byte[] GetAuthorizationUtilizationFile()
        {
            var exporter = new TableExporter(_context.Database.Connection.ConnectionString);
            return exporter.GetFile(new ExportDefinition
            {
                SelectStatement = "EXEC [webreports].[AuthorizationUtilization]",
                FormatAsTable = true
            });
        }


        public byte[] DumpTables()
        {
            Action<ExcelWorksheet, DataTable> format = (ws, table) =>
            {
                var r = ws.Cells[2, 2, table.Rows.Count + 1, 2];
                r.Style.Numberformat.Format = "yyyy-mm-dd HH:mm:ss";
            };
            var exporter = new TableExporter(_context.Database.Connection.ConnectionString);
            return exporter.GetFile(new ExportDefinition[] {
                new ExportDefinition
                {
                    SelectStatement = "SELECT * FROM Patients",
                    WorksheetName = "Patients",
                    TableRangeName = "Patients",
                    FormatAsTable = true,
                    Format = format
                },
                new ExportDefinition
                {
                    SelectStatement = "SELECT * FROM Referrals",
                    WorksheetName = "Referrals",
                    TableRangeName = "Referrals",
                    FormatAsTable = true,
                    Format = format
                },
                new ExportDefinition
                {
                    SelectStatement = "SELECT * FROM ProvidersDump",
                    WorksheetName = "Providers",
                    TableRangeName = "Providers",
                    FormatAsTable = true,
                    Format = format
                },
            });
        }

        public List<ProviderAppUtilizationReportItem> GetCasePercentageEnteredViaProviderApp(DateTime startDate,
            DateTime endDate)
        {
            endDate = endDate.AddDays(1);

            var results = (from hours in _context.Hours
                join providers in _context.Providers on hours.ProviderID equals providers.ID
                join users in _context.ProviderPortalUsers on providers.ID equals users.ProviderID
                where startDate <= hours.Date && hours.Date < endDate &&
                      providers.Status == ProviderStatus.Active &&
                      users.HasAppAccess
                group hours by new {hours.Date, hours.EntryApp}
                into g
                select new
                {
                    g.Key.Date,
                    App = g.Key.EntryApp,
                    TotalHours = g.Sum(h => h.TotalHours)
                }).ToList();


            return results
                .Where(r => r.App == "Provider App")
                .Select(r => new ProviderAppUtilizationReportItem
                {
                    Date = r.Date,
                    Value = Math.Round(r.TotalHours * 100 /
                            results.Where(r2 => r2.Date == r.Date).Sum(r2 => r2.TotalHours), 2)
                }).ToList();

        }
        public List<ProviderAppUtilizationReportItem> GetPortalAppPercentageEnteredViaProviderApp(DateTime startDate,
            DateTime endDate)
        {
            endDate = endDate.AddDays(1);

            var users = (from portalUser in _context.ProviderPortalUsers
                join provider in _context.Providers on portalUser.ProviderID equals provider.ID
                where provider.Status == ProviderStatus.Active
                select new
                {
                    portalUser.ID,
                    portalUser.DateCreated,
                    portalUser.HasAppAccess
                }).ToList();

            var total = users.Count;
            var count = users.Count(u => u.HasAppAccess && u.DateCreated <= startDate);

            var providers = new List<ProviderAppUtilizationReportItem>
            {
                // showing how many users have been in the system prior to start date
                new ProviderAppUtilizationReportItem
                {
                    Date = startDate,
                    Value = (decimal) Math.Round(count * 100.0 / total, 2)
                }
            };

            // adding amount of users added within the time-frame
            var addedGroups = users
                .Where(u => startDate < u.DateCreated && u.DateCreated < endDate && u.HasAppAccess)
                .GroupBy(u => u.DateCreated)
                .OrderBy(u => u.Key)
                .ToList();

            foreach (var group in addedGroups)
            {
                count += group.Count();
                providers.Add(new ProviderAppUtilizationReportItem
                {
                    Date = group.Key,
                    Value = (decimal) Math.Round(count * 100.0 / total, 2)
                });
            }

            providers.Add(new ProviderAppUtilizationReportItem
            {
                Date = endDate,
                Value = (decimal)Math.Round(count * 100.0 / total, 2)
            });

            return providers;
        }

        private Repos.HoursRepo hoursRepo;
        private Data.V2.CoreContext _context;

        public ReportingService()
        {
            hoursRepo = new Repos.HoursRepo();
            _context = AppService.Current.DataContextV2;
        }

    }
}