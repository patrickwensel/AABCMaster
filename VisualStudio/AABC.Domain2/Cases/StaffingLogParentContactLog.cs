using System;

namespace AABC.Domain2.Cases
{
    public class StaffingLogParentContactLog
    {
        public int ID { get; set; }
        public int GuardianRelationshipID { get; set; }
        public int StaffingLogID { get; set; }
        public string ContactedPersonName { get; set; }
        public DateTime ContactDate { get; set; }
        public ContactMethodTypes ContactMethodType { get; set; }
        public string ContactMethodValue { get; set; }

        public string Notes { get; set; }

        public DateTime DateCreated { get; set; }
        public int CreatedByUserID { get; set; }

        public virtual StaffingLog StaffingLog { get; set; }
    }
}
