using System;

namespace AABC.Domain.Cases
{
    public class CaseProviderNote
    {
        public int? ID { get; set; }
        public int CaseProviderID { get; set; }
        public DateTime Date { get; set; }
        public string Note { get; set; }
    }
}
