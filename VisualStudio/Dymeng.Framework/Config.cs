using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Dymeng.Framework
{


    public class ConfigurationException : Exception
    {
        public ConfigurationException() : base() { }
        public ConfigurationException(string message) : base(message) { }
        public ConfigurationException(string format, params object[] args) : base(string.Format(format, args)) { }
        public ConfigurationException(string message, Exception innerException) : base(message, innerException) { }
        public ConfigurationException(string format, Exception innerException, params object[] args) : base(string.Format(format, args), innerException) { }   
    }

    public class Config
    {

        public IntegrationsConfig Integrations = new IntegrationsConfig();
        public ConnectionsConfig Connections = new ConnectionsConfig();
        public ExceptionsConfig Exceptions = new ExceptionsConfig();
        public DocumentsConfig Documents = new DocumentsConfig();


        public string SolutionName { get; set; }

        // Default Instance
        private static Config defaultConfig;
        //private static bool loadAttempted = false;
        private const string DEFAULT_FILENAME = "Dymeng.Framework.Config.xml";

        public static Config Default {
            get
            {
                if (defaultConfig == null) {
                    // prevent recursive attempts to load this if there's an internal failure
                    //if (loadAttempted) {
                    //    return null;
                    //}
                    //loadAttempted = true;
                    string filePath = getCodebaseDirectory();
                    filePath = Path.Combine(filePath, DEFAULT_FILENAME);
                    defaultConfig = new Config(filePath);
                }
                return defaultConfig;
            }
        }

        

        // CTORs
        public Config(string filePath) {
            try {
                load(filePath);
            } catch (Exception e) {
                throw new ConfigurationException(e.Message, e);
            }
        }
        
        void load(string filepath) {

            string tempVal = "";

            XmlDocument doc = new XmlDocument();
            doc.Load(filepath);

            SolutionName = doc.SelectSingleNode("/Config/SolutionName").Value;

            Connections.CoreConnection = doc.SelectSingleNode("/Config/ConnectionStrings/ConnectionString[@name='CoreConnection']").Attributes["connectionString"].Value;
            Connections.UserConnection = doc.SelectSingleNode("/Config/ConnectionStrings/ConnectionString[@name='UserConnection']").Attributes["connectionString"].Value;

            Exceptions.Logging.Paths = getExceptionLoggingPaths(doc);
            Exceptions.Logging.DbConnections = getExceptionLoggingDbConnections(doc);


            Documents.RootDirectory = doc.SelectSingleNode("/Config/Documents/RootDirectory").InnerText;

            tempVal = doc.SelectSingleNode("/Config/Documents/HoldingDirectory").InnerText;
            Documents.HoldDirectory = System.IO.Path.Combine(this.Documents.RootDirectory, tempVal);

            tempVal = doc.SelectSingleNode("/Config/Documents/Revisions/HoldingDirectory").InnerText;
            Documents.Revisions.HoldDirectory = System.IO.Path.Combine(this.Documents.RootDirectory, tempVal);


            Integrations.Clio.Authentication.Bearer = doc.SelectSingleNode("/Config/Integrations/Clio/Authentication/Bearer").InnerText;
            Integrations.Clio.Documents.Post.Endpoint = doc.SelectSingleNode("/Config/Integrations/Clio/Documents/Post/Endpoint").InnerText;
            Integrations.Clio.Documents.Post.UserAgent = doc.SelectSingleNode("/Config/Integrations/Clio/Documents/Post/UserAgent").InnerText;

        }


        List<string> getExceptionLoggingDbConnections(XmlDocument doc) {

            List<string> strings = new List<string>();

            XmlNodeList nodes = doc.SelectNodes("/Config/Exceptions/Logging/Databases/Connection");

            foreach (XmlNode node in nodes) {
                strings.Add(node.Attributes["connectionString"].Value);
            }

            return strings;

        }

        List<string> getExceptionLoggingPaths(XmlDocument doc) {

            List<string> paths = new List<string>();

            XmlNodeList nodes = doc.SelectNodes("/Config/Exceptions/Logging/Paths/Path");

            foreach (XmlNode node in nodes) {
                paths.Add(node.Value);
            }

            return paths;
        }

        static string getCodebaseDirectory() {
            string filePath = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(filePath);
            filePath = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(filePath);
        }




        // NESTED CONFIG CLASSES

        public class ConnectionsConfig
        {
            public string UserConnection { get; set; }
            public string CoreConnection { get; set; }
        }


        public class DocumentsConfig
        {
            public string RootDirectory { get; set; }
            public string HoldDirectory { get; set; }

            public RevisionsConfig Revisions = new RevisionsConfig();


            public class RevisionsConfig
            {
                public string HoldDirectory { get; set; }
            }

        }


        public class IntegrationsConfig
        {


            public ClioConfig Clio = new ClioConfig();


            public class ClioConfig
            {

                public AuthenticationConfig Authentication = new AuthenticationConfig();
                public DocumentConfig Documents = new DocumentConfig();

                public class AuthenticationConfig
                {
                    public string Bearer { get; set; }
                }

                public class DocumentConfig
                {

                    public PostConfig Post = new DocumentConfig.PostConfig();

                    public class PostConfig
                    {

                        public string Endpoint { get; set; }
                        public string UserAgent { get; set; }
                        public List<HeaderConfig> Headers = new List<HeaderConfig>();

                        public class HeaderConfig
                        {

                            public string ContentType { get; set; }

                        }

                    }

                }

            }

        }

        public class ExceptionsConfig
        {

            public LoggingConfig Logging = new LoggingConfig();
            

            public class LoggingConfig
            {
                public List<string> Paths { get; set; }
                public List<string> DbConnections { get; set; }
            }

        }

    }
}
