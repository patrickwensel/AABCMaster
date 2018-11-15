using System;

namespace AABC.DomainServices.Integrations.Catalyst
{
    public class HasDataResultItem
    {
        public string Result { get; set; }
        public int? CaseID { get; set; }
        public int? ProviderID { get; set; }
        public DateTime? VisitDate { get; set; }
        public string ProviderInitials { get; set; }
        public string StudentName { get; set; }
    }
}
