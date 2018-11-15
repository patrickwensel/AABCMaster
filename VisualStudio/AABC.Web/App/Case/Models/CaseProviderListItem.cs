using System;

namespace AABC.Web.Models.Cases
{
    public class CaseProviderListItem
    {

        public int ID { get; set; }
        public int ProviderID { get; set; }
        public bool Active { get; set; }
        public string Type { get; set; }
        public int? TypeID { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsSupervisor { get; set; }
        public bool IsAssessor { get; set; }
        public bool IsAuthorizedBCBA { get; set; }
        
    }
}