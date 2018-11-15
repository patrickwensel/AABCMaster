using AABC.Data.V2;
using AABC.Web.App.Providers.Models;
using AABC.Web.Models.Providers;
using AABC.Web.Reports;
using AABC.Web.Repos;
using DevExpress.Web.Mvc;
using Dymeng.Framework;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace AABC.Web.App.Providers
{
    public class ProviderService
    {

        private readonly CoreContext Context;
        private readonly HoursRepo HoursRepository;

        public ProviderService()
        {
            Context = AppService.Current.DataContextV2;
            HoursRepository = new HoursRepo();
        }


        public string GetMimeType(string fileName)
        {
            var extension = fileName.Substring(fileName.LastIndexOf('.'));
            switch (extension)
            {
                case ".pdf":
                    return "application/pdf";
                default:
                    return "application/octet";
            }
        }


        public void PayrollExportXlxs(DateTime targetDate, bool commit, PayrollFilter filter, out GridViewSettings gridViewSettings, out IEnumerable<PayrollGridItemVM> data)
        {
            data = HoursRepository.GetPayablesByPeriod(new DateTime(targetDate.Year, targetDate.Month, 1), filter);
            gridViewSettings = new GridViewSettings
            {
                Name = "gvPayrollOverviewGrid",
                KeyFieldName = "ID"
            };
            gridViewSettings.SettingsBehavior.AllowSort = false;
            gridViewSettings.SettingsBehavior.AllowGroup = false;
            gridViewSettings.SettingsBehavior.AllowFocusedRow = false;
            gridViewSettings.SettingsBehavior.AllowSelectSingleRowOnly = true;
            gridViewSettings.Settings.ShowFilterRow = false;
            gridViewSettings.CallbackRouteValues = new { Action = "PayrollGridCallback" };
            gridViewSettings.SettingsExport.ExportSelectedRowsOnly = false;
            gridViewSettings.SettingsExport.FileName = "Payables_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xlsx";

            gridViewSettings.Columns.Add(col =>
            {
                col.Name = "PayrollID";
                col.FieldName = "PayrollID";
                col.Caption = "PayrollID";
            });

            gridViewSettings.Columns.Add(col =>
            {
                col.Name = "colFirstName";
                col.FieldName = "FirstName";
                col.Caption = "First Name";
            });
            gridViewSettings.Columns.Add(col =>
            {
                col.Name = "colLastName";
                col.FieldName = "LastName";
                col.Caption = "Last Name";
            });
            gridViewSettings.Columns.Add(col =>
            {
                col.Name = "colHours";
                col.FieldName = "Hours";
                col.Caption = "Hours";
                col.Width = 80;
            });
            gridViewSettings.Columns.Add(col =>
            {
                col.Name = "colEntriesMissingCatalystData";
                col.FieldName = "EntriesMissingCatalystData";
                col.Caption = "Entries Missing Catalyst Data";
            });
            try
            {
                if (commit)
                {
                    ReportService.GeneratePayrollReport(targetDate, filter);
                }
                return;
            }
            catch (Exception e)
            {
                Exceptions.Handle(e, Global.GetWebInfo());
                throw e;
            }
        }


        public int GenerateNewPayrollID()
        {
            var r = new Random();
            var v = 0;
            var valid = false;
            // get list of provider IDs and payroll IDs
            var existingIDs = Context.Providers.Select(x => x.ID).ToList();
            existingIDs.AddRange(Context.Providers.Where(x => x.PayrollID != null).Select(x => x.PayrollID.Value).ToList());
            while (!valid)
            {
                v = r.Next(1000, 10000);
                if (!existingIDs.Contains(v))
                {
                    valid = true;
                }
            }
            return v;
        }


        public MemoryStream GetHoursReports(int caseID, int providerID, DateTime startPeriod, DateTime endPeriod)
        {
            var startDate = new DateTime(startPeriod.Year, startPeriod.Month, 1);
            var endDate = new DateTime(endPeriod.Year, endPeriod.Month, 1).AddMonths(1).AddDays(-1);
            if (startDate > endDate)
            {
                throw new InvalidOperationException("Start Date greater than End Date");
            }
            var tempDir = ConfigurationManager.AppSettings["TempDirectory"];
            var reportBaseName = "ProviderHoursReport_";
            var currentPeriod = startDate;
            var files = new List<string>();
            while (currentPeriod < endDate)
            {
                var report = ReportService.GetPatientHoursReportWithSignLine(caseID, currentPeriod, currentPeriod.AddMonths(1).AddDays(-1), providerID);
                using (var stream = new MemoryStream())
                {
                    string filename = Path.Combine(tempDir, reportBaseName + caseID + "_" + providerID + "_" + currentPeriod.ToString("yyyyMMddHHmmss") + "_pvdlrep.pdf");
                    if (File.Exists(filename))
                    {
                        File.Delete(filename);
                    }
                    report.ExportToPdf(filename);
                    files.Add(filename);
                }
                currentPeriod = currentPeriod.AddMonths(1);
            }

            // merge the files
            string targetPath = Path.Combine(tempDir, reportBaseName + providerID + "_" + caseID + "_" + startDate.ToString("yyyyMMddHHmmss") + ".pdf");
            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }

            MergePDFs(targetPath, files.ToArray());
            using (var stream = new MemoryStream())
            {
                byte[] docBytes = File.ReadAllBytes(targetPath);
                stream.Write(docBytes, 0, docBytes.Length);
                return stream;
            }
        }


        public bool IsNameCollision(int? id, string firstName, string lastName)
        {
            var entities = Context.Providers.Where(x => x.FirstName == firstName && x.LastName == lastName).ToList();
            if (entities.Count > 1)
            {
                return true;
            }
            else if (entities.Count == 0)
            {
                return false;
            }
            else
            {
                // there's only one, check to see if it's ours
                return !(entities.Single().ID == id);
            }
        }


        private void MergePDFs(string targetPath, string[] files)
        {
            using (var targetDoc = new PdfDocument())
            {
                foreach (string pdf in files)
                {
                    using (var pdfDoc = PdfReader.Open(pdf, PdfDocumentOpenMode.Import))
                    {
                        for (int i = 0; i < pdfDoc.PageCount; i++)
                        {
                            targetDoc.AddPage(pdfDoc.Pages[i]);
                        }
                    }
                }
                targetDoc.Save(targetPath);
            }
        }

    }
}