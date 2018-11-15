using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AABC.Web.App.Providers
{
    public class ProviderQueryService
    {


        private Data.V2.CoreContext _context;
        private Repositories.IProviderRepository _repository;

        public ProviderQueryService(Data.V2.CoreContext context, Repositories.IProviderRepository repository) {
            _context = AppService.Current.DataContextV2;
            _repository = repository;
        }



        public System.IO.MemoryStream GetHoursReports(int caseID, int providerID, DateTime startPeriod, DateTime endPeriod) {

            DateTime startDate = new DateTime(startPeriod.Year, startPeriod.Month, 1);
            DateTime endDate = new DateTime(endPeriod.Year, endPeriod.Month, 1).AddMonths(1).AddDays(-1);

            if (startDate > endDate) {
                throw new InvalidOperationException("Start Date greater than End Date");
            }

            var tempDir = System.Configuration.ConfigurationManager.AppSettings["TempDirectory"];

            string reportBaseName = "ProviderHoursReport_";

            DateTime currentPeriod = startDate;

            var files = new List<string>();

            while (currentPeriod < endDate) {

                var report = Reports.ReportService.GetPatientHoursReportWithSignLine(caseID, currentPeriod, currentPeriod.AddMonths(1).AddDays(-1), providerID);

                using (var stream = new System.IO.MemoryStream()) {
                    string filename = System.IO.Path.Combine(tempDir, reportBaseName + caseID + "_" + providerID + "_" + currentPeriod.ToString("yyyyMMddHHmmss") + "_pvdlrep.pdf");
                    if (System.IO.File.Exists(filename)) {
                        System.IO.File.Delete(filename);
                    }
                    report.ExportToPdf(filename);
                    files.Add(filename);
                }

                currentPeriod = currentPeriod.AddMonths(1);
            }

            // merge the files
            string targetPath = System.IO.Path.Combine(tempDir, reportBaseName + providerID + "_" + caseID + "_" + startDate.ToString("yyyyMMddHHmmss") + ".pdf");
            if (System.IO.File.Exists(targetPath)) {
                System.IO.File.Delete(targetPath);
            }

            mergePDFs(targetPath, files.ToArray());

            using (var stream = new System.IO.MemoryStream()) {
                byte[] docBytes = System.IO.File.ReadAllBytes(targetPath);
                stream.Write(docBytes, 0, docBytes.Length);
                return stream;
            }
        }



        public int GetNewPayrollID() {

            var r = new Random();
            int v = 0;
            bool valid = false;

            // get list of provider IDs and payroll IDs
            var existingIDs = _context.Providers.Select(x => x.ID).ToList();
            existingIDs.AddRange(_context.Providers.Where(x => x.PayrollID != null).Select(x => x.PayrollID.Value).ToList());

            while (!valid) {
                v = r.Next(1000, 10000);
                if (!existingIDs.Contains(v)) {
                    valid = true;
                }
            }
            return v;
        }











        private void mergePDFs(string targetPath, string[] files) {
            using (PdfSharp.Pdf.PdfDocument targetDoc = new PdfSharp.Pdf.PdfDocument()) {
                foreach (string pdf in files) {
                    using (PdfSharp.Pdf.PdfDocument pdfDoc = PdfSharp.Pdf.IO.PdfReader.Open(pdf, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import)) {
                        for (int i = 0; i < pdfDoc.PageCount; i++) {
                            targetDoc.AddPage(pdfDoc.Pages[i]);
                        }
                    }
                }
                targetDoc.Save(targetPath);
            }
        }









        public bool IsNameCollision(int? id, string firstName, string lastName) {

            var entities = _context.Providers
                .Where(
                    x => x.FirstName == firstName 
                    && x.LastName == lastName)
                .ToList();

            if (entities.Count > 1) {
                return true;
            }

            if (entities.Count == 0) {
                return false;
            }

            // there's only one, check to see if it's ours
            if (entities.Single().ID == id) {
                return false;
            } else {
                return true;
            }
        }


    }
}