using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AABC.Mobile.Api
{
    public interface ISettingsProvider
    {

        // These are the settings that go to the client
        // the key is the client key (the one the client expects to recieve)
        KeyValuePair<string, bool> ActiveSession_Abandon_Enabled { get; }
        KeyValuePair<string, int> ActiveSession_Abandon_TimeoutMinutes { get; }
        KeyValuePair<string, bool> ActiveSession_DateCrossover_Cutoff_Enabled { get; }
        KeyValuePair<string, bool> ActiveSession_DateCrossover_Cutoff_Notification { get; }
        KeyValuePair<string, int> ActiveSession_DateCrossover_Cutoff_NotificationMinutes { get; }
        KeyValuePair<string, int> Communication_TimeoutSeconds { get; }
        KeyValuePair<string, string> NoteEntry_AnswerValidation_Regex { get; }
        KeyValuePair<string, string> NoteEntry_AnswerValidation_Message { get; }        
        KeyValuePair<string, bool> TestFeature_InPage_Enabled { get; }
        KeyValuePair<string, bool> TestFeature_TabNav_Enabled { get; }
        KeyValuePair<string, string> Version_Client_MinimumSupported { get; }
        KeyValuePair<string, string> Version_Server { get; }

        // these are used internally
        // they don't need a key returned with the value
        bool ServerMode_Demo_Enabled { get; }


        List<KeyValuePair<string, object>> AllClientAppSettings { get; }

    }

    
}