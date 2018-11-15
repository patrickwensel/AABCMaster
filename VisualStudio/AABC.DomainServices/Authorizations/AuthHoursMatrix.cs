using System.Data;
using System.Data.SqlClient;

namespace AABC.DomainServices.Authorizations
{
    public class AuthHoursMatrix
    {
        const int MAX_REQUESTED_MONTH_SPREAD = 12;
        private readonly string ConnectionString;
        private readonly int AuthHoursGridShowFutureStartDays;

        public AuthHoursMatrix(string connectionString, int authHoursGridShowFutureStartDays)
        {
            ConnectionString = connectionString;
            AuthHoursGridShowFutureStartDays = authHoursGridShowFutureStartDays;
        }


        public DataTable GetMatrixItems(int caseId, bool ignoreBCBAAuths)
        {
            var ds = new DataSet("GetAuthMatrixSource");
            using (var con = new SqlConnection(ConnectionString))
            using (var cmd = new SqlCommand("dbo.GetAuthMatrixSource", con))
            using (var da = new SqlDataAdapter())
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@caseId", caseId));
                cmd.Parameters.Add(new SqlParameter("@ignoreBCBAAuths", ignoreBCBAAuths));
                cmd.Parameters.Add(new SqlParameter("@numberOfMonthsRecentlyTerminated", 1));
                cmd.Parameters.Add(new SqlParameter("@numberOfDaysUpcomingAuthWindow", AuthHoursGridShowFutureStartDays));
                cmd.Parameters.Add(new SqlParameter("@maxMonthSpread", MAX_REQUESTED_MONTH_SPREAD));
                con.Open();
                da.SelectCommand = cmd;
                da.Fill(ds);
            }
            return ds.Tables[0];
        }


    }
}
