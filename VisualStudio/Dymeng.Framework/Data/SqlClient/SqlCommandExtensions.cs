using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Dymeng.Framework.Data.SqlClient
{
    public static class SqlCommandExtensions
    {
        
        public static void CreateInList(this SqlCommand cmd, string inParamName, IEnumerable<string> items, string listParamPrefix = "p") {
            CreateInList(cmd, inParamName, items.ToArray(), listParamPrefix);
        }

        public static void CreateInList(this SqlCommand cmd, string inParamName, IEnumerable<int> items, string listParamPrefix = "p") {
            CreateInList(cmd, inParamName, items.Select(x => x.ToString()).ToArray(), listParamPrefix);
        }

        public static void CreateInList(this SqlCommand cmd, string inParamName, IEnumerable<DateTime> items, string listParamPrefix = "p") {
            CreateInList(cmd, inParamName, items.Select(x => x.ToString("s")).ToArray(), listParamPrefix);
        }

        public static void CreateInList(this SqlCommand cmd, string inParamName, IEnumerable<double> items, string listParamPrefix = "p") {
            CreateInList(cmd, inParamName, items.Select(x => x.ToString()).ToArray(), listParamPrefix);
        }

        static void CreateInList(SqlCommand cmd, string baseParamName, string[] items, string listParamPrefix) {

            var p = items.Select((s, i) => "@" + listParamPrefix + i.ToString()).ToArray();

            cmd.CommandText = cmd.CommandText.Replace(baseParamName, string.Join(",", p));

            for (int i = 0; i < p.Length; i++) {
                cmd.Parameters.AddWithValue(p[i], items[i]);
            }
            
        }


        public static int ExecuteScalarToInt(this SqlCommand cmd) {
            var t = cmd.GetTable();

            if (t.Rows.Count != 1) {
                throw new DataSelectException("Expected row: 1, actual rows: " + t.Rows.Count);
            }

            DataRow r = t.Rows[0];

            try {
                return r.ToInt(0);
            }
            catch (Exception e) {
                Exceptions.Handle(e);
                throw new ArgumentException("Field ID not found");
            }
        }

        public static string ExecuteScalarToString(this SqlCommand cmd)
        {
            var t = cmd.GetTable();

            if (t.Rows.Count != 1) {
                throw new DataSelectException("Expected row: 1, actual rows: " + t.Rows.Count);
            }

            DataRow r = t.Rows[0];

            try {
                return r.ToStringValue(0);
            }
            catch (Exception e) {
                Exceptions.Handle(e);
                throw new ArgumentException("Field ID not found");
            }
        }


        /// <summary>
        /// Gets a DataSet for the specified Command
        /// </summary>
        /// <param name="cmd">System.Data.SqlClient.SqlCommand</param>
        /// <returns>System.Data.DataSet filled with information from the Command specifications</returns>
        /// <exception cref="Dymeng.Framework.Data.DataSelectException">Generic wrapper for inner exceptions</exception>
        public static DataSet GetDataSet(this SqlCommand cmd) {

            DataSet set = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            
            try {
                cmd.Connection.Open();
                adapter.Fill(set);
                cmd.Connection.Close();
                return set;
            } catch(Exception e) {

                throw new DataSelectException("Failure filling dataset", e) {
                    ConnectionString = cmd.Connection.ConnectionString,
                    CommandText = cmd.CommandText
                };
            }
            

            
        }

        /// <summary>
        /// Gets a DataSet for the specified Command and names the tables in the set per the specified tableNames array (ordinally matched)
        /// </summary>
        /// <param name="cmd">System.Data.SqlClient.SqlCommand</param>
        /// <returns>System.Data.DataSet filled with information from the Command specifications</returns>
        /// <exception cref="Dymeng.Framework.Data.DataSelectException">Generic wrapper for inner exceptions</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">Table names could not resolve to the actual tables in the set</exception>
        public static DataSet GetDataSet(this SqlCommand cmd, string[] tableNames) {

            DataSet set = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            try {
                cmd.Connection.Open();
                adapter.Fill(set);
                cmd.Connection.Close();
                
            } catch (Exception e) {
                throw new DataSelectException("Failure filling dataset", e)
                {
                    ConnectionString = cmd.Connection.ConnectionString,
                    CommandText = cmd.CommandText
                };
            }
            
            try {
                for (int i = 0; i < set.Tables.Count; i++) {
                    set.Tables[i].TableName = tableNames[i];
                }
            } catch (Exception e) {
                throw new ArgumentOutOfRangeException("Table Names array out of range", e);
            }

            return set;
        }

        /// <summary>
        /// Gets a DataTable for the specified Command.
        /// </summary>
        /// <param name="cmd">System.Data.SqlClient.SqlCommand</param>
        /// <returns>System.Data.DataTable filled with rows from the specified Command</returns>
        /// <exception cref="Dymeng.Framework.Data.DataSelectException">Generic wrapper for inner exceptions</exception>
        public static DataTable GetTable(this SqlCommand cmd) {

            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            
            try {
                cmd.Connection.Open();
                adapter.Fill(table);
                cmd.Connection.Close();
                return table;
            } catch (Exception e) {
                throw new DataSelectException("Failure filling dataset", e)
                {
                    ConnectionString = cmd.Connection.ConnectionString,
                    CommandText = cmd.CommandText
                };
            }
        }


        /// <summary>
        /// Gets a single DataRow for the specified Command.  Command must produce one and only one row.
        /// </summary>
        /// <param name="cmd">System.Data.SqlClient.SqlCommand</param>
        /// <returns>System.Data.DataRow filled from the specified Command</returns>
        /// <exception cref="Dymeng.Framework.Data.DataSingleRowExpectedException">More than one row was returned</exception>
        /// <exception cref="Dymeng.Framework.Data.DataSelectException">Generic wrapper for inner exceptions</exception>
        /// <exception cref="Dymeng.Framework.Data.DataRowNotFoundException">No rows were returned</exception>
        public static DataRow GetRow(this SqlCommand cmd) {

            try {
                DataTable table = cmd.GetTable();
                if (table.Rows.Count > 1) {
                    throw new DataSingleRowExpectedException("Single row expected, method returned " + table.Rows.Count + " rows.")
                    {
                        ConnectionString = cmd.Connection.ConnectionString,
                        CommandText = cmd.CommandText
                    };
                }
                if (table.Rows.Count == 0) {
                    throw new DataRowNotFoundException("Single row expected but was not found.")
                    {
                        ConnectionString = cmd.Connection.ConnectionString,
                        CommandText = cmd.CommandText
                    };
                }
                return table.Rows[0];

            } catch (DataSelectException e) {
                throw e;

            } catch (DataSingleRowExpectedException e) {
                throw e;

            } catch (DataRowNotFoundException e) {
                throw e;
            
            } catch (Exception e) {
                throw new DataSelectException("Failure selecting row", e)
                {
                    ConnectionString = cmd.Connection.ConnectionString,
                    CommandText = cmd.CommandText
                };
            }
            

            
        }

        /// <summary>
        /// Gets a single DataRow for the specified Command.  Command must produce no more than one row.
        /// </summary>
        /// <param name="cmd">System.Data.SqlClient.SqlCommand</param>
        /// <returns>System.Data.DataRow filled from the specified Command</returns>
        /// <exception cref="Dymeng.Framework.Data.DataSingleRowExpectedException">More than one row was returned</exception>
        /// <exception cref="Dymeng.Framework.Data.DataSelectException">Generic wrapper for inner exceptions</exception>
        public static DataRow GetRowOrNull(this SqlCommand cmd) {

            try {

                DataTable table = cmd.GetTable();

                if (table.Rows.Count == 0) {
                    return null;
                }

                if (table.Rows.Count > 1) {
                    throw new DataSingleRowExpectedException("Single or zero rows expected, returned " + table.Rows.Count + " rows.")
                    {
                        ConnectionString = cmd.Connection.ConnectionString,
                        CommandText = cmd.CommandText
                    };
                } else {
                    return table.Rows[0];
                }
            }

            catch (DataSelectException e) {
                throw e;

            }
            catch (DataSingleRowExpectedException e) {
                throw e;

            }
            catch (Exception e) {
                throw new DataSelectException("Failure selecting row", e)
                {
                    ConnectionString = cmd.Connection.ConnectionString,
                    CommandText = cmd.CommandText
                };
            }

            
        }



        /// <summary>
        /// Executes a non-query and returns the number of rows affected
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>int: number of rows affected</returns>
        /// <exception cref="Dymeng.Framework.Data.DataExecutionException">Generic wrapper for inner exceptions</exception>
        public static int ExecuteNonQueryToInt(this SqlCommand cmd) {

            try {
                int i = 0;

                cmd.Connection.Open();
                i = cmd.ExecuteNonQuery();
                cmd.Connection.Close();

                return i;
            } catch (Exception e) {
                throw new DataExecutionException("Error executing command", e)
                {
                    ConnectionString = cmd.Connection.ConnectionString,
                    CommandText = cmd.CommandText
                };
            }
            
        }


        /// <summary>
        /// Inserts a row and returns the SCOPE_IDENTITY of the insert, or null if not inserted
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>int? SCOPE_IDENTITY value of the insert</returns>
        /// <exception cref="Dymeng.Framework.Data.DataInsertException">Generic wrapper for inner exceptions</exception>
        public static int? InsertToIdentityOrNull(this SqlCommand cmd) {
            
            try {
                cmd.CommandText += ";SELECT SCOPE_IDENTITY();";

                cmd.Connection.Open();
                object o = cmd.ExecuteScalar();
                cmd.Connection.Close();

                return DBConvert.ToIntOrNull(o);

            } catch (Exception e) {
                throw new DataInsertException("Unable to insert row as specified", e)
                {
                    ConnectionString = cmd.Connection.ConnectionString,
                    CommandText = cmd.CommandText
                };
            }
            
            
        }

        /// <summary>
        /// Inserts a row and returns the SCOPE_IDENTITY of the insert
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>int? SCOPE_IDENTITY value of the insert</returns>
        /// <exception cref="Dymeng.Framework.Data.DataInsertException">Generic wrapper for inner exceptions</exception>
        public static int InsertToIdentity(this SqlCommand cmd) {

            try {
                cmd.CommandText += ";SELECT SCOPE_IDENTITY();";

                cmd.Connection.Open();
                object o = cmd.ExecuteScalar();
                cmd.Connection.Close();
                return DBConvert.ToInt(o);

            } catch (ArgumentNullException e) {
                throw new DataInsertException("Failure inserting row.", e)
                {
                    ConnectionString = cmd.Connection.ConnectionString,
                    CommandText = cmd.CommandText
                };

            } catch (Exception e) {
                throw new DataInsertException("Failure inserting row.", e)
                {
                    ConnectionString = cmd.Connection.ConnectionString,
                    CommandText = cmd.CommandText
                };
            } finally {
                if (cmd.Connection.State != ConnectionState.Closed) {
                    cmd.Connection.Close();
                }
            }
            
        }


    }
}
