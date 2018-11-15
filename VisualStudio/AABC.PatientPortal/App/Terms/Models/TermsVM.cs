using System;

namespace AABC.PatientPortal.App.Terms.Models
{
    public class TermsVM
    {
        public string Text { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsAccepted { get; set; }
    }
}