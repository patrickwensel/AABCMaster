using DevExpress.XtraPrinting.Native;
using System;

namespace AABC.ProviderPortal.Reports
{
    public class ReportService
    {

        public static PatientHoursReport GetPatientHoursReport(int caseID, DateTime startDate, DateTime endDate, int providerID)
        {
            var report = new PatientHoursReport();
            PrintingSettings.PassPdfDrawingExceptions = true;
            report.Parameters["CaseID"].Value = caseID;
            report.Parameters["StartDate"].Value = startDate;
            report.Parameters["EndDate"].Value = endDate;
            report.Parameters["ProviderID"].Value = providerID;
            return report;
        }

    }
}