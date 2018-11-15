using AABC.Domain2.Cases;
using AABC.DomainServices.Sessions;
using DevExpress.DataAccess.Native.Sql;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using Dymeng.Framework.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace AABC.Web.Reports
{

    public class ReportService
    {


        public static ParentHoursReport GetParentHoursReport(int caseID, DateTime startDate, DateTime endDate, int providerID)
        {
            var report = new ParentHoursReport();
            var fonts = new PrivateFontCollection();
            fonts.AddFontFile(HttpContext.Current.Server.MapPath("~/Content/Fonts/KUNSTLER.TTF"));
            PrintingSettings.PassPdfDrawingExceptions = true;
            report.Parameters["CaseID"].Value = caseID;
            report.Parameters["StartDate"].Value = startDate;
            report.Parameters["EndDate"].Value = endDate;
            report.Parameters["ProviderID"].Value = providerID;
            return report;
        }


        public static PatientHoursReport GetPatientHoursReport(int caseID, DateTime startDate, DateTime endDate, int providerID)
        {
            var report = new PatientHoursReport();
            report.ParsedNotes.GetValue += ParseJson;
            var fonts = new PrivateFontCollection();
            fonts.AddFontFile(HttpContext.Current.Server.MapPath("~/Content/Fonts/KUNSTLER.TTF"));
            PrintingSettings.PassPdfDrawingExceptions = true;
            report.Parameters["CaseID"].Value = caseID;
            report.Parameters["StartDate"].Value = startDate;
            report.Parameters["EndDate"].Value = endDate;
            report.Parameters["ProviderID"].Value = providerID;
            return report;
        }


        public static XtraReport GetPatientHoursReportWithSignLine(int caseID, DateTime startDate, DateTime endDate, int providerID)
        {
            var report = new PatientHoursReportWithSignLine();
            report.ParsedNotes.GetValue += ParseJson;
            var fonts = new PrivateFontCollection();
            fonts.AddFontFile(HttpContext.Current.Server.MapPath("~/Content/Fonts/KUNSTLER.TTF"));
            PrintingSettings.PassPdfDrawingExceptions = true;
            report.Parameters["CaseID"].Value = caseID;
            report.Parameters["StartDate"].Value = startDate;
            report.Parameters["EndDate"].Value = endDate;
            report.Parameters["ProviderID"].Value = providerID;
            return report;
        }


        public static XtraReport GetPatientHoursReportWithSupervisingBCBA(int caseID, DateTime startDate, DateTime endDate, int providerID)
        {
            var report = new PatientHoursReportWithBCBASignLine();
            report.ParsedNotes.GetValue += ParseJson;
            var fonts = new PrivateFontCollection();
            fonts.AddFontFile(HttpContext.Current.Server.MapPath("~/Content/Fonts/KUNSTLER.TTF"));
            PrintingSettings.PassPdfDrawingExceptions = true;
            report.Parameters["CaseID"].Value = caseID;
            report.Parameters["StartDate"].Value = startDate;
            report.Parameters["EndDate"].Value = endDate;
            report.Parameters["ProviderID"].Value = providerID;
            return report;
        }


        public static ParentApprovalReport GetParentApprovalReport(ParentApproval approval)
        {
            var sigImg = GetSignature(approval);
            var report = new ParentApprovalReport();
            PrintingSettings.PassPdfDrawingExceptions = true;
            foreach (Band b in report.Bands)
            {
                foreach (XRControl c in b.Controls)
                {
                    if (c.Name == "xrPictureBox2")
                    {
                        ((XRPictureBox)c).Image = sigImg;
                    }
                }
            }
            report.Parameters["ParentApprovalID"].Value = approval.ID;
            return report;
        }


        private static Image GetSignature(ParentApproval approval)
        {
            // Get the most recent signature before the approval date.
            var sig = AppService.Current.DataContextV2.ParentSignatures
                .Where(x => x.LoginID == approval.ParentLoginID
                            && x.Date < approval.DateApproved)
                .OrderByDescending(x => x.Date).FirstOrDefault();

            // Convert transparent to white
            Bitmap sigImgWhite;
            if (sig != null)
            {
                var base64Data = Regex.Match(sig.Data, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                var binData = Convert.FromBase64String(base64Data);
                Image sigImg;
                using (var stream = new MemoryStream(binData))
                {
                    sigImg = new Bitmap(stream);
                }
                sigImgWhite = new Bitmap(sigImg.Width, sigImg.Height);
                var rect = new Rectangle(Point.Empty, sigImgWhite.Size);
                using (var G = Graphics.FromImage(sigImgWhite))
                {
                    G.Clear(Color.White);
                    G.DrawImageUnscaledAndClipped(sigImg, rect);
                }
            }
            else
            {
                sigImgWhite = new Bitmap(100, 100);
                using (var G = Graphics.FromImage(sigImgWhite))
                {
                    G.Clear(Color.White);
                }
            }
            return sigImgWhite;
        }


        public static string GenerateBillingReport(int caseID, DateTime firstDayOfPeriod)
        {
            var data = new DataHelper();
            var periodID = data.GetPeriodID(caseID, firstDayOfPeriod);
            var reportBaseID = data.GetReportBaseID(periodID);
            var reportsWithBaseID = data.GetReportCountByBaseID(reportBaseID);
            var reportID = reportBaseID.ToString();
            if (reportsWithBaseID > 0)
            {
                reportID += "-A" + reportsWithBaseID;
            }
            int? userID = null;
            try
            {
                userID = Global.Default.User().ID;
            }
            catch (Exception)
            {
                // ignore
            }
            data.RecordReport(reportBaseID, periodID, reportID, userID, firstDayOfPeriod, caseID);
            return reportID;
        }


        public static string GeneratePayrollReport(DateTime firstDayOfPeriod, App.Providers.Models.PayrollFilter filter)
        {
            var data = new DataHelper();
            var baseReportID = data.GetPayrollBaseReportID();
            data.InsertPayablesRecord(baseReportID, firstDayOfPeriod, filter);
            return baseReportID.ToString();
        }

        private static void ParseJson(object sender, GetValueEventArgs e) {
            {
                if (e.Row is ResultRow row)
                {
                    var text = row.ToArray()[19] as string ?? string.Empty;
                    if (IsValidJson(text))
                    {
                        var r = JsonConvert.DeserializeObject<SessionReport>(text);
                        e.Value = r.FormatForReport();
                    }
                    else
                    {
                        e.Value = text;
                    }
                }
            };
        }


        private static bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        public class DataHelper
        {


            // Provider stuff here
            public int GetPayrollBaseReportID()
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT MAX(ReportBaseID) AS MaxBaseID FROM dbo.CasePayableReports;";
                    DataRow r = cmd.GetRowOrNull();
                    if (r == null)
                    {
                        return GetNextReportBaseID();
                    }
                    else
                    {
                        int? ret = r.ToIntOrNull(0);
                        return ret == null ? 1000 : ret.Value + 1;
                    }
                }
            }


            public void InsertPayablesRecord(int reportID, DateTime firstDayOfPeriod, App.Providers.Models.PayrollFilter filter)
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO dbo.CasePayableReports (ReportBaseID, ReportID) " +
                        "VALUES (@BaseID, @ReportID);";
                    cmd.Parameters.AddWithValue("@BaseID", reportID);
                    cmd.Parameters.AddWithValue("@ReportID", reportID.ToString());
                    cmd.InsertToIdentity();
                    cmd.Parameters.Clear();
                    switch (filter)
                    {
                        case App.Providers.Models.PayrollFilter.NYOnly:
                            cmd.CommandText =
                                "UPDATE cah SET cah.HoursPayableRef = @ReportID FROM dbo.CaseAuthHours AS cah INNER JOIN dbo.Providers AS p ON cah.CaseProviderID = p.ID " +
                                "WHERE HoursDate >= @StartDate AND HoursDate <= @EndDate AND HoursStatus = 3 AND HoursPayableRef IS NULL " +
                                "  AND p.ProviderState = 'NY';";
                            break;
                        case App.Providers.Models.PayrollFilter.NonNY:
                            cmd.CommandText =
                                "UPDATE cah SET cah.HoursPayableRef = @ReportID FROM dbo.CaseAuthHours AS cah INNER JOIN dbo.Providers AS p ON cah.CaseProviderID = p.ID " +
                                "WHERE HoursDate >= @StartDate AND HoursDate <= @EndDate AND HoursStatus = 3 AND HoursPayableRef IS NULL " +
                                "  AND (p.ProviderState IS NULL OR p.ProviderState <> 'NY');";
                            break;
                        default:
                            cmd.CommandText = "UPDATE dbo.CaseAuthHours SET HoursPayableRef = @ReportID " +
                                "WHERE HoursDate >= @StartDate AND HoursDate <= @EndDate AND HoursStatus = 3 AND HoursPayableRef IS NULL;";
                            break;
                    }
                    cmd.Parameters.AddWithValue("@StartDate", firstDayOfPeriod);
                    cmd.Parameters.AddWithValue("@EndDate", firstDayOfPeriod.AddMonths(1).AddDays(-1));
                    cmd.Parameters.AddWithValue("@ReportID", reportID.ToString());
                    cmd.ExecuteNonQueryToInt();
                }
            }



            // Billing stuff below
            internal void RecordReport(int baseID, int periodID, string reportID, int? userID, DateTime firstDayOfPeriod, int caseID)
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO dbo.CaseBillingReports (ReportBaseID, ReportPeriodID, ReportID, ReportGeneratedByUserID) " +
                        "VALUES (@BaseID, @PeriodID, @ReportID, @UserID);";
                    cmd.Parameters.AddWithValue("@BaseID", baseID);
                    cmd.Parameters.AddWithValue("@PeriodID", periodID);
                    cmd.Parameters.AddWithValue("@ReportID", reportID);
                    cmd.Parameters.AddWithNullableValue("@UserID", userID);
                    cmd.InsertToIdentity();
                    cmd.Parameters.Clear();
                    cmd.CommandText = "UPDATE dbo.CaseAuthHours SET HoursBillingRef = @ReportID " +
                        "WHERE CaseID = @CaseID AND HoursDate >= @StartDate AND HoursDate <= @EndDate AND HoursStatus = 2 AND HoursBillingRef IS NULL;";
                    cmd.Parameters.AddWithValue("@CaseID", caseID);
                    cmd.Parameters.AddWithValue("@StartDate", firstDayOfPeriod);
                    cmd.Parameters.AddWithValue("@EndDate", firstDayOfPeriod.AddMonths(1).AddDays(-1));
                    cmd.Parameters.AddWithValue("@ReportID", reportID);
                    cmd.ExecuteNonQueryToInt();
                }
            }


            internal int GetReportCountByBaseID(int baseID)
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT COUNT(*) AS CountOfReports FROM dbo.CaseBillingReports WHERE ReportBaseID = @BaseID;";
                    cmd.Parameters.AddWithValue("@BaseID", baseID);
                    return cmd.ExecuteScalarToInt();
                }
            }


            internal int GetReportBaseID(int periodID)
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT ReportBaseID FROM dbo.CaseBillingReports WHERE ReportPeriodID = @Period GROUP BY ReportBaseID;";
                    cmd.Parameters.AddWithValue("@Period", periodID);
                    DataRow r = cmd.GetRowOrNull();
                    if (r == null)
                    {
                        return GetNextReportBaseID();
                    }
                    else
                    {
                        return r.ToInt(0);
                    }
                }
            }


            internal int GetNextReportBaseID()
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT MAX(ReportPeriodID) AS NextID FROM dbo.CaseBillingReports;";
                    DataRow r = cmd.GetRowOrNull();
                    if (r == null)
                    {
                        return 1000;
                    }
                    else
                    {
                        if (r[0] == DBNull.Value)
                        {
                            return 1000;
                        }
                        else
                        {
                            return r.ToInt(0) + 1;
                        }
                    }
                }
            }


            internal int GetPeriodID(int caseID, DateTime firstDayOfPeriod)
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT ID FROM dbo.CaseMonthlyPeriods WHERE CaseID = @CaseID AND PeriodFirstDayOfMonth = @Period;";
                    cmd.Parameters.AddWithValue("@CaseID", caseID);
                    cmd.Parameters.AddWithValue("@Period", firstDayOfPeriod);
                    DataRow row = cmd.GetRowOrNull();
                    if (row == null)
                    {
                        return CreatePeriodID(caseID, firstDayOfPeriod);
                    }
                    else
                    {
                        return row.ToInt(0);
                    }
                }
            }


            internal int CreatePeriodID(int caseID, DateTime firstDayOfPeriod)
            {
                using (var conn = new SqlConnection(connectionString))
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO dbo.CaseMonthlyPeriods (CaseID, PeriodFirstDayOfMonth) VALUES (@CaseID, @Period);";
                    cmd.Parameters.AddWithValue("@CaseID", caseID);
                    cmd.Parameters.AddWithValue("@Period", firstDayOfPeriod);
                    return cmd.InsertToIdentity();
                }
            }



            private readonly string connectionString;

            public DataHelper()
            {
                connectionString = ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString;
            }


        }

    }


}
