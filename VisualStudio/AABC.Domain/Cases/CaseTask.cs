using System;

namespace AABC.Domain.Cases
{
    public class CaseTask
    {
        public int? ID { get; set; }
        public DateTime? DateCreated { get; set; }

        public DateTime EnteredOn { get; set; }
        public string Description { get; set; }
        public bool Complete { get; set; }
        public DateTime? CompletedOn { get; set; }
        public OfficeStaff.OfficeStaff CompletedBy { get; set; }

    }
}
