using System;
using System.Configuration;
using System.Data.SqlClient;

namespace AABC.DomainServices.HoursResolution.Logging
{
    public class HoursResolutionLoggingService
    {
        public string ConnectionString { get; private set; }

        public HoursResolutionLoggingService(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            ConnectionString = connectionString;
        }


        public void Log(HoursResolutionLoggingEntry e)
        {
            try
            {
                if (e == null) throw new ArgumentNullException(nameof(e));
                using (var connection = new SqlConnection(ConnectionString))
                {
                    var query = @"INSERT INTO dbo.CaseAuthHoursBreakdownLog (
                                WasResolved,
                                HoursID,
                                HoursDate,
                                ServiceID,
                                BillableHours,
                                ProviderTypeID,
                                InsuranceID,
                                AuthMatchRuleDetailJSON,
                                ActiveAuthorizationsJSON,
                                ResolvedAuthID,
                                ResolvedAuthCode,
                                ResolvedCaseAuthID,
                                ResolvedCaseAuthStart,
                                ResolvedCaseAuthEndDate,
                                ResolvedAuthMatchRuleID,
                                ResolvedMinutes
                            ) VALUES (
                                @WasResolved,
                                @HoursID,
                                @HoursDate,
                                @ServiceID,
                                @BillableHours,
                                @ProviderTypeID,
                                @InsuranceID,
                                @AuthMatchRuleDetailJSON,
                                @ActiveAuthorizationsJSON,
                                @ResolvedAuthID,
                                @ResolvedAuthCode,
                                @ResolvedCaseAuthID,
                                @ResolvedCaseAuthStart,
                                @ResolvedCaseAuthEndDate,
                                @ResolvedAuthMatchRuleID,
                                @ResolvedMinutes
                            )";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@WasResolved", e.WasResolved);
                        command.Parameters.AddWithValue("@HoursID", e.HoursID);
                        command.Parameters.AddWithValue("@HoursDate", e.HoursDate);
                        command.Parameters.AddWithValue("@ServiceID", e.ServiceID);
                        command.Parameters.AddWithValue("@BillableHours", e.BillableHours);
                        command.Parameters.AddWithValue("@ProviderTypeID", e.ProviderTypeID);
                        command.Parameters.AddWithValue("@InsuranceID", e.InsuranceID);
                        command.Parameters.AddWithValue("@AuthMatchRuleDetailJSON", (object)e.AuthMatchRuleDetailJSON ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ActiveAuthorizationsJSON", (object)e.ActiveAuthorizationsJSON ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ResolvedAuthID", (object)e.ResolvedAuthID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ResolvedAuthCode", (object)e.ResolvedAuthCode ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ResolvedCaseAuthID", (object)e.ResolvedCaseAuthID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ResolvedCaseAuthStart", (object)e.ResolvedCaseAuthStart ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ResolvedCaseAuthEndDate", (object)e.ResolvedCaseAuthEndDate ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ResolvedAuthMatchRuleID", (object)e.ResolvedAuthMatchRuleID ?? DBNull.Value);
                        command.Parameters.AddWithValue("@ResolvedMinutes", (object)e.ResolvedMinutes ?? DBNull.Value);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception exc)
            {
                Console.Write($"HourResolutionLoggingService: {exc.Message}");
            }
        }

        public static HoursResolutionLoggingService Create()
        {
            var enable = true;
            bool.TryParse(ConfigurationManager.AppSettings["EnableResolutionServiceLogging"], out enable);
            var connectionString = ConfigurationManager.ConnectionStrings["CoreConnection"].ConnectionString;
            return new HoursResolutionLoggingService(connectionString);
        }

    }
}
