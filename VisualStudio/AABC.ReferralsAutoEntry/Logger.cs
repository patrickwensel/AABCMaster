using System.Configuration;
using System.IO;

namespace AABC.ReferralsAutoEntry
{
    class Logger
    {
        string _logPath;

        public Logger()
        {
            _logPath = ConfigurationManager.AppSettings["LogPath"];
            WriteLog("*** New Session Started ***");

        }

        public Logger(string startMode) {
            // startmode is not used but allows me to pass a 
            // value from the program.cs so it doesn't write the 
            // New Session Started twice
            _logPath = ConfigurationManager.AppSettings["LogPath"];
        }

        public void LogMessage(string msg)
        {
            WriteLog(msg);
        }

        public void LogError(string msg)
        {
            WriteLog("[ERROR] " + msg);
        }

        private void WriteLog(string msg)
        {
            using (StreamWriter logWriter = new StreamWriter(_logPath, true))
            {
                logWriter.WriteLine(System.DateTime.Now.ToString("G") + " " + msg);
            }
        }

    }
}
