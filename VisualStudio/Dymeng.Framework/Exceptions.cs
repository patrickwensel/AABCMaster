using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;

namespace Dymeng.Framework
{

    public static class ExceptionExtensions
    {

        public static string GetMessages(this Exception e)
        {
            return GetMessages(e);
        }


        private static string GetMessages(Exception e, string msgs = "")
        {

            if (e == null)
            {
                return string.Empty;
            }

            if (msgs == "")
            {
                msgs = e.Message;
            }

            if (e.InnerException != null)
            {
                msgs += "\r\nInner Exception: " + GetMessages(e.InnerException, msgs);
            }

            return msgs;

        }
    }


    public class Exceptions
    {

        public static void LogMessageToTelementry(string message, string appInfo = null)
        {
            var endpoint = "http://telemetry.dymeng.com/v1/Errors/Submit";
            var agent = "CID=23&PID=12&MID=" + Environment.MachineName; // + "UID=Username";
            var info = "MESSAGE: " + message;
            if (!string.IsNullOrEmpty(appInfo))
            {
                agent += "&AID=" + appInfo;
            }
            var content = agent + "|" + info;
            try
            {
                var request = WebRequest.Create(endpoint);
                request.Method = "POST";

                byte[] bytearray = Encoding.UTF8.GetBytes(content);
                request.ContentLength = bytearray.Length;

                var stream = request.GetRequestStream();
                stream.Write(bytearray, 0, bytearray.Length);
                stream.Close();

                try
                {
                   var response = request.GetResponse();
                    response.Close();
                }
                catch { }
            }
            catch { }
        }


        public static void Handle(Exception e, string appInfo = null)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("EXCEPTION ------- ");
                System.Diagnostics.Debug.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine("----");
                System.Diagnostics.Debug.Write(e.ToString());
                System.Diagnostics.Debug.WriteLine("----");
                LogToFile(e);
                LogToTelemetry(e, appInfo);
            }
            catch { }
            //if (Config.Default.Exceptions.Logging.Paths.Count > 0) {
            //    logToFiles(e, Config.Default.Exceptions.Logging.Paths);
            //}
            //if (Config.Default.Exceptions.Logging.DbConnections.Count > 0) {
            //    logToDbs(e, Config.Default.Exceptions.Logging.DbConnections);
            //}
        }


        private static void LogToTelemetry(Exception e, string appInfo = null)
        {
            LogMessageToTelementry(e.ToString(), appInfo);
            //string endpoint = "http://telemetry.dymeng.com/v1/Errors/Submit";
            //string agent = "CID=23&PID=12&MID=" + Environment.MachineName; // + "UID=Username";
            //string info = e.ToString();
            //if (!string.IsNullOrEmpty(appInfo))
            //{
            //    agent = "&AID=" + appInfo;
            //}
            //string content = agent + "|" + info;

            //var request = WebRequest.Create(endpoint);
            //request.Method = "POST";

            //byte[] bytearray = Encoding.UTF8.GetBytes(content);
            //request.ContentLength = bytearray.Length;

            //var stream = request.GetRequestStream();
            //stream.Write(bytearray, 0, bytearray.Length);
            //stream.Close();

            //try
            //{
            //    var response = request.GetResponseAsync();
            //}
            //catch { }
        }


        private static void LogToDbs(Exception e, List<string> connections)
        {

            foreach (string connStr in connections)
            {

                using (SqlConnection conn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand())
                {

                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO dbo.ExceptionLog (SolutionName, Message) VALUES (@SolutionName, @Message);";

                    cmd.Parameters.AddWithValue("@SolutionName", Config.Default.SolutionName);
                    cmd.Parameters.AddWithValue("@Message", e.ToString());

                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        // what do?
                        // TODO: failover to file logging
                    }
                }

            }
        }


        private static void LogToFile(Exception e)
        {
            try
            {
                string path = System.Configuration.ConfigurationManager.AppSettings["LogPath"];
                File.AppendAllText(path, GetPrintableMessage(e));
            }
            catch { }
        }


        private static string GetPrintableMessage(Exception e)
        {
            string message = Environment.NewLine + "EXCEPTION ------- " + DateTime.Now.ToString("yyyy-MM-dd_HHmmss") + Environment.NewLine;
            message += e.Message + Environment.NewLine;
            message += "----" + Environment.NewLine;
            message += e.ToString() + Environment.NewLine;
            message += "----" + Environment.NewLine;
            return message;
        }


        private static void LogToFiles(Exception e, List<string> paths)
        {
            foreach (var p in paths)
            {
                File.AppendAllText(p, GetPrintableMessage(e));
            }
        }

    }
}
