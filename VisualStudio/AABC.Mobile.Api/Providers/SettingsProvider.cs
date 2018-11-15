using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AABC.Mobile.Api
{
    public class SettingsProvider : ISettingsProvider {

        // These keys are what the client mobile app expects.
        // However, they're also what's stored in web.config or the database list
        // So, if you see them used as a key to retrieve a value, that's happenstance...
        // but do not change the key here unless the mobile app is updated accordingly
        const string CLIENTKEY_ACTIVESESSION_ABANDON_ENABLED = "ActiveSession.Abandon.Enabled";
        const string CLIENTKEY_ACTIVESESSION_ABANDON_TIMEOUTMINUTES = "ActiveSession.Abandon.TimeoutMinutes";
        const string CLIENTKEY_ACTIVESESSION_ABANDON_DATECROSSOVER_CUTOFF_ENABLED = "ActiveSession.DateCrossover.Cutoff.Enabled";
        const string CLIENTKEY_ACTIVESESSION_ABANDON_DATECROSSOVER_CUTOFF_NOTIFICATION = "ActiveSession.DateCrossover.Cutoff.Notification";
        const string CLIENTKEY_ACTIVESESSION_ABANDON_DATECROSSOVER_CUTOFF_NOTIFICATIONMINUTES = "ActiveSession.DateCrossover.Cutoff.NotificationMinutes";
        const string CLIENTKEY_ACTIVESESSION_GPS_TRACKLEAVE = "ActiveSession.GPS.TrackLeave";
        const string CLIENTKEY_COMMUNICATION_TIMEOUTSECONDS = "Communication.TimeoutSeconds";
        const string CLIENTKEY_NOTEENTRY_ANSWERVALIDATION_REGEX = "NoteEntry.AnswerValidation.Regex";
        const string CLIENTKEY_NOTEENTRY_ANSWERVALIDATION_MESSAGE = "NoteEntry.AnswerValidation.Message";
        const string CLIENTKEY_TESTFEATURE_INPAGE_ENABLED = "TestFeature.InPage.Enabled";
        const string CLIENTKEY_TESTFEATURE_TABNAV_ENABLED = "TestFeature.TabNav.Enabled";
        const string CLIENTKEY_VERSION_SERVER = "Version.Server";
        const string CLIENTKEY_VERSION_CLIENT_MINIMUM = "Version.Client.Minimum";



        // internal settings (not used by client app)
        public bool ServerMode_Demo_Enabled => getBoolFromWebConfig("ServerMode.Demo.Enabled");


        // returns a list of the client app settings
        public List<KeyValuePair<string, object>> AllClientAppSettings {
            get {

                var result = new List<KeyValuePair<string, object>>();

                result.Add(new KeyValuePair<string, object>(this.ActiveSession_Abandon_Enabled.Key, this.ActiveSession_Abandon_Enabled.Value));
                result.Add(new KeyValuePair<string, object>(this.ActiveSession_Abandon_TimeoutMinutes.Key, this.ActiveSession_Abandon_TimeoutMinutes.Value));
                result.Add(new KeyValuePair<string, object>(this.ActiveSession_DateCrossover_Cutoff_Enabled.Key, this.ActiveSession_DateCrossover_Cutoff_Enabled.Value));
                result.Add(new KeyValuePair<string, object>(this.ActiveSession_DateCrossover_Cutoff_Notification.Key, this.ActiveSession_DateCrossover_Cutoff_Notification.Value));
                result.Add(new KeyValuePair<string, object>(this.ActiveSession_DateCrossover_Cutoff_NotificationMinutes.Key, this.ActiveSession_DateCrossover_Cutoff_NotificationMinutes.Value));
                result.Add(new KeyValuePair<string, object>(this.Communication_TimeoutSeconds.Key, this.Communication_TimeoutSeconds.Value));
                result.Add(new KeyValuePair<string, object>(this.NoteEntry_AnswerValidation_Message.Key, this.NoteEntry_AnswerValidation_Message.Value));
                result.Add(new KeyValuePair<string, object>(this.NoteEntry_AnswerValidation_Regex.Key, this.NoteEntry_AnswerValidation_Regex.Value));
                result.Add(new KeyValuePair<string, object>(this.TestFeature_InPage_Enabled.Key, this.TestFeature_InPage_Enabled.Value));
                result.Add(new KeyValuePair<string, object>(this.TestFeature_TabNav_Enabled.Key, this.TestFeature_TabNav_Enabled.Value));
                result.Add(new KeyValuePair<string, object>(this.Version_Client_MinimumSupported.Key, this.Version_Client_MinimumSupported.Value));
                result.Add(new KeyValuePair<string, object>(this.Version_Server.Key, this.Version_Server.Value));

                return result;
            }


        }


        // client app settings (needs a key reported for the client to match on)
        public KeyValuePair<string, bool> ActiveSession_Abandon_Enabled { get {
                bool b = getBoolFromWebConfig(CLIENTKEY_ACTIVESESSION_ABANDON_ENABLED);
                return new KeyValuePair<string, bool>(CLIENTKEY_ACTIVESESSION_ABANDON_ENABLED, b);
            }
        }
        
        public KeyValuePair<string, int> ActiveSession_Abandon_TimeoutMinutes { get {
                int i = getIntFromWebConfig(CLIENTKEY_ACTIVESESSION_ABANDON_TIMEOUTMINUTES);
                return new KeyValuePair<string, int>(CLIENTKEY_ACTIVESESSION_ABANDON_TIMEOUTMINUTES, i);
            }
        }

        public KeyValuePair<string, bool> ActiveSession_DateCrossover_Cutoff_Enabled {
            get {
                bool b = getBoolFromWebConfig(CLIENTKEY_ACTIVESESSION_ABANDON_DATECROSSOVER_CUTOFF_ENABLED);
                return new KeyValuePair<string, bool>(CLIENTKEY_ACTIVESESSION_ABANDON_DATECROSSOVER_CUTOFF_ENABLED, b);
            }
        }

        public KeyValuePair<string, bool> ActiveSession_DateCrossover_Cutoff_Notification {
            get {
                bool b = getBoolFromWebConfig(CLIENTKEY_ACTIVESESSION_ABANDON_DATECROSSOVER_CUTOFF_NOTIFICATION);
                return new KeyValuePair<string, bool>(CLIENTKEY_ACTIVESESSION_ABANDON_DATECROSSOVER_CUTOFF_NOTIFICATION, b);
            }
        }

        public KeyValuePair<string, int> ActiveSession_DateCrossover_Cutoff_NotificationMinutes {
            get {
                int i = getIntFromWebConfig(CLIENTKEY_ACTIVESESSION_ABANDON_DATECROSSOVER_CUTOFF_NOTIFICATIONMINUTES);
                return new KeyValuePair<string, int>(CLIENTKEY_ACTIVESESSION_ABANDON_DATECROSSOVER_CUTOFF_NOTIFICATIONMINUTES, i);
            }
        }

        public KeyValuePair<string, int> Communication_TimeoutSeconds {
            get {
                int i = getIntFromWebConfig(CLIENTKEY_COMMUNICATION_TIMEOUTSECONDS);
                return new KeyValuePair<string, int>(CLIENTKEY_COMMUNICATION_TIMEOUTSECONDS, i);
            }
        }

        public KeyValuePair<string, string> NoteEntry_AnswerValidation_Regex {
            get {
                //string s = getStringFromDatabase(CLIENTKEY_NOTEENTRY_ANSWERVALIDATION_REGEX);
                string s = getStringFromWebConfig(CLIENTKEY_NOTEENTRY_ANSWERVALIDATION_REGEX);
                return new KeyValuePair<string, string>(CLIENTKEY_NOTEENTRY_ANSWERVALIDATION_REGEX, s);
            }
        }

        public KeyValuePair<string, string> NoteEntry_AnswerValidation_Message {
            get {
                //string s = getStringFromDatabase(CLIENTKEY_NOTEENTRY_ANSWERVALIDATION_MESSAGE);
                string s = getStringFromWebConfig(CLIENTKEY_NOTEENTRY_ANSWERVALIDATION_MESSAGE);
                return new KeyValuePair<string, string>(CLIENTKEY_NOTEENTRY_ANSWERVALIDATION_MESSAGE, s);
            }
        }

        

        public KeyValuePair<string, bool> TestFeature_InPage_Enabled {
            get {
                bool b = getBoolFromWebConfig(CLIENTKEY_TESTFEATURE_INPAGE_ENABLED);
                return new KeyValuePair<string, bool>(CLIENTKEY_TESTFEATURE_INPAGE_ENABLED, b);
            }
        }

        public KeyValuePair<string, bool> TestFeature_TabNav_Enabled {
            get {
                bool b = getBoolFromWebConfig(CLIENTKEY_TESTFEATURE_TABNAV_ENABLED);
                return new KeyValuePair<string, bool>(CLIENTKEY_TESTFEATURE_TABNAV_ENABLED, b);
            }
        }

        public KeyValuePair<string, string> Version_Client_MinimumSupported {
            get {
                string s = getStringFromWebConfig(CLIENTKEY_VERSION_CLIENT_MINIMUM);
                return new KeyValuePair<string, string>(CLIENTKEY_VERSION_CLIENT_MINIMUM, s);
            }
        }

        public KeyValuePair<string, string> Version_Server {
            get {
                string s = getStringFromWebConfig(CLIENTKEY_VERSION_SERVER);
                return new KeyValuePair<string, string>(CLIENTKEY_VERSION_SERVER, s);
            }
        }



        





        private bool getBoolFromWebConfig(string key) {
            string value = System.Configuration.ConfigurationManager.AppSettings[key];
            string s = value.ToString();
            if (s == "True" || s == "true") {
                return true;
            }
            return false;
        }

        private int getIntFromWebConfig(string key) {
            string value = System.Configuration.ConfigurationManager.AppSettings[key];
            return int.Parse(value.ToString());
        }

        private string getStringFromWebConfig(string key) {
            string value = System.Configuration.ConfigurationManager.AppSettings[key];
            return value.ToString();
        }

        private string getStringFromDatabase(string key) {
            throw new NotImplementedException();
        }


    }



}