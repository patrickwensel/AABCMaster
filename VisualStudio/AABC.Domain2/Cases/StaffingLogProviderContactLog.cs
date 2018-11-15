using System;

namespace AABC.Domain2.Cases
{
    public class StaffingLogProviderContactLog
    {
        public int ID { get; set; }
        public int StaffingLogProviderID { get; set; }
        public int StatusID { get; set; }
        public DateTime ContactDate { get; set; }
        public string Notes { get; set; }
        public DateTime? FollowUpDate { get; set; }

        public DateTime DateCreated { get; set; }
        public int CreatedByUserID { get; set; }


        public virtual StaffingLogProviderStatus Status { get; set; }
        public virtual StaffingLogProvider StaffingLogProvider { get; set; }
    }
}
