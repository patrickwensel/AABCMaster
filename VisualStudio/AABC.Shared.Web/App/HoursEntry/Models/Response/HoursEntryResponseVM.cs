using System.Collections.Generic;

namespace AABC.Shared.Web.App.HoursEntry.Models.Response
{

    public enum MessageSeverity
    {
        General = 0,
        Warning = 1,
        Error = 2
    }

    public class HoursEntryResponseVM
    {
        public bool WasProcessed { get; set; }  // true if the hours were entered/udpated successfully
        public int? HoursID { get; set; }        // returns the hours ID, either existing or newly created if applicable
        public List<HoursEntryResponseMessage> Messages { get; set; } = new List<HoursEntryResponseMessage>();
    }
    
    public class HoursEntryResponseMessage
    {
        public MessageSeverity Severity { get; set; }
        public string Message { get; set; }
    }

}