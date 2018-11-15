using System;
using System.Data;
using System.Data.SqlClient;

namespace AABC.Web.App.Systems
{
    public interface ITransactionLogger
    {
        void Log(TransactionLogEntry t);
    }

    public class TransactionLogEntry
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string TransactionId { get; set; }
    }

    public class TransactionLogger : ITransactionLogger
    {

        private readonly string ConnectionString;

        public TransactionLogger(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public void Log(TransactionLogEntry t)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string sql = "INSERT INTO TransactionLog (Name,Phone,Email,TransactionId) VALUES(@param1,@param2,@param3,@param4)";
                SqlCommand cmd = new SqlCommand(sql, connection);
                cmd.Parameters.Add("@param1", SqlDbType.NVarChar, 100).Value = t.Name;
                cmd.Parameters.Add("@param2", SqlDbType.VarChar, 50).Value = t.Phone;
                cmd.Parameters.Add("@param3", SqlDbType.VarChar, 200).Value = t.Email;
                cmd.Parameters.Add("@param4", SqlDbType.VarChar, 50).Value = !string.IsNullOrEmpty(t.TransactionId) ? (object)t.TransactionId : DBNull.Value;
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
            }
        }
    }
}